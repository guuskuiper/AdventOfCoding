namespace AdventOfCode.Year2023.Day17;

[DayInfo(2023, 17)]
public class Solution17 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        int[,] grid = input.ToGrid(x => int.Parse([x]));
        CruciblePoint start = new CruciblePoint(new(0, 0), new(0, 0));
        Point end = new Point(grid.Width() - 1, grid.Heigth() - 1);

        double min;
        {
            CrucibleGrid crucibleGrid = new(grid);
            (Dictionary<CruciblePoint, CruciblePoint> path, Dictionary<CruciblePoint, double> costs) =
                Dijkstra.SearchGoal(crucibleGrid, start, point => point.P == end);
            var ends = costs.Keys.Where(p => p.P == end).ToArray();
            var endCosts = ends.Select(x => costs[x]).ToArray();
            min = endCosts.Min();
        }

        double minB;
        {
            UltraCrucibleGrid crucibleGrid = new(grid);
            (Dictionary<CruciblePoint, CruciblePoint> path, Dictionary<CruciblePoint, double> costs) =
                Dijkstra.SearchGoal(crucibleGrid, start, point => point.P == end);
            var ends = costs.Keys.Where(p => p.P == end).ToArray();
            var endCosts = ends.Select(x => costs[x]).ToArray();
            minB = endCosts.Min();
        }
        
        return min + "\n" + minB;
    }

    private record CruciblePoint(Point P, Size D);

    private class UltraCrucibleGrid : CrucibleGrid
    {
        public UltraCrucibleGrid(int[,] grid) : base(grid)
        {
        }
        
        public override IEnumerable<CruciblePoint> Neighbors(CruciblePoint node)
        {
            foreach (var neighbor in SquareGrid.SquareNeightbors)
            {
                Point result = node.P + neighbor;

                if (InRange(result))
                {
                    Size directionVector = neighbor;
                    if (IsStraight(node.D, neighbor))
                    {
                        directionVector += node.D;
                    }
                    else if (IsReverse(node.D, neighbor))
                    {
                        continue; // not allowed
                    }
                    else
                    {
                        // left / right
                        double prevStraightDistance = Distance(node.D); 
                        if (prevStraightDistance is > 0 and < 4 )
                        {
                            continue; // only allowed to after 4 straight moves 
                        }
                    }
                    
                    if(Distance(directionVector) > 10)
                    {
                        continue; // not allowed to ga straight for > 10x
                    }
                    
                    yield return new CruciblePoint(result, directionVector);
                }
            }
        }
    }
    
    private class CrucibleGrid : IWeightedGraph<CruciblePoint, double>
    {
        private readonly int[,] grid;
        public CrucibleGrid(int[,] grid)
        {
            this.grid = grid;
            Width = grid.Width();
            Height = grid.Heigth();
        }
        
        public int Width { get; }
        public int Height { get; }
        
        public virtual IEnumerable<CruciblePoint> Neighbors(CruciblePoint node)
        {
            foreach (var neighbor in SquareGrid.SquareNeightbors)
            {
                Point result = node.P + neighbor;

                if (InRange(result))
                {
                    Size directionVector = neighbor;
                    if (IsStraight(node.D, neighbor))
                    {
                        directionVector += node.D;
                    }
                    else if (IsReverse(node.D, neighbor))
                    {
                        continue; // not allowed
                    }
                    
                    if(Distance(directionVector) > 3)
                    {
                        continue; // not allowed to ga straight for > 3x
                    }
                    
                    yield return new CruciblePoint(result, directionVector);
                }
            }
        }
        
        protected bool IsStraight(Size a, Size d)
        {
            bool sameX = int.Sign(a.Width) == int.Sign(d.Width);
            bool sameY = int.Sign(a.Height) == int.Sign(d.Height);

            return sameX && sameY;
        }

        protected bool IsReverse(Size a, Size d)
        {
            Size fromRevers = new Size(-a.Width, -a.Height);
            bool reverse = IsStraight(fromRevers, d);
            return reverse;
        }

        protected double Distance(Size a) => Math.Abs(a.Height) + Math.Abs(a.Width); // Either Height or Width should be 0;
        
        protected bool InRange(Point p) => p.X >= 0 && p.X < Width &&
                                           p.Y >= 0 && p.Y < Height;

        private double Cost(Point d) => grid[d.X, d.Y];
        
        public double Cost(CruciblePoint _, CruciblePoint b) => Cost(b.P);
    }
}    