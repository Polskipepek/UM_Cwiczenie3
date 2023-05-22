namespace UM_Cwiczenie3.ML.Sentiments;
internal class SentimentAnalyzer {

    public ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainSet) {
        var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features")).Fit(trainSet);
        return estimator;
    }

    public CalibratedBinaryClassificationMetrics Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet) {
        var predictions = model.Transform(splitTestSet);
        CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions);
        return metrics;

    }

    public void PrintPrediction(IEnumerable<SentimentPrediction> predictedResults) {
        Console.WriteLine();
        foreach (SentimentPrediction prediction in predictedResults) {
            Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability:P2} ");
        }
        Console.WriteLine("=============== End of predictions ===============");
    }

    public IEnumerable<SentimentPrediction> PredictSentiments(MLContext mlContext, ITransformer model) {
        int batchCount = BetterInput.GetKeyNumber("\r\nWrite Number of items to predict", 1, 10);
        List<SentimentData> sentiments = new(Enumerable.Range(0, batchCount).Select(x => new SentimentData { SentimentText = BetterInput.GetReadLine("Sentiment Text").ToString(), Sentiment = BetterInput.GetKeyNumberSameLine("Sentiment value [0-1]", 0, 1, false) == 1 }));
        IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);
        IDataView predictions = model.Transform(batchComments);
        IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
        return predictedResults;
    }

    public ITransformer BuildBinaryAnalyzerModel(MLContext mlContext, string dataPath) {
        TrainTestData splitDataView = DataLoader.LoadData<SentimentData>(mlContext, dataPath, hasHeader: false, separatorChar: '\t');
        ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
        var metrics = Evaluate(mlContext, model, splitDataView.TestSet);
        PrintMetrics(metrics);
        return model;
    }

    private void PrintMetrics(CalibratedBinaryClassificationMetrics metrics) {
        Console.WriteLine();
        Console.WriteLine("Model quality metrics evaluation");
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        Console.WriteLine("=============== End of model evaluation ===============");
    }
}
