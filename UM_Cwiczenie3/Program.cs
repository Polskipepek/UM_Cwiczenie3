// See https://aka.ms/new-console-template for more information

using System.Data;
using UM3.ML.Clustering;
using UM3.ML.Sentiments;

Console.WriteLine("Hello, World!");
Console.WriteLine("Select classification type:");

MLContext mlContext = new();

while (true) {
    FunctionType function = (FunctionType)GetKeyNumber("1. Binary Classification\r\n2. Clustering", 1, 2);

    switch (function) {
        case FunctionType.Sentiment:
            SentimentAnalyzer sentimentAnalyzer = new();

            SentimentType type = (SentimentType)GetKeyNumber("1. Amazon Cells\r\n2. IMDB\r\n3. Yelp", 1, 3) - 1;
            string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "Sentiment", $"{type}.txt");

            ITransformer model = BuildBinaryAnalyzerModel(sentimentAnalyzer, dataPath);
            PredictSentiments(mlContext, sentimentAnalyzer, model);
            break;

        case FunctionType.Clustering:
            dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "Clustering", "iris.data");
            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Clustering", "IrisClusteringModel.zip");

            model = TrainModel(mlContext, dataPath, modelPath, out IDataView dataView);

            PrintClusters(model.Transform(dataView));
            PredictCluster(mlContext, model);
            break;

        default:
            break;
    };
}

void PrintClusters(IDataView transformedData) {

    var clusterAssignments = mlContext.Data.CreateEnumerable<ClusterPrediction>(transformedData, reuseRowObject: false).ToArray();
    var originalData = mlContext.Data.CreateEnumerable<IrisData>(transformedData, reuseRowObject: false)
        .ToArray();


    var clusterDetails = clusterAssignments.Zip(originalData, (prediction, data) => new {
        data.SepalLength,
        data.SepalWidth,
        data.PetalLength,
        data.PetalWidth,
        prediction.PredictedClusterId,
        prediction.Distances
    });

    var groupedData = clusterDetails.GroupBy(detail => detail.PredictedClusterId);

    foreach (var group in groupedData) {
        Console.WriteLine($"Cluster ID: {group.Key}");

        Console.WriteLine($"Data point: Sepal Length = {group.Average(g => g.SepalLength):0.00}, " +
                          $"Sepal Width = {group.Average(g => g.SepalWidth):0.00}, " +
                          $"Petal Length = {group.Average(g => g.PetalLength):0.00}, " +
                          $"Petal Width = {group.Average(g => g.PetalWidth):0.00}");

        Console.WriteLine($"Distances: {group.Average(g => g.Distances?.Average() ?? 0f):0.00}");
        Console.WriteLine();
    }
}

ITransformer BuildBinaryAnalyzerModel(SentimentAnalyzer sentimentAnalyzer, string dataPath) {
    TrainTestData splitDataView = DataLoader.LoadData<SentimentData>(mlContext, dataPath, hasHeader: false, separatorChar: '\t');
    ITransformer model = sentimentAnalyzer.BuildAndTrainModel(mlContext, splitDataView.TrainSet);
    sentimentAnalyzer.Evaluate(mlContext, model, splitDataView.TestSet);
    return model;
}

static int GetInputNumber(string title, int min, int max, bool clearAfter = true) {
    int numRow = -1;
    Console.WriteLine(title);
    while (!int.TryParse(Console.ReadLine(), out numRow) || numRow < min || numRow > max) {
        Console.Clear();
        Console.WriteLine("You entered an invalid number");
        Console.Write(title);
    }
    if (clearAfter) Console.Clear();
    return numRow;
}

static int GetKeyNumber(string title, int min, int max, bool clearAfter = true) {
    int numRow = -1;
    Console.WriteLine(title);
    while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out numRow) || numRow < min || numRow > max) {
        Console.Clear();
        Console.WriteLine("You entered an invalid number");
        Console.Write(title);
    }
    if (clearAfter) Console.Clear();
    return numRow;
}

