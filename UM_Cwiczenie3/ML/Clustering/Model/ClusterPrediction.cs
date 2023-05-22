namespace UM_Cwiczenie3.ML.Clustering.Model {
    internal class ClusterPrediction {
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;

        [ColumnName("Score")]
        public float[]? Distances;
    }
}
