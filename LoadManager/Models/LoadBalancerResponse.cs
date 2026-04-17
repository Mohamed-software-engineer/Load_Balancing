namespace LoadManager.Models
{
    public class LoadBalancerResponse
    {
        public string Algorithm { get; set; } = string.Empty;
        public string SelectedNodeId { get; set; } = string.Empty;
        public string SelectedNodeName { get; set; } = string.Empty;
        public string SelectedNodeUrl { get; set; } = string.Empty;
        public int N { get; set; }
        public int StatusCode { get; set; }
        public string BackendResponse { get; set; } = string.Empty;
    }
}