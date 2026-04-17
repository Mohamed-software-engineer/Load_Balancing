using Models;

namespace Services
{
    public class MetricsService
    {
        private int _totalRequests;
        private int _successfulRequests;
        private int _failedRequests;
        private readonly object _lock = new();
        private readonly Dictionary<string, int> _requestsPerAlgorithm = new(StringComparer.OrdinalIgnoreCase);

        public void IncrementTotalRequest(string algorithm)
        {
            lock (_lock)
            {
                _totalRequests++;

                if (_requestsPerAlgorithm.ContainsKey(algorithm))
                    _requestsPerAlgorithm[algorithm]++;
                else
                    _requestsPerAlgorithm[algorithm] = 1;
            }
        }

        public void IncrementSuccess()
        {
            lock (_lock)
            {
                _successfulRequests++;
            }
        }

        public void IncrementFailure()
        {
            lock (_lock)
            {
                _failedRequests++;
            }
        }

        public void Reset(List<NodeInfo> nodes)
        {
            lock (_lock)
            {
                _totalRequests = 0;
                _successfulRequests = 0;
                _failedRequests = 0;
                _requestsPerAlgorithm.Clear();

                foreach (var node in nodes)
                {
                    node.ActiveConnections = 0;
                    node.TotalRequests = 0;
                    node.FailedRequests = 0;
                }
            }
        }

        public MetricsResponse GetMetrics(List<NodeInfo> nodes)
        {
            lock (_lock)
            {
                return new MetricsResponse
                {
                    TotalRequests = _totalRequests,
                    SuccessfulRequests = _successfulRequests,
                    FailedRequests = _failedRequests,
                    RequestsPerAlgorithm = new Dictionary<string, int>(_requestsPerAlgorithm),
                    Nodes = nodes.Select(n => new NodeMetricsResponse
                    {
                        NodeId = n.NodeId,
                        NodeName = n.NodeName,
                        Weight = n.Weight,
                        ActiveConnections = n.ActiveConnections,
                        TotalRequests = n.TotalRequests,
                        FailedRequests = n.FailedRequests,
                        IsHealthy = n.IsHealthy
                    }).ToList()
                };
            }
        }
    }
}