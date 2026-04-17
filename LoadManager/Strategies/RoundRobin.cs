using Models;

namespace Strategies
{
    public class RoundRobin : IStrategySelection
    {
        private int _currentIndex = -1;
        private readonly object _lock = new();

        public NodeInfo SelectNode(List<NodeInfo> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                throw new ArgumentException("Node list cannot be null or empty.");

            var healthyNodes = nodes.Where(n => n.IsHealthy).ToList();
            if (healthyNodes.Count == 0)
                throw new InvalidOperationException("No healthy nodes available.");

            lock (_lock)
            {
                _currentIndex = (_currentIndex + 1) % healthyNodes.Count;
                return healthyNodes[_currentIndex];
            }
        }
    }
}