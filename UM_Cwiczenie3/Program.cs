// See https://aka.ms/new-console-template for more information

using UM_Cwiczenie3.ML.Clustering.Model;
using UM_Cwiczenie3.ML.Sentiments.Model;

Console.WriteLine("Hello, World!");
Console.WriteLine("Select classification type:");

MLContext mlContext = new();

while (true) {
    FunctionType function = (FunctionType)BetterInput.GetKeyNumber("1. Binary Classification\r\n2. Clustering", 1, 2);

    switch (function) {
        case FunctionType.Sentiment:
            SentimentAnalyzer sentimentAnalyzer = new();
            ITransformer model = CreateSentimentModel(mlContext, sentimentAnalyzer);

            while (true) {
                IEnumerable<SentimentPrediction> predictions = sentimentAnalyzer.PredictSentiments(mlContext, model);
                sentimentAnalyzer.PrintPrediction(predictions);

                if (BetterInput.GetKeyNumber("\r\nPredict more? 1-Yes, 0-No", 0, 1) == 0) break;
            }
            break;

        case FunctionType.Clustering:
            ClusteringAnalyzer clusteringAnalyzer = new();

            model = CreateClusteringModel(mlContext, clusteringAnalyzer);

            var predictor = mlContext.Model.CreatePredictionEngine<IrisData, ClusterPrediction>(model);

            while (true) {
                var prediction = clusteringAnalyzer.PredictCluster(predictor);
                clusteringAnalyzer.PrintPrediction(prediction);

                if (BetterInput.GetKeyNumber("\r\nPredict more? 1-Yes, 0-No", 0, 1) == 0) break;
            }
            break;

        default:
            break;
    };
}

static ITransformer CreateSentimentModel(MLContext mlContext, SentimentAnalyzer sentimentAnalyzer) {
    SentimentType sentimentType = (SentimentType)BetterInput.GetKeyNumber("1. Amazon Cells\r\n2. IMDB\r\n3. Yelp", 1, 3) - 1;
    string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "Sentiment", $"{sentimentType}.txt");

    return sentimentAnalyzer.BuildBinaryAnalyzerModel(mlContext, dataPath);
}

static ITransformer CreateClusteringModel(MLContext mlContext, ClusteringAnalyzer clusteringAnalyzer) {
    string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "Clustering", "iris.data");
    string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Clustering", "IrisClusteringModel.zip");

    return clusteringAnalyzer.TrainModel(mlContext, dataPath, modelPath);
}