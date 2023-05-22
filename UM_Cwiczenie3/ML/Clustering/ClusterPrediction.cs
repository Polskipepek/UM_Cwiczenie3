namespace UM_Cwiczenie3.ML.Clustering {
    internal class ClusterPrediction {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;

        [ColumnName("Score")]
        public float[]? Distances;
    }
}
