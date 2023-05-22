namespace UM_Cwiczenie3.ML.Sentiments.Model;

public class SentimentPrediction : SentimentData {

    [ColumnName("PredictedLabel")]
    public bool Prediction { get; set; }

    public float Probability { get; set; }

    public float Score { get; set; }
}