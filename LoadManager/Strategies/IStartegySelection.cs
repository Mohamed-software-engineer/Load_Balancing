using Models;

namespace Strategies
{
    public interface IStrategySelection
    {
        public NodeInfo SelectNode(List<NodeInfo> nodes);
    }
}