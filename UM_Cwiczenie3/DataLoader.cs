namespace UM_Cwiczenie3;
internal class DataLoader {
    public static TrainTestData LoadData<T>(MLContext mlContext, string dataPath, bool hasHeader, char separatorChar, double testFraction = 0.2d) {
        IDataView dataView = mlContext.Data.LoadFromTextFile<T>(dataPath, hasHeader: hasHeader, separatorChar: separatorChar);
        TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: testFraction);
        return splitDataView;
    }
}
