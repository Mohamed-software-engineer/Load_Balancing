using Models;

namespace Strategies
{
    public class LeastConnections : IStrategySelection
    {
        public NodeInfo SelectNode(List<NodeInfo> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                throw new ArgumentException("Node list cannot be null or empty.");

            var healthyNodes = nodes.Where(n => n.IsHealthy).ToList();
            if (healthyNodes.Count == 0)
                throw new InvalidOperationException("No healthy nodes available.");

            return healthyNodes
                .OrderBy(n => n.ActiveConnections)
                .ThenBy(n => n.TotalRequests)
                .First();
        }
    }
}