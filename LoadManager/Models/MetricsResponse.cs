namespace Models
{
    public class MetricsResponse
    {
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public Dictionary<string, int> RequestsPerAlgorithm { get; set; } = new();
        public List<NodeMetricsResponse> Nodes { get; set; } = new();
    }
}