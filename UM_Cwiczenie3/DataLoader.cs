using System.Data;
using System.Reflection.PortableExecutable;

namespace UM_Cwiczenie3;
internal class DataLoader
{
    public static TrainTestData LoadData<T>(MLContext mlContext, string dataPath, bool hasHeader, char separatorChar, double testFraction = 0.2d)
    {
        IDataView dataView = mlContext.Data.LoadFromTextFile<T>(dataPath, hasHeader: hasHeader, separatorChar: separatorChar);
        TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: testFraction);
        return splitDataView;
    }

    public static DataTable ReadData(string path, string delimeter = ",", bool hasHeader = true)
    {

        DataTable dt = new();
        using StreamReader sr = new(path);
        if (sr.Peek() == -1)
            throw new Exception("OutOfBoundsExeption.");

        string[] headers = sr.ReadLine()?.Split(delimeter) ?? Array.Empty<string>();

        if (hasHeader)
        {
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
        }
        else
        {
            for(int i=0; i < headers.Length; i++)
            {
                dt.Columns.Add($"Col{i+1}");
            }
            DataRow dr = dt.NewRow();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dr[i] = headers[i];
            }
            dt.Rows.Add(dr);
        }

        while (!sr.EndOfStream)
        {
            string[] rows = sr.ReadLine()?.Split(delimeter) ?? Array.Empty<string>();
            DataRow dr = dt.NewRow();
            for (int i = 0; i < headers.Length; i++)
            {
                dr[i] = rows[i];
            }
            dt.Rows.Add(dr);
        }

        return dt;
    }
}
