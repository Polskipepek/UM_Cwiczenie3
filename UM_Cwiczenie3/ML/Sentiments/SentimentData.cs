namespace UM3.ML.Sentiments;

public class SentimentData {
    [LoadColumn(0)]
    public string? SentimentText;

    [LoadColumn(1), ColumnName("Label")]
    public bool Sentiment;
}