namespace UM_Cwiczenie3.ML.DbScan.Model;
internal class DbScanDataPoint {
    [LoadColumn(0, 1)]
    [VectorType(2)]
    public float[] Features { get; set; }

    [LoadColumn(2)]
    [KeyType(count: 11)]
    public uint Label { get; set; }
}
