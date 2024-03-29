using System.Text;

namespace AdventOfCode.Year2019.Day18;

public class Maze
{
    private const char WALL = '#';
    private const char EMPTY = '.';
    private const char START = '@';

    private char[,] grid;
    private int width;
    private int height;

    private Dictionary<char, (int x, int y)> specials;

    public Maze(string[] input)
    {
        width = input[0].Length;
        height = input.Length;

        grid = new char[width, height];
        for (int y = 0; y < height; y++)
        {
            var line = input[y];
            for (int x = 0; x < width; x++)
            {
                grid[x,y] = line[x];
            }
        }

        FindAllSpecials();
    }

    public int Solve()
    {
        foreach(var q in Enumerable.Range(0, 4))
        {
            FindInQuarter(q);
        }

        //Display();

        return CalcuteShortestPathSeries(START, 'p', 'o', 'h', 'e', 'n', 'r', 'f', 'c', 'g', 'q', 'x', 'm', 'l', 'r', 'd', 's', 'v', 't', 'z');

    }

    public int Solve2()
    {
        ModifyGrid();

        var R0 = CalcuteShortestPathSeries('0', 'm', 'l', 'r', 'd'); // shortest!
        var R1 = CalcuteShortestPathSeries('1', 'q', 'c', 'w', 'f', 'x');
        var R2 = CalcuteShortestPathSeries('2', 's', 'n', 'g', 'v', 't', 'z');
        var R3 = CalcuteShortestPathSeries('3', 'p', 'o', 'h', 'e'); // shortest!

        return R0 + R1 + R2 + R3;
    }

    private void ModifyGrid()
    {
        var oldStart = specials[START];

        grid[oldStart.x, oldStart.y] = WALL;
        grid[oldStart.x + 1, oldStart.y] = WALL;
        grid[oldStart.x - 1, oldStart.y] = WALL;
        grid[oldStart.x, oldStart.y + 1] = WALL;
        grid[oldStart.x, oldStart.y - 1] = WALL;

        grid[oldStart.x - 1, oldStart.y - 1] = '0';
        grid[oldStart.x + 1, oldStart.y - 1] = '1';
        grid[oldStart.x - 1, oldStart.y + 1] = '2';
        grid[oldStart.x + 1, oldStart.y + 1] = '3';

        FindAllSpecials();// find new

        //Display();
    }

