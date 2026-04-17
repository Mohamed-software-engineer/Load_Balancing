using System.Diagnostics;

namespace NodeServer.Services
{
    public class CalculateH
    {
        public (double Result, long Limit, long ProcessingTimeMs) Execute(int n)
        {
            if (n <= 0)
                throw new ArgumentException("n must be greater than 0.");

            double result = 0;
            long limit = n * 1_000_000L;

            var stopwatch = Stopwatch.StartNew();

            for (long i = 1; i <= limit; i++)
            {
                result += (Math.Sqrt(i) * Math.Sin(i)) / Math.Log(i + 1);
            }

            stopwatch.Stop();

            return (result, limit, stopwatch.ElapsedMilliseconds);
        }
    }
}