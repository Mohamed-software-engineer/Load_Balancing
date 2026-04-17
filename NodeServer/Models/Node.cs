namespace Models
{
    public class Node
    {
        public string NodeId { get; set; } = string.Empty;
        public string NodeName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}