using System.Drawing;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode.Year2022.Day22;

public class Solution22 : Solution
{
    public class JungleMap
    {
        private record Edge(int SideFrom, int SideTo, int MoveLeftCount, bool FlipX = false, bool FlipY = false);
        private record Face(int X, int Y, int Id);

        private const int FACE_SIZE = 50;
        private readonly string[] _tiles;
        private readonly char[,] _path;
        private Point _current;
        private Size _direction;

        public JungleMap(string[] tiles)
        {
            _tiles = tiles;
            _path = new char[_tiles.Length, _tiles[0].Length];
            for (int y = 0; y < _tiles.Length; y++)
            {
                for (int x = 0; x < _tiles[0].Length; x++)
                {
                    char c = x >= _tiles[y].Length ? ' ' : _tiles[y][x];
                    _path[y, x] = c;
                }
            }
            _direction = new Size(1, 0);
            Point start = Point.Empty;
            for (int x = 0; x < _tiles[0].Length; x++)
            {
                if (_tiles[0][x] == '.')
                {
                    start = new Point(x, 0);
                    break;
                }
            }
            _current = start;
        }

        public void Print()
        {
            StringBuilder sb = new();
            for (int y = 0; y < _path.GetLength(0); y++)
            {
                for (int x = 0; x < _path.GetLength(1); x++)
                {
                    sb.Append(_path[y, x]);
                }

                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }

        public void TurnLeftCount(int count)
        {
            for (int i = 0; i < count; i++)
            {
                TurnLeft();
            }
        }

        public void TurnLeft()
        {
            _direction = RotateLeft(_direction);
        }

        private Size RotateLeft(Size size)
        {
            return new Size(size.Height, -size.Width);
        }

        public void TurnRight()
        {
            _direction = new (-_direction.Height, _direction.Width);
        }

        private void Draw(Point p, Size d)
        {
            if (_path[p.Y, p.X] == '#') throw new Exception();
            _path[p.Y, p.X] = d switch
            {
                { Width: 1, Height: 0 } => '>',
                { Width: 0, Height: 1 } => 'v',
                { Width: -1, Height: 0 } => '<',
                { Width: 0, Height: -1 } => '^',
            };
        }

        public bool Move(int tiles)
        {
            bool stopped = false;
            Point next = _current;
            for (int i = 0; i < tiles; i++)
            {
                next += _direction;

                if (IsOpen(next))
                {
                    _current = next;
                }
                else if (IsWrap(next))
                {
                    Point wrap = Wrap();
                    if (IsOpen(wrap))
                    {
                        next = wrap;
                        _current = next;
                    }
                    else
                    {
                        stopped = true;
                        break;
                    }
                }
                else
                {
                    stopped = true;
                    break;
                }
                Draw(next, _direction);
            }

            return stopped;
        }

        public bool MoveB(int tiles)
        {
            bool stopped = false;
            Point prev;
            Point next = _current;
            Draw(next, _direction);
            for (int i = 0; i < tiles; i++)
            {
                prev = next;
                next += _direction;

                bool wasWrap = false;

                if (IsOpen(next))
                {
                    _current = next;
                }
                else if (IsWrap(next))
                {
                    Point wrap = WrapB(prev, next, out int moveLeftCount);
                    if (IsOpen(wrap))
                    {
                        next = wrap;
                        _current = next;
                        TurnLeftCount(moveLeftCount);
                        wasWrap = true;
                    }
                    else
                    {
                        stopped = true;
                        break;
                    }
                }
                else
                {
                    stopped = true;
                    break;
                }
                Draw(next, _direction);
                if (wasWrap)
                {
                    //Print();
                }
            }

            return stopped;
        }

        private readonly List<Edge> _edges = new ()
        {
            new(1, 3, 0),
            new(1, 2, 0),
            new(2, 6, 0),
            new(5, 6, 0),
            new(4, 5, 0),

            new(1, 5, 2, FlipY: true), // position also flip vertically
            new(1, 4, 3),
            new(2, 3, 1),
            new(2, 5, 1),
            new(3, 4, 0, FlipY: true),
            new(3, 6, 2, FlipY: true),// position also flip vertically
            new(4, 6, 1),
        };

        private readonly Dictionary<int, Face> _faces = new()
        {
            { 1, new Face(1, 0, 1) },
            { 2, new Face(1, 1, 2) },
            { 3, new Face(2, 0, 3) },
            { 4, new Face(0, 3, 4) },
            { 5, new Face(0, 2, 5) },
            { 6, new Face(1, 2, 6) },
        };

        private HashSet<Edge> firstTimeEdge = new();

        public (Point point, Size direction) TestWrap(Point prev, Point next)
        {
            Size direction = new Size(next.X - prev.X, next.Y - prev.Y);
            Point wrap = WrapB(prev, next, out int leftCount);
            Size rotated = direction;
            for (int i = 0; i <leftCount; i++)
            {
                rotated = RotateLeft(rotated);
            }

            return (wrap, rotated);
        }

        private Point WrapB(Point prev, Point outside, out int moveLeftCount)
        {
            int side = PointSide(prev);
            int nextSide = OtherSide(outside, side);

            Face current = _faces[side];
            Face target = _faces[nextSide];

            Edge edge;
            if (side < nextSide)
            {
                edge = _edges.Single(e => e.SideFrom == side && e.SideTo == nextSide);
            }
            else
            {
                Edge revereEdge = _edges.Single(e => e.SideFrom == nextSide && e.SideTo == side);
                edge = revereEdge with
                {
                    SideFrom = revereEdge.SideTo, 
                    SideTo = revereEdge.SideFrom,
                    MoveLeftCount = (4 - revereEdge.MoveLeftCount) % 4
                };
            }

            moveLeftCount = edge.MoveLeftCount;

            int remainderX = prev.X % FACE_SIZE;
            int remainderY = prev.Y % FACE_SIZE;

            Size remainder = new(remainderX, remainderY);
            Size rotated = remainder;
            if (moveLeftCount % 2 == 1)
            {
                rotated = FlipXY(rotated);
            }

            if (edge.FlipX)
            {
                rotated = FlipX(rotated);
            }

            if (edge.FlipY)
            {
                rotated = FlipY(rotated);
            }


            if (!firstTimeEdge.Contains(edge))
            {
                firstTimeEdge.Add(edge);
            }

            Size translation = new ((target.X - current.X) * FACE_SIZE, (target.Y - current.Y) * FACE_SIZE);

            Point next = prev + translation - remainder + rotated;

            return next;
        }

        private Size FlipXY(Size p) => new (p.Height, p.Width);
        private Size FlipX(Size p) => p with { Width = FACE_SIZE - 1 - p.Width };
        private Size FlipY(Size p) => p with { Height = FACE_SIZE - 1 - p.Height };


        private int PointSide(Point p)
        {
            int x = p.X < 0 ? -1 : p.X / FACE_SIZE;
            int y = p.Y < 0 ? -1 : p.Y / FACE_SIZE;
            int side = (x, y) switch
            {
                { x: 1, y: 0 } => 1,
                { x: 2, y: 0 } => 3,
                { x: 1, y: 1 } => 2,
                { x: 1, y: 2 } => 6,
                { x: 0, y: 2 } => 5,
                { x: 0, y: 3 } => 4,
                _ => throw new ArgumentOutOfRangeException()
            };
            return side;
        }

        private int OtherSide(Point p, int prevSide)
        {
            int x = p.X < 0 ? -1 : p.X / FACE_SIZE;
            int y = p.Y < 0 ? -1 : p.Y / FACE_SIZE;
            int side = (x, y) switch
            {
                { x: 0, y: 0 } => 5,
                { x: 1, y: -1 } => 4,
                { x: 2, y: -1 } => 4,
                { x: 3, y: 0 } => 6,
                { x: 0, y: 1 } => prevSide == 2 ? 5 : 2,
                { x: 2, y: 1 } => prevSide == 2 ? 3 : 2,
                { x: -1, y: 2 } => 1,
                { x: 2, y: 2 } => 3,
                { x: -1, y: 3 } => 1,
                { x: 1, y: 3 } => prevSide == 4 ? 6 : 4,
                { x: 0, y: 4 } => 3,
                _ => throw new ArgumentOutOfRangeException()
            };
            return side;
        }

        private Point Wrap()
        {
            int wrapX = _direction.Width switch
            {
                -1 => _tiles[_current.Y].Length - 1,
                0 => _current.X,
                1 => 0,
                _ => throw new ArgumentOutOfRangeException()
            };
            int wrapY = _direction.Height switch
            {
                -1 => _tiles.Length - 1,
                0 => _current.Y,
                1 => 0,
                _ => throw new ArgumentOutOfRangeException()
            };
            Point wrap = new Point(wrapX, wrapY);
            while (IsWrap(wrap))
            {
                wrap += _direction;
            }

            return wrap;
        }

        public int Password()
        {
            int row = _current.Y + 1;
            int column = _current.X + 1;
            int facing = _direction switch
            {
                { Width: 1, Height: 0 } => 0,
                { Width: 0, Height: 1 } => 1,
                { Width: -1, Height: 0 } => 2,
                { Width: 0, Height: -1 } => 3,
                _ => throw new ArgumentOutOfRangeException()
            };
            return 1000 * row + 4 * column + facing;
        }

        private bool InRange(Point p) => p.Y >= 0 &&
                                         p.Y < _tiles.Length &&
                                         p.X >= 0 &&
                                         p.X < _tiles[p.Y].Length;
                                         
        private bool IsWall(Point p) => InRange(p) && _tiles[p.Y][p.X] == '#';
        private bool IsOpen(Point p) => InRange(p) && _tiles[p.Y][p.X] == '.';
        private bool IsWrap(Point p) => !InRange(p) || _tiles[p.Y][p.X] == ' ';
    }

