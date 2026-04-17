namespace Models
{
    public class HealthResponse
    {
        public string NodeId { get; set; } = string.Empty;
        public string NodeName { get; set; } = string.Empty;
        public string Status { get; set; } = "Healthy";
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }
}