    private (int x, int y) FindChar(char ch)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(grid[x,y] == ch) return (x, y);
            }
        }

        throw new Exception("Char not found: " + ch);
    }

    private void Display()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Console.Write(grid[x,y]);
            }
            Console.WriteLine();
        }
    }

    private void FindAllSpecials()
    {
        specials = new Dictionary<char, (int x, int y)>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var currentCh = grid[x,y];
                if(currentCh == WALL || currentCh == EMPTY) continue;
                specials.Add(currentCh, (x, y));
            }
        }
    }

    private void FindInQuarter(int q)
    {
        var sb = new StringBuilder();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(q==0)
                {
                    if(x > width / 2) continue;
                    if(y > width / 2) continue;
                }
                else if(q==1)
                {
                    if(x < width / 2) continue;
                    if(y > width / 2) continue;
                }
                else if(q==2)
                {
                    if(x > width / 2) continue;
                    if(y < width / 2) continue;
                }
                else if(q==3)
                {
                    if(x < width / 2) continue;
                    if(y < width / 2) continue;
                }
                var ch = grid[x,y];
                if(char.IsLower(ch))
                {
                    sb.Append(ch);
                }
            }
        }
    }

    private class Edge
    {
        public int X;
        public int Y;
        public char Door;
        public char Key;
        public Edge Parent;

        public Edge(int x, int y)
        {
            X = x;
            Y = y;
            Parent = null;
            Door = '\0';
            Key = '\0';
        }
    }

    private (int l, string keys, string doors) CalcuteShortestPath(char from, char to, bool display = false)
    {
        var visited = new bool[width, height];

        var start = specials[from];

        visited[start.x,start.y] = true;

        Queue<Edge> queue = new Queue<Edge>();
        queue.Enqueue(new Edge(start.x, start.y));

        Edge currentEdge = null;
        while(queue.Count > 0)
        {
            currentEdge = queue.Dequeue();

            if(grid[currentEdge.X, currentEdge.Y] == to)
            {
                if(char.IsUpper(to)) currentEdge.Door = to;
                if(char.IsLower(to)) currentEdge.Key = to;
                break;
            }

            var edges = new Edge[4] {
                new Edge(currentEdge.X + 1, currentEdge.Y), 
                new Edge(currentEdge.X - 1, currentEdge.Y), 
                new Edge(currentEdge.X    , currentEdge.Y + 1), 
                new Edge(currentEdge.X    , currentEdge.Y - 1)
            };

            foreach(var edge in edges)
            {
                var pos = grid[edge.X, edge.Y];
                if(pos != WALL && !visited[edge.X, edge.Y])
                {
                    if(char.IsUpper(pos)) edge.Door = pos;
                    if(char.IsLower(pos)) edge.Key = pos;
                    visited[edge.X, edge.Y] = true;
                    queue.Enqueue(edge);
                    edge.Parent = currentEdge;
                }
            }
        }

        // count
        var pathLength = 0;
        StringBuilder doors = new StringBuilder();
        StringBuilder keys = new StringBuilder();
        if(currentEdge != null)
        {
            while(currentEdge.Parent != null)
            {
                if(grid[currentEdge.X, currentEdge.Y] == EMPTY)
                {
                    if(display)
                    {
                        grid[currentEdge.X, currentEdge.Y] = ' ';
                    }
                }
                if(currentEdge.Door != '\0')
                {
                    // wrong order :(
                    //if(keys.ToString().IndexOf(char.ToLower(currentEdge.Door)) < 0)
                    {
                        doors.Append(currentEdge.Door);
                    }
                }
                if(currentEdge.Key != '\0')
                {
                    keys.Append(currentEdge.Key);
                }
                currentEdge = currentEdge.Parent;
                pathLength++;
            }
        }

        var reverseDoors = new string(doors.ToString().Reverse().ToArray());
        var reverseKeys = new string(keys.ToString().Reverse().ToArray());

        //System.Console.WriteLine(from +"-" + to + " length " + pathLength.ToString("D3") + " door: " + string.Format("{0,-10}", reverseDoors) + " keys: " + string.Format("{0,-10}", reverseKeys));

        return (pathLength, reverseKeys, reverseDoors);
    }

    private int CalcuteShortestPathSeries(params char[] chs)
    {
        int sum = 0;
        var previous = chs[0];
        for(int i = 1; i < chs.Length; i++)
        {
            var currentCh = chs[i];

            (var length, _, _) = CalcuteShortestPath(previous, currentCh);
            sum += length;
            previous = currentCh;
        }

        //System.Console.WriteLine($"Path length {sum}: {string.Join('-', chs)}");

        return sum;
    }

    private string[] Combination(params char[] chars)
    {
        var query = from a in chars
            from b in chars
            from c in chars
            where a != b && a != c && b != c
            select "" + a + b + c;
            
        foreach (var item in query)
        {
            Console.WriteLine(item);
        }

        return query.ToArray();
    }

    static public IEnumerable<string> permute(string word)
    {
        if (word.Length > 1)
        {

            char character = word[0];
            foreach (string subPermute in permute(word.Substring(1)))
            {

                for (int index = 0; index <= subPermute.Length; index++)
                {
                    string pre = subPermute.Substring(0, index);
                    string post = subPermute.Substring(index);

                    if (post.Contains(character))
                        continue;                       

                    yield return pre + character + post;
                }

            }
        }
        else
        {
            yield return word;
        }
    }

    public IEnumerable<string> AddConstrainsts(IEnumerable<string> stream, string doors, char target)
    {
        IEnumerable<string> output = stream;
        foreach(var key in doors.ToLower())
        {
            output = output.Where(x => x.IndexOf(key) <= x.IndexOf(target));
        }
        return output;
    }

    private (int length, string path) SolveExact(string chars, string start = "", string end = "", char startChar = START)
    {
        var permutations = permute(chars);

        //System.Console.WriteLine(permutations.Count());

        //System.Console.WriteLine("Fact: " + Fact(chars.Length));

        if(Fact(chars.Length) > 10000000) throw new Exception("Impossible to solve");

        var improved = permutations;
        //foreach(var ch in Enumerable.Range('a', chars.Count()).Select(c => (char)c))
        foreach(var ch in chars)
        {
            var (l, keys, doors) = CalcuteShortestPath(startChar, ch);
            improved = AddConstrainsts(improved, doors, ch);
        }
        //System.Console.WriteLine("Added constraints");
        //System.Console.WriteLine("Auto: " + improved.Count());

        int min = int.MaxValue;
        string best = "";
        foreach(var word in improved)
        {
            //System.Console.WriteLine(word);
            var word2 = start + word + end;
            var length = CalcuteShortestPathSeries(word2.ToCharArray());
            if(length < min)
            {
                min = length;
                best = word2;
            }
        }
        return (min, best);
    }

    public int SolveAllCombinations()
    {
        var chars = new string(specials.Keys.Where(x => char.IsLower(x)).ToArray());
            
        var (best, min) = SolveExact(chars, START.ToString());
        Console.WriteLine($"Best {best} length {min}");

        return 0;
    }

    public static int Fact(int x)
    {
        if(x > 1) return x * Fact(x-1);
        return x;
    }
}