static int GetKeyNumberSameLine(string title, int min, int max, bool clearAfter = true) {
    int numRow = -1;
    Console.Write($"{title}: ");
    while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out numRow) || numRow < min || numRow > max) {
        Console.Clear();
        Console.WriteLine("You entered an invalid number");
        Console.Write($"{title}: ");
    }
    if (clearAfter) Console.Clear();
    Console.WriteLine();
    return numRow;
}

static float GetFloat(string title, float min, float max, bool clearAfter = true) {
    float numRow = -1;
    Console.WriteLine(title);
    while (!float.TryParse(Console.ReadLine(), out numRow) || numRow < min || numRow > max) {
        Console.Clear();
        Console.WriteLine("You entered an invalid number");
        Console.Write(title);
    }
    if (clearAfter) Console.Clear();
    return numRow;
}
static float GetFloatSameLine(string title, float min, float max, bool clearAfter = true) {
    float numRow = -1;
    Console.Write($"{title}: ");
    while (!float.TryParse(Console.ReadLine(), out numRow) || numRow < min || numRow > max) {
        Console.Clear();
        Console.WriteLine("You entered an invalid number");
        Console.Write(title);
    }
    if (clearAfter) Console.Clear();
    return numRow;
}

static string GetReadLine(string title, int minLength = 1) {
    Console.Write($"{title}: ");
    string value = "";
    while (value?.Length < minLength) {
        value = Console.ReadLine();
    }
    return value!;
}

static void PredictCluster(MLContext mlContext, ITransformer model) {
    var predictor = mlContext.Model.CreatePredictionEngine<IrisData, ClusterPrediction>(model);

    while (true) {
        IrisData iris = new() {
            SepalLength = GetFloatSameLine("Sepal length", 0.1f, 100f, false),
            SepalWidth = GetFloatSameLine("Sepal width", 0.1f, 100f, false),
            PetalLength = GetFloatSameLine("Petal length", 0.1f, 100f, false),
            PetalWidth = GetFloatSameLine("Petal length", 0.1f, 100f, false)
        };

        var prediction = predictor.Predict(iris);
        Console.WriteLine($"Cluster: {prediction.PredictedClusterId}");
        Console.WriteLine($"Distances: {string.Join(" ", prediction.Distances ?? Array.Empty<float>()):P2}");

        if (GetKeyNumber("\r\nPredict more? 1-Yes, 0-No", 0, 1) == 0) break;
    }
}

static ITransformer TrainModel(MLContext mlContext, string dataPath, string modelPath, out IDataView dataView) {
    dataView = mlContext.Data.LoadFromTextFile<IrisData>(dataPath, hasHeader: false, separatorChar: ',');
    string featuresColumnName = "Features";
    var pipeline = mlContext.Transforms
        .Concatenate(featuresColumnName, "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")
        .Append(mlContext.Clustering.Trainers.KMeans(featuresColumnName, numberOfClusters: 3));

    var model = pipeline.Fit(dataView);

    using (FileStream fileStream = new(modelPath, FileMode.Create, FileAccess.Write, FileShare.Write)) {
        mlContext.Model.Save(model, dataView.Schema, fileStream);
    }
    return model;
}

void PredictSentiments(MLContext mlContext, SentimentAnalyzer sentimentAnalyzer, ITransformer model) {
    while (true) {
        int batchCount = GetKeyNumber("\r\nWrite Number of items to predict", 1, 10);
        List<SentimentData> sentiments = new(Enumerable.Range(0, batchCount).Select(x => new SentimentData { SentimentText = GetReadLine("Sentiment Text").ToString(), Sentiment = GetKeyNumberSameLine("Sentiment value [0-1]", 0, 1, false) == 1 }));
        sentimentAnalyzer.UseModelWithBatchItems(mlContext, model, sentiments);

        if (GetKeyNumber("\r\nPredict more? 1-Yes, 0-No", 0, 1) == 0) break;
    }
}