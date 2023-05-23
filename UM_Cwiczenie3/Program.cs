// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json.Linq;
using System.Data;
using System.Drawing;

Console.WriteLine("Hello, World!");
Console.WriteLine("Select classification type:");

MLContext mlContext = new();

while (true)
{
    FunctionType function = (FunctionType)BetterInput.GetKeyNumber("1. Binary Classification\r\n2. Clustering\r\n3. DBSCAN", 1, 3);

    switch (function)
    {
        case FunctionType.Sentiment:
            SentimentAnalyzer sentimentAnalyzer = new();
            ITransformer model = CreateSentimentModel(mlContext, sentimentAnalyzer);

            while (true)
            {
                IEnumerable<SentimentPrediction> predictions = sentimentAnalyzer.PredictSentiments(mlContext, model);
                sentimentAnalyzer.PrintPrediction(predictions);

                if (BetterInput.GetKeyNumber("\r\nPredict more? 1-Yes, 0-No", 0, 1) == 0) break;
            }
            break;

        case FunctionType.Clustering:
            ClusteringAnalyzer clusteringAnalyzer = new();

            model = CreateClusteringModel(mlContext, clusteringAnalyzer);

            var predictor = mlContext.Model.CreatePredictionEngine<IrisData, ClusterPrediction>(model);

            while (true)
            {
                var prediction = clusteringAnalyzer.PredictCluster(predictor);
                clusteringAnalyzer.PrintPrediction(prediction);

                if (BetterInput.GetKeyNumber("\r\nPredict more? 1-Yes, 0-No", 0, 1) == 0) break;
            }
            break;

        case FunctionType.DBSCAN:
            DbScanAnalyzer dbScanAnalyzer = new();
            var data=MapToDbScanDataPoint(DataLoader.ReadData(Path.Combine(Environment.CurrentDirectory, "Data", "DbScan", "DBSCAN_MOCK_DATA.csv"), ",", false));
            while (true)
            {
                double eps = BetterInput.GetDoubleSameLine("Epsilon value", 1, 500, false);
                int minPoints = BetterInput.GetInputNumberSameLine("Number of minimum points", 1, 500, false);
                List<DbScanDataPoint> points=new List<DbScanDataPoint>();
                foreach(DbScanDataPoint p in data)
                {
                    points.Add(new DbScanDataPoint() { Features = (double[])p.Features.Clone() });
                }

                var clusters= dbScanAnalyzer.GetClusters(points, eps, minPoints);
                dbScanAnalyzer.Print(clusters, points.Count);

                if (BetterInput.GetKeyNumber("\r\nPredict more? 1-Yes, 0-No", 0, 1) == 0) break;
            }
            break;
        default:
            break;
    };
}

static IEnumerable<DbScanDataPoint> MapToDbScanDataPoint(DataTable table)
{
    List<DbScanDataPoint> entities = new();
    foreach (DataRow row in table.Rows)
    {
        var point = new DbScanDataPoint();
        List<double> features = new List<double>();
        for(int i = 0; i < table.Columns.Count; i++)
        {
            features.Add(Convert.ToDouble(row[i].ToString().Replace(".", ",")));
        }
        point.Features = features.ToArray();
        entities.Add(point);
    }
    return entities;
}

static ITransformer CreateSentimentModel(MLContext mlContext, SentimentAnalyzer sentimentAnalyzer)
{
    SentimentType sentimentType = (SentimentType)BetterInput.GetKeyNumber("1. Amazon Cells\r\n2. IMDB\r\n3. Yelp", 1, 3) - 1;
    string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "Sentiment", $"{sentimentType}.txt");

    return sentimentAnalyzer.BuildModel(mlContext, dataPath);
}

static ITransformer CreateClusteringModel(MLContext mlContext, ClusteringAnalyzer clusteringAnalyzer)
{
    string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "Clustering", "iris.data");
    string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Clustering", "IrisClusteringModel.zip");

    return clusteringAnalyzer.TrainModel(mlContext, dataPath, modelPath);
}