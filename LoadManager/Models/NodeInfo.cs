namespace Models
{
    public class NodeInfo
    {
        public string NodeId { get; set; } = string.Empty;
        public string NodeName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Weight { get; set; } = 1;
        public int ActiveConnections { get; set; } = 0;
        public int TotalRequests { get; set; } = 0;
        public int FailedRequests { get; set; } = 0;
        public bool IsHealthy { get; set; } = true;
    }
}