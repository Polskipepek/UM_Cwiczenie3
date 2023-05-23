using System.Drawing;
using System.Linq;
using System.Reflection;

namespace UM_Cwiczenie3.ML.DbScan;
internal class DbScanAnalyzer {
    const int _noise = -1;
    const int _unclassified = 0;

    public List<List<DbScanDataPoint>> GetClusters(List<DbScanDataPoint> points, double eps, int minPoints)
    {
        if (points == null || points.Count==0) return null;
        List<List<DbScanDataPoint>> clusters = new List<List<DbScanDataPoint>>();

        eps = Math.Pow(eps, 2);
        int clusterId = 1;
        int maxClusterId = 0;

        foreach(DbScanDataPoint p in points)
        {
            if (p.ClusterId == _unclassified)
            {
                if (ExpandCluster(points, p, clusterId, eps, minPoints)) clusterId++;
            }

            if(p.ClusterId>maxClusterId) maxClusterId = clusterId;
        }

        if (maxClusterId> 0)
        {
            for (int i = 0; i < maxClusterId; i++)
                clusters.Add(new List<DbScanDataPoint>());

            foreach (DbScanDataPoint p in points)
            {
                if (p.ClusterId > 0) clusters[p.ClusterId - 1].Add(p);
            }
        }

        return clusters;
    }
    public List<DbScanDataPoint> GetRegion(List<DbScanDataPoint> points, DbScanDataPoint p, double eps)
    {
        List<DbScanDataPoint> region = new List<DbScanDataPoint>();
        for (int i = 0; i < points.Count; i++)
        {
            double distance = Distance(p, points[i]);
            if (distance <= eps) region.Add(points[i]);
        }
        return region;
    }

    public double Distance(DbScanDataPoint p1, DbScanDataPoint p2)
    {
        double result = 0;
        for (int i = 0; i < p1.Features.Length; i++)
        {
            result += Math.Pow(p1.Features[i] - p2.Features[i], 2);
        }

        return Math.Sqrt(result);
    }

    public void Print(List<List<DbScanDataPoint>> clusters, int length)
    {
        if(clusters!=null && clusters.Count > 0)
        {
            int total = 0;
            //Print clusters
            for (int i = 0; i < clusters.Count; i++)
            {
                Console.WriteLine($"\nCluster {i+1} has got {clusters[i].Count} points:");
                foreach (DbScanDataPoint p in clusters[i])
                {
                    Console.Write($"{p} ");
                }
                Console.WriteLine();
                total += clusters[i].Count;
            }

            //Print noises
            int noises = length - total;
            if (noises>0)
            {
                Console.WriteLine($"\nThere are {noises} noises.");
            }
        }
        else
        {
            Console.WriteLine("Algorithm couldn't find any cluster");
        }
    }

    public bool ExpandCluster(List<DbScanDataPoint> points, DbScanDataPoint p, int clusterId, double eps, int minPoints)
    {
        List<DbScanDataPoint> region = GetRegion(points, p, eps);
        if (region.Count < minPoints)
        {
            p.ClusterId = _noise;
            return false;
        }
        else
        {
            foreach(DbScanDataPoint reg in region)
            {
                reg.ClusterId = clusterId;
            }

            region.Remove(p);

            while (region.Count > 0)
            {
                DbScanDataPoint currentP = region[0];
                List<DbScanDataPoint> result = GetRegion(points, currentP, eps);
                if (result.Count >= minPoints)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        DbScanDataPoint resultP = result[i];
                        if (resultP.ClusterId == _unclassified || resultP.ClusterId == _noise)
                        {
                            if (resultP.ClusterId == _unclassified) region.Add(resultP);
                            resultP.ClusterId = clusterId;
                        }
                    }
                }
                region.Remove(currentP);
            }
            return true;
        }
    }
}
