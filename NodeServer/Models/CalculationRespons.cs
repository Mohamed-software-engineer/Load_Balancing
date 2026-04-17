namespace Models
{
    public class CalculationResponse
    {
        public string NodeId { get; set; } = string.Empty;
        public string NodeName { get; set; } = string.Empty;

        public int N { get; set; }
        public long Limit { get; set; }

        public double Result { get; set; }
        public long ProcessingTimeMs { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}