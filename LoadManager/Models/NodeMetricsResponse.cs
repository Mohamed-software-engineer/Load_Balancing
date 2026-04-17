namespace Models
{
    public class NodeMetricsResponse
    {
        public string NodeId { get; set; } = string.Empty;
        public string NodeName { get; set; } = string.Empty;
        public int Weight { get; set; }
        public int ActiveConnections { get; set; }
        public int TotalRequests { get; set; }
        public int FailedRequests { get; set; }
        public bool IsHealthy { get; set; }
    }
}