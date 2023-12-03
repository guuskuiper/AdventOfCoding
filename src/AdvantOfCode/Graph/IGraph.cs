using System.Numerics;
using System.Drawing;

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

public interface IWeightedGraph<TNode, out TCosts> : IGraph<TNode>
    where TCosts : INumber<TCosts>
{
    TCosts Cost(TNode a, TNode b);
}

public interface IValueGrid<out TValue>
{
    TValue this[Point p] { get; }
}