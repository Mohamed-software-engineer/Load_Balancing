using Models;

namespace Services
{
    public class NodeRegistryService
    {
        private readonly List<NodeInfo> _nodes =
        [
            new NodeInfo
            {
                NodeId = "node1",
                NodeName = "Node Server 1",
                Url = "http://node1:8080",
                Weight = 5,
                IsHealthy = true
            },
            new NodeInfo
            {
                NodeId = "node2",
                NodeName = "Node Server 2",
                Url = "http://node2:8080",
                Weight = 1,
                IsHealthy = true
            },
            new NodeInfo
            {
                NodeId = "node3",
                NodeName = "Node Server 3",
                Url = "http://node3:8080",
                Weight = 4,
                IsHealthy = true
            },
            new NodeInfo
            {
                NodeId = "node4",
                NodeName = "Node Server 4",
                Url = "http://node4:8080",
                Weight = 2,
                IsHealthy = true
            },
            new NodeInfo
            {
                NodeId = "node5",
                NodeName = "Node Server 5",
                Url = "http://node5:8080",
                Weight = 3,
                IsHealthy = true
            }
        ];

        public List<NodeInfo> GetNodes() => _nodes;
    }
}