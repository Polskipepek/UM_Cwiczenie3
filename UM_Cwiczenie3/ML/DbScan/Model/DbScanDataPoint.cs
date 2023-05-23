namespace UM_Cwiczenie3.ML.DbScan.Model;
internal class DbScanDataPoint {
    public double[] Features { get; set; }
    public int ClusterId;

    public override string ToString()
    {
        string result = "[";
        foreach (double value in Features) result += $"{value}; ";
        if(result.Length>2) result=result.Substring(0,result.Length-2);
        return result + "]";
    }
}
