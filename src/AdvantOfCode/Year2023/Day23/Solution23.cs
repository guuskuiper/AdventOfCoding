namespace AdventOfCode.Year2023.Day23;

[DayInfo(2023, 23)]
public class Solution23 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        char[,] grid = input.ToGrid();
        IslandGrid island = new(grid);
        Point start = new(1, 0);
        Point end = new(grid.Width() - 2, grid.Heigth()-1);
        int length = FindLongest(island, start, end);

        var wg = WeighedIslandGraph.Create(island, start, end);
        long part2 = FindLongest(wg);

        return length + "\n" + part2;
    }

    private long FindLongest(WeighedIslandGraph graph)
    {
        NodeState startState = new(0, 0, 0);
        long maxDistance = 0;
        AQueue<NodeState> queue = [startState];

        // must take that path, otherwise the end is blocked
        WeightedEdge lastEdge = graph.End.Edges.Single();
        int secondLastId = lastEdge.End.Id;
        
        long states = 0;
        while (!queue.Empty)
        {
            states++;
            NodeState current = queue.Get();
            foreach (var result in Step(graph, current))
            {
                if (result.Pos == secondLastId)
                {
                    int totalDistance = result.Distance + lastEdge.Weight;
                    if (totalDistance > maxDistance)
                    {
                        maxDistance = totalDistance;
                    }
                }
                else
                {
                    queue.Add(result);
                }
            }
        }

        return maxDistance;
    }

    private record NodeState(int Pos, long Visited, int Distance);
        
    private bool IsSet(long visited, int id) => (visited & (1L << id)) != 0;
    private long Set(long visited, int id) => visited | (1L << id);

    private IEnumerable<NodeState> Step(WeighedIslandGraph graph, NodeState state)
    {
        var current = graph.Nodes[state.Pos];

        foreach (WeightedEdge edge in current.Edges)
        {
            int endId = edge.End.Id;
            if(IsSet(state.Visited, endId)) continue;

            long newVisited = Set(state.Visited, endId);

            yield return new NodeState(endId, newVisited, state.Distance + edge.Weight);
        }
    }


    private int FindLongest(IslandGrid grid, Point start, Point end)
    {
        IGraph<Point> dag = CreateDag(grid, start);
        List<Point> sorted = TopologicalSort.Sort(dag, start);

        Dictionary<Point, int> costs = [];
        costs[start] = 0;

        foreach (Point point in sorted)
        {
            int cost = costs[point];
            foreach (Point neighbor in dag.Neighbors(point))
            {
                if (costs.TryGetValue(neighbor, out int costNeighbor))
                {
                    if (costNeighbor < cost + 1)
                    {
                        costs[neighbor] = cost + 1;
                    }
                }
                else
                {
                    costs[neighbor] = cost + 1;
                }
            }
        }

        return costs[end];
    }

    private IGraph<Point> CreateDag(IslandGrid grid, Point start)
    {
        Dag dag = new();
        AQueue<Point> frontier = [start];
        HashSet<Point> nodes = [start];

        while (!frontier.Empty)
        {
            Point current = frontier.Get();

            foreach (Point next in grid.Neighbors(current))
            {
                if(!grid.IsInDirectionPassable(current, next)) continue;
                
                if(!nodes.Contains(next))
                {
                    frontier.Add(next);
                    nodes.Add(next);
                    dag.AddEdge(current, next);
                }
                else
                {
                    char c = grid[current];
                    if (c != '.')
                    {
                        dag.AddEdge(current, next);
                    }
                }
            }
        }

        return dag;
    }

    private record WeightedEdge(WeightedNode Start, WeightedNode End, int Weight);

    private record WeightedNode(Point Point, int Id)
    {
        public List<WeightedEdge> Edges = new();
    }
    private class WeighedIslandGraph : IWeightedGraph<WeightedNode, int>
    {
        private List<WeightedEdge> edges = [];
        private List<WeightedNode> nodes = [];

        public static WeighedIslandGraph Create(IGraph<Point> graph, Point start, Point end)
        {
            WeighedIslandGraph wg = new();

            HashSet<Point> originalNodes = TopologicalSort.Nodes(graph, start);
            HashSet<WeightedNode> wgNodes = [new WeightedNode(start, 0), new WeightedNode(end, 1)];
            foreach (Point node in originalNodes)
            {
                int neighbors = graph.Neighbors(node).Count();
                if (neighbors > 2)
                {
                    wgNodes.Add(new WeightedNode(node, wgNodes.Count));
                }
            }

            List<WeightedEdge> edges = [];
            foreach (WeightedNode node in wgNodes)
            {
                // find edges with distances
                Point nodePoint = node.Point;
                foreach (var initialDirection in graph.Neighbors(nodePoint))
                {
                    HashSet<Point> visited = [nodePoint, initialDirection];
                    Point current = initialDirection;

                    while (!wgNodes.Select(x => x.Point).Contains(current))
                    {
                        current = graph.Neighbors(current).Single(p => !visited.Contains(p));
                        visited.Add(current);
                    }

                    WeightedNode endNode = wgNodes.Single(n => n.Point == current);
                    int distance = visited.Count - 1;
                    var edge = new WeightedEdge(node, endNode, distance);
                    edges.Add(edge);
                    node.Edges.Add(edge);
                }
            }

            wg.edges = edges;
            List<WeightedNode> wgNodeList = wgNodes.ToList();
            wg.nodes = wgNodeList;
            wg.Start = wgNodeList[0];
            wg.End = wgNodeList[1];

            (Dictionary<WeightedNode, WeightedNode>? weightedNodes, Dictionary<WeightedNode, int>? costs) = Dijkstra.SearchFrom(wg, wg.Start, wg.End);
            Dictionary<Point, Point> res = BFS.SearchFrom(graph, start, end);

            Point c = end;
            List<Point> p = [];
            while (c != start)
            {
                c = res[c];
                p.Add(c);
            }

            int s2 = p.Count;
            int shortestPath = costs[wgNodeList[1]];
            return wg;
        }
        
        public WeightedNode Start { get; private set; }
        public WeightedNode End { get; private set; }

        public List<WeightedNode> Nodes => nodes;


        public IEnumerable<WeightedNode> Neighbors(WeightedNode node)
        {
            return node.Edges.Select(x => x.End);
        }

        public int Cost(WeightedNode a, WeightedNode b)
        {
            return a.Edges.Single(e => e.End == b).Weight;
        }
    }

    private class Dag : IGraph<Point>
    {
        private readonly Dictionary<Point, List<Point>> _neighbours = []; 

        public void AddEdge(Point from, Point to)
        {
            List<Point> list = _neighbours.GetOrCreate(from);
            list.Add(to);
        }

        public Dictionary<Point, List<Point>> Temp => _neighbours;
        
        public IEnumerable<Point> Neighbors(Point node)
        {
            _neighbours.TryGetValue(node, out List<Point>? neighbors);
            return neighbors ?? [];
        }
    }

    private class IslandGrid : IRectGrid<Point>, IValueGrid<char>
    {
        private readonly char[,] _grid;
        public IslandGrid(char[,] grid)
        {
            _grid = grid;
            Width = grid.Width();
            Height = grid.Heigth();
        }
        
        public int Width { get; }
        public int Height { get; }

        public IEnumerable<Point> Neighbors(Point node)
        {
            foreach (var neighbor in SquareGrid.SquareNeightbors)
            {
                Point result = node + neighbor;

                if (InRange(result))
                {
                    if(IsForest(result)) continue;
                    yield return result;
                }
            }
        }

        public bool IsInDirectionPassable(Point from, Point to)
        {
            bool passable = true;
            if (!IsValidHill(from, to))
            {
                passable = false;
            }
            if (IsOppositeHill(from, to))
            {
                passable = false;
            }

            return passable;
        }

        private bool IsForest(Point p) => this[p] == '#';

        private bool IsValidHill(Point hill, Point destination)
        {
            char c = this[hill];
            Size direction = c switch
            {
                '>' => new Size(1, 0),
                '<' => new Size(-1, 0),
                '^' => new Size(0, -1),
                'v' => new Size(0, 1),
                _ => Size.Empty
            };
            Point hillResult = hill + direction;
            return direction == Size.Empty || destination == hillResult;
        }
        
        private bool IsOppositeHill(Point current, Point destination)
        {
            char c = this[destination];
            Size direction = c switch
            {
                '>' => new Size(1, 0),
                '<' => new Size(-1, 0),
                '^' => new Size(0, -1),
                'v' => new Size(0, 1),
                _ => Size.Empty
            };
            Point hillResult = destination + direction;
            return current == hillResult;
        }

        public char this[Point p] => _grid[p.X, p.Y];

        private bool InRange(Point p) => p.X >= 0 && p.X < Width &&
                                         p.Y >= 0 && p.Y < Height;
    }
}    