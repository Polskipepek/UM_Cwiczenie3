namespace UM_Cwiczenie3.ML.DbScan;
internal class DbScanAnalyzer {
    public ITransformer BuildModel(MLContext mlContext, IDataView dataView) {

        var pipeline = mlContext.Transforms.Concatenate("Features", "Features")
            .Append(mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("Label"))
            .Append(mlContext.Clustering.Trainers.KMeans(numberOfClusters: 1))
            .Append(mlContext.Transforms.NormalizeMinMax("Features")); // Re-normalize features after clustering

        var model = pipeline.Fit(dataView);
        return model;
    }

    public IEnumerable<DbScanDataPoint> GetClusters(MLContext mlContext, ITransformer model, IDataView dataView) {
        var transformedData = model.Transform(dataView);
        var predictions = mlContext.Data.CreateEnumerable<DbScanDataPoint>(transformedData, reuseRowObject: false).ToArray();
        return predictions;
    }

    public void PrintPredictions(IEnumerable<DbScanDataPoint> predictions) {
        var clusters = predictions
            .GroupBy(p => p.Label)
            .ToDictionary(g => g.Key, g => new { a = g.Average(a => a.Features[0]), b = g.Average(a => a.Features[1]) });

        foreach (var cluster in clusters) {
            Console.WriteLine($"Average Data point ({cluster.Value.a}, {cluster.Value.b}) belongs to cluster {cluster.Key}");
        }
        Console.WriteLine();
    }
}
