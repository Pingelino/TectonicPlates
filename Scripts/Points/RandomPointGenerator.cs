using System;

public static class RandomPointGenerator
{
    public static Point[] GenerateRandomPoints(int amount, int seed)
    {
        Point[] points = new Point[amount];
        Random random = new Random(seed);

        for (int i = 0; i < amount; i++)
        {
            double x = NormalDistribution(random.NextDouble(), random.NextDouble());
            double y = NormalDistribution(random.NextDouble(), random.NextDouble());
            double z = NormalDistribution(random.NextDouble(), random.NextDouble());
            points[i] = new Point(x, y, z).normalized;
        }
        return points;
    }

    public static double NormalDistribution(double a, double b)
    {
        double theta = Math.PI * 2.0 * a;
        double rho = Math.Sqrt(-2.0 * Math.Log(b));
        return rho * Math.Cos(theta);
    }
}