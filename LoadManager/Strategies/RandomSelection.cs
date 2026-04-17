using Models;

namespace Strategies
{
    public class RandomSelection : IStrategySelection
    {
        public NodeInfo SelectNode(List<NodeInfo> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                throw new ArgumentException("Node list cannot be null or empty.");

            var healthyNodes = nodes.Where(n => n.IsHealthy).ToList();
            if (healthyNodes.Count == 0)
                throw new InvalidOperationException("No healthy nodes available.");

            var index = Random.Shared.Next(healthyNodes.Count);
            return healthyNodes[index];
        }
    }
}