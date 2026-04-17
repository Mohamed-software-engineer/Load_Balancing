using LoadManager.Models;
using Models;
using Strategies;

namespace Services
{
    public class LoadBalancerService
    {
        private readonly NodeRegistryService _nodeRegistryService;
        private readonly MetricsService _metricsService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Dictionary<string, IStrategySelection> _strategies;

        public LoadBalancerService(
            NodeRegistryService nodeRegistryService,
            MetricsService metricsService,
            IHttpClientFactory httpClientFactory)
        {
            _nodeRegistryService = nodeRegistryService;
            _metricsService = metricsService;
            _httpClientFactory = httpClientFactory;

            _strategies = new Dictionary<string, IStrategySelection>(StringComparer.OrdinalIgnoreCase)
            {
                ["rr"] = new RoundRobin(),
                ["random"] = new RandomSelection(),
                ["wrr"] = new WeightedRoundRobin(),
                ["lc"] = new LeastConnections(),
                ["hash"] = new HashSelection()
            };
        }
        public async Task<LoadBalancerResponse> ForwardRequestAsync(int n, string algorithm)
        {
            if (!_strategies.TryGetValue(algorithm, out var strategy))
                throw new ArgumentException($"Unsupported algorithm: {algorithm}");

            var nodes = _nodeRegistryService.GetNodes();
            _metricsService.IncrementTotalRequest(algorithm);

            var selectedNode = strategy.SelectNode(nodes);
            selectedNode.ActiveConnections++;

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{selectedNode.Url}/cal?n={n}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                     selectedNode.TotalRequests++;
                    _metricsService.IncrementSuccess();
                }
                else
                {
                    selectedNode.FailedRequests++;
                    _metricsService.IncrementFailure();
                }

                return new LoadBalancerResponse
                {
                    Algorithm = algorithm,
                    SelectedNodeId = selectedNode.NodeId,
                    SelectedNodeName = selectedNode.NodeName,
                    SelectedNodeUrl = selectedNode.Url,
                    N = n,
                    StatusCode = (int)response.StatusCode,
                    BackendResponse = content
                };
            }
            catch
            {
                 selectedNode.FailedRequests++;
                _metricsService.IncrementFailure();
                throw;
            }
            finally
            {
                selectedNode.ActiveConnections--;
            }
        }
    }
}