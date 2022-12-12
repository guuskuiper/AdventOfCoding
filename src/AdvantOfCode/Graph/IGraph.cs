namespace AdventOfCode.Graph;

public interface IGraph<TNode>
{
    IEnumerable<TNode> Neighbors(TNode node);
}

public interface IRectGrid<TNode> : IGraph<TNode>
{
    int Width { get; }
    int Height { get; }
}

public interface IWeightedGraph<TNode> : IGraph<TNode>
{
    double Cost(TNode a, TNode b);
}