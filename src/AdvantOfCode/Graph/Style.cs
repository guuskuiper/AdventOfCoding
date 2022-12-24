using System.Drawing;

namespace AdventOfCode.Graph;

public interface IStyle<in T>
{
    string Format(T p);
}

public class Default<T> : IStyle<T>
{
    public string Format(T p) => ".";
}

public class Const<T> : IStyle<T>
{
    private readonly string _constant;

    public Const(string constant)
    {
        _constant = constant;
    }

    public string Format(T p) => _constant;
}

public class PathArrowReverseStyle : IStyle<Point>
{
    private readonly IRectGrid<Point> _grid;
    private readonly Dictionary<Point, Point> _parents;
    private readonly Point _start;
    private readonly Point _goal;
    private readonly IStyle<Point> _baseStyle;

    public PathArrowReverseStyle(IRectGrid<Point> grid, Dictionary<Point, Point> parents, Point start, Point goal)
    {
        _grid = grid;
        _parents = parents;
        _start = start;
        _goal = goal;
        _baseStyle = new Default<Point>();
    }

    public string Format(Point p)
    {
        string returnable;
        if(p == _goal) returnable = "G";
        else if(p == _start) returnable = "A";
        else if(_parents.TryGetValue(p, out Point p2) && p2 != p)
        {
            if (p2.X == p.X + 1) returnable = ">";
            else if (p2.X == p.X - 1) returnable = "<";
            else if (p2.Y == p.Y + 1) returnable = "v";
            else if (p2.Y == p.Y - 1) returnable = "^";
            else returnable = _baseStyle.Format(p);
        }
        else
        {
            returnable = _baseStyle.Format(p);
        }
        
        return returnable;
    }
}


public class PathArrowStyle : IStyle<Point>
{
    private readonly IRectGrid<Point> _grid;
    private readonly Dictionary<Point, Point> _parents;
    private readonly Point _start;
    private readonly Point _goal;
    private readonly IStyle<Point> _baseStyle;

    public PathArrowStyle(IRectGrid<Point> grid, Dictionary<Point, Point> parents, Point start, Point goal, IStyle<Point> baseStyle)
    {
        _grid = grid;
        _parents = parents;
        _start = start;
        _goal = goal;
        _baseStyle = baseStyle;
    }

    public PathArrowStyle(IRectGrid<Point> grid, Dictionary<Point, Point> parents, Point start, Point goal) : 
        this(grid, parents, start, goal, new Default<Point>()) {}

    public string Format(Point p)
    {
        string returnable;
        if(p == _goal) returnable = "G";
        else if(p == _start) returnable = "A";
        else if(_parents.TryGetValue(p, out Point p2) && p2 != p)
        {
            if (p.X == p2.X + 1) returnable = ">";
            else if (p.X == p2.X - 1) returnable = "<";
            else if (p.Y == p2.Y + 1) returnable = "v";
            else if (p.Y == p2.Y - 1) returnable = "^";
            else returnable = _baseStyle.Format(p);
        }
        else
        {
            returnable = _baseStyle.Format(p);
        }
        
        return returnable;
    }
}

public class NumberStyleDecorator : IStyle<Point>
{
    private readonly IStyle<Point> _decorated;
    private readonly Dictionary<Point, double> _costs;

    public NumberStyleDecorator(Dictionary<Point, double> costs, IStyle<Point> decorated)
    {
        _decorated = decorated;
        _costs = costs;
    }

    public string Format(Point p) => _costs.TryGetValue(p, out double cost) ? ((int)cost).ToString("D1") : _decorated.Format(p);
}

public class PathStyleDecorator : IStyle<Point>
{
    private readonly IStyle<Point> _decorated;
    private readonly HashSet<Point> _path;

    public PathStyleDecorator(IEnumerable<Point> path, IStyle<Point> decorated)
    {
        _decorated = decorated;
        _path = new HashSet<Point>(path);
    }

    public string Format(Point p)
    {
        return _path.Contains(p) ? "@" : _decorated.Format(p);
    }
}