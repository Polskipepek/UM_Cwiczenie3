namespace UM3.ML.Sentiments;
internal class SentimentAnalyzer {

    public ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainSet) {
        var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features")).Fit(trainSet);
        return estimator;
    }

    public void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet) {
        var predictions = model.Transform(splitTestSet);
        CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions);

        Console.WriteLine();
        Console.WriteLine("Model quality metrics evaluation");
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        Console.WriteLine("=============== End of model evaluation ===============");
    }

    public void UseModelWithBatchItems(MLContext mlContext, ITransformer model, IEnumerable<SentimentData> sentiments) {
        IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);
        IDataView predictions = model.Transform(batchComments);
        IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);

        Console.WriteLine();
        foreach (SentimentPrediction prediction in predictedResults) {
            Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability:P2} ");
        }
        Console.WriteLine("=============== End of predictions ===============");
    }
}
