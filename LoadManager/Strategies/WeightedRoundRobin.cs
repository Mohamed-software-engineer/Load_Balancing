using Models;

namespace Strategies
{
    public class WeightedRoundRobin : IStrategySelection
    {
        private int _currentIndex = -1;
        private readonly object _lock = new();

        public NodeInfo SelectNode(List<NodeInfo> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                throw new ArgumentException("Node list cannot be null or empty.");

            var healthyNodes = nodes.Where(n => n.IsHealthy && n.Weight > 0).ToList();
            if (healthyNodes.Count == 0)
                throw new InvalidOperationException("No healthy nodes available.");

            var weightedNodes = BuildWeightedList(healthyNodes);

            lock (_lock)
            {
                _currentIndex = (_currentIndex + 1) % weightedNodes.Count;
                return weightedNodes[_currentIndex];
            }
        }

        private static List<NodeInfo> BuildWeightedList(List<NodeInfo> nodes)
        {
            var weightedNodes = new List<NodeInfo>();

            foreach (var node in nodes)
            {
                for (int i = 0; i < node.Weight; i++)
                {
                    weightedNodes.Add(node);
                }
            }

            return weightedNodes;
        }
    }
}