namespace UM_Cwiczenie3.ML {
    internal abstract class AnalyzerBase {
        public abstract ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainSet);
        public abstract void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet);
    }
}