    public static string[] GetInput() => InputReader.ReadFileLinesArray();

    public string Run()
    {
        string example = """ 
                    ...#
                    .#..
                    #...
                    ....
            ...#.......#
            ........#...
            ..#....#....
            ..........#.
                    ...#....
                    .....#..
                    .#......
                    ......#.

            10R5L5R10L4R5L5
            """;
        //string[] lines = example.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        var lines = InputReader.ReadFileLinesArray();

        var mapLines = lines.AsSpan().Slice(0, lines.Length - 1).ToArray();

        var passwordA = PasswordA(mapLines, lines);
        var passwordB = PasswordB(mapLines, lines);

        return passwordA + "\n" + passwordB;
    }

    private static int PasswordA(string[] mapLines, string[] lines)
    {
        JungleMap jungle = new(mapLines);
        LineReader reader = new LineReader(lines[lines.Length - 1]);
        bool isMove = reader.IsDigit;
        while (!reader.IsDone)
        {
            if (isMove)
            {
                int move = reader.ReadInt();
                jungle.Move(move);
            }
            else
            {
                char rotate = reader.ReadChar();
                if (rotate == 'R') jungle.TurnRight();
                else if (rotate == 'L') jungle.TurnLeft();
            }

            isMove = !isMove;
        }
        
        //jungle.Print();
        int password = jungle.Password();
        return password;
    }

    private static int PasswordB(string[] mapLines, string[] lines)
    {
        JungleMap jungle = new(mapLines);
        LineReader reader = new LineReader(lines[lines.Length - 1]);
        bool isMove = reader.IsDigit;
        while (!reader.IsDone)
        {
            if (isMove)
            {
                int move = reader.ReadInt();
                jungle.MoveB(move);
            }
            else
            {
                char rotate = reader.ReadChar();
                if (rotate == 'R') jungle.TurnRight();
                else if (rotate == 'L') jungle.TurnLeft();
            }

            isMove = !isMove;
        }

        jungle.Print();

        int password = jungle.Password();
        return password;
        // 135204 too low
        // 132268 also to low
    }
}
