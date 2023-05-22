using UM_Cwiczenie3.ML.Clustering.Model;

namespace UM_Cwiczenie3.ML.Clustering {
    internal class ClusteringAnalyzer {
        public ITransformer TrainModel(MLContext mlContext, string dataPath, string modelPath) {
            IDataView dataView = mlContext.Data.LoadFromTextFile<IrisData>(dataPath, hasHeader: false, separatorChar: ',');
            string featuresColumnName = "Features";
            var pipeline = mlContext.Transforms
                .Concatenate(featuresColumnName, "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")
                .Append(mlContext.Clustering.Trainers.KMeans(featuresColumnName, numberOfClusters: 3));

            var model = pipeline.Fit(dataView);

            using (FileStream fileStream = new(modelPath, FileMode.Create, FileAccess.Write, FileShare.Write)) {
                mlContext.Model.Save(model, dataView.Schema, fileStream);
            }

            var clusters = CalculateClusters(mlContext, model.Transform(dataView));
            PrintClustersDetails(clusters);

            return model;
        }

        public Dictionary<int, IEnumerable<ClusterDetails>> CalculateClusters(MLContext mlContext, IDataView transformedData) {
            var clusterAssignments = mlContext.Data.CreateEnumerable<ClusterPrediction>(transformedData, reuseRowObject: false).ToArray();
            var originalData = mlContext.Data
                .CreateEnumerable<IrisData>(transformedData, reuseRowObject: false)
                .ToArray();

            var clusterDetails = clusterAssignments.Zip(originalData, (prediction, data) => new ClusterDetails() {
                SepalLength = data.SepalLength,
                SepalWidth = data.SepalWidth,
                PetalLength = data.PetalLength,
                PetalWidth = data.PetalWidth,
                PredictedClusterId = prediction.PredictedClusterId,
                Distances = prediction.Distances
            });

            var groupedData = clusterDetails
                .GroupBy(detail => detail.PredictedClusterId)
                .ToDictionary(g => (int)g.Key, g => g.Select(a => new ClusterDetails {
                    SepalWidth = a.PetalWidth,
                    SepalLength = a.SepalLength,
                    PetalWidth = a.PetalWidth,
                    PetalLength = a.PetalLength,
                    Distances = a.Distances,
                    PredictedClusterId = a.PredictedClusterId,
                }));

            return groupedData;
        }

        public ClusterPrediction PredictCluster(PredictionEngine<IrisData, ClusterPrediction> predictor) {
            IrisData iris = new() {
                SepalLength = BetterInput.GetFloatSameLine("Sepal length", 0.1f, 100f, false),
                SepalWidth = BetterInput.GetFloatSameLine("Sepal width", 0.1f, 100f, false),
                PetalLength = BetterInput.GetFloatSameLine("Petal length", 0.1f, 100f, false),
                PetalWidth = BetterInput.GetFloatSameLine("Petal length", 0.1f, 100f, false)
            };

            var prediction = predictor.Predict(iris);
            return prediction;
        }

        private void PrintClustersDetails(Dictionary<int, IEnumerable<ClusterDetails>> groupedData) {
            foreach (var group in groupedData.OrderBy(g => g.Key)) {
                Console.WriteLine($"Cluster ID: {group.Key}");

                Console.WriteLine($"Data point: Sepal Length = {group.Value.Average(g => g.SepalLength):0.00}, " +
                                  $"Sepal Width = {group.Value.Average(g => g.SepalWidth):0.00}, " +
                                  $"Petal Length = {group.Value.Average(g => g.PetalLength):0.00}, " +
                                  $"Petal Width = {group.Value.Average(g => g.PetalWidth):0.00}");

                Console.WriteLine($"Distances: {group.Value.Average(g => g.Distances?.Average() ?? 0f):0.00}");
                Console.WriteLine();
            }
        }

        public void PrintPrediction(ClusterPrediction prediction) {
            Console.WriteLine($"Cluster: {prediction.PredictedClusterId}");
            Console.WriteLine($"Distances: {string.Join(" ", prediction.Distances ?? Array.Empty<float>()):P2}");
        }
    }
}
