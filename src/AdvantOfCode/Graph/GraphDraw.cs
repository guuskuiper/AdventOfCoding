using System.Drawing;
using System.Text;

namespace AdventOfCode.Graph;

public static class GraphDraw
{
    public static void DrawGrid(IRectGrid<Point> g)
    {
        DrawGridStyles(g, new Default<Point>());
    }
    
    public static void DrawGrid(IRectGrid<Point> g, Dictionary<Point, Point> parents, Point start, Point goal) 
    {
        DrawGridStyles(g, new PathArrowStyle(g, parents, start, goal));
    }
    
    public static void DrawGrid(IRectGrid<Point> g, Dictionary<Point, Point> parents, IEnumerable<Point> path, Point start, Point goal)
    {
        IStyle<Point> style = new PathStyleDecorator(path, new PathArrowStyle(g, parents, start, goal));
        DrawGridStyles(g, style);
    }

    public static void DrawDistanceGrid(IRectGrid<Point> g, Dictionary<Point, double> distances)
    {
        IStyle<Point> style = new NumberStyleDecorator(distances, new Default<Point>());
        DrawGridStyles(g, style);
    }

    private static void DrawGridStyles(IRectGrid<Point> g, IStyle<Point> style)
    {
        StringBuilder sb = new();
        foreach (var y in Enumerable.Range(0, g.Height))
        {
            foreach (var x in Enumerable.Range(0, g.Width))
            {
                sb.Append(style.Format(new Point(x, y)));
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }
}
