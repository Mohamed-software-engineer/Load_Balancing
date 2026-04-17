using Models;

namespace Strategies
{
    public class HashSelection : IStrategySelection
    {
        private int _requestCounter = 0;

        public NodeInfo SelectNode(List<NodeInfo> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                throw new ArgumentException("Node list cannot be null or empty.");

            var healthyNodes = nodes.Where(n => n.IsHealthy).ToList();
            if (healthyNodes.Count == 0)
                throw new InvalidOperationException("No healthy nodes available.");

            var key = Interlocked.Increment(ref _requestCounter).ToString(); // 0 --> number of requests
            var hash = Math.Abs(key.GetHashCode());
            var index = hash % healthyNodes.Count;
            return healthyNodes[index];
        }
    }
}