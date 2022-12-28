using System.Drawing;
using System.Text;

namespace AdventOfCode.Year2022.Day23;

[DayInfo(2022, 23)]
public class Solution23 : Solution
{
    private class Elf
    {
        public static Size North = new(0, -1);
        public static Size South = new(0, 1);
        public static Size West = new(-1, 0);
        public static Size East = new(1, 0);
        public static Size NW = North + West;
        public static Size NE = North + East;
        public static Size SW = South + West;
        public static Size SE = South + East;
        
        public static Size[] Directions = { North, South, West, East};
        public static Size[] Neightbors = { North, South, West, East, NW, NE, SW, SE};
        
        public Point Position { get; private set; }
        public Point Target { get; private set; }
        private int _direction = 0;

        public Elf(Point position)
        {
            Position = position;
            Target = Position;
        }

        public void ResetTarget()
        {
            Target = Position;
        }
        
        public bool ApplyTarget()
        {
            bool moved = Position != Target;
            Position = Target;
            return moved;
        }

        public Point ChooseMove(Dictionary<Point, Elf> elvePositions, int firstDirection)
        {
            bool alone = true;
            foreach (Size neightbor in Neightbors)
            {
                Point next = Position + neightbor;
                if (elvePositions.ContainsKey(next))
                {
                    alone = false;
                    break;
                }
            }

            if (alone)
            {
                Target = Position;
            }
            else
            {
                for (int i = 0; i < Directions.Length; i++)
                {
                    int direction = (i + firstDirection) % Directions.Length;
                    if (CanMove2(elvePositions, direction))
                    {
                        Target = Position + Directions[direction];
                        break;
                    }
                }
            }

            return Target;
        }

        private bool CanMove(Dictionary<Point, Elf> elvePositions, int direction)
        {
            return direction switch
            {
                0 => !elvePositions.ContainsKey(Position + North) &&
                     !elvePositions.ContainsKey(Position + NE) &&
                     !elvePositions.ContainsKey(Position + NW),
                1 => !elvePositions.ContainsKey(Position + South) &&
                     !elvePositions.ContainsKey(Position + SE) &&
                     !elvePositions.ContainsKey(Position + SW),
                2 => !elvePositions.ContainsKey(Position + West) &&
                     !elvePositions.ContainsKey(Position + NW) &&
                     !elvePositions.ContainsKey(Position + SW),
                3 => !elvePositions.ContainsKey(Position + East) &&
                     !elvePositions.ContainsKey(Position + NE) &&
                     !elvePositions.ContainsKey(Position + SE),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        private bool CanMove2(Dictionary<Point, Elf> elvePositions, int direction)
        {
            return direction switch
            {
                0 => ValidPosition(elvePositions, North, NE, NW),
                1 => ValidPosition(elvePositions, South, SE, SW),
                2 => ValidPosition(elvePositions, West, NW, SW),
                3 => ValidPosition(elvePositions, East, NE, SE),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        private bool ValidPosition(Dictionary<Point, Elf> elvePositions, params Size[] checks)
        {
            bool valid = true;
            foreach (Size check in checks)
            {
                Point next = Position + check;
                if (elvePositions.ContainsKey(next))
                {
                    valid = false;
                    break;
                }
            }

            return valid;
        }
    }
    public string Run()
    {
        string example1 = """
            .....
            ..##.
            ..#..
            .....
            ..##.
            .....
            """;
        string example = """
            ..............
            ..............
            .......#......
            .....###.#....
            ...#...#.#....
            ....#...##....
            ...#.###......
            ...##.#.##....
            ....#..#......
            ..............
            ..............
            ..............
            """;
        //string[] lines = example.Split("\r\n");
        var lines = InputReader.ReadFileLinesArray();
        List<Elf> elves = Parse(lines).ToList();
        Rounds(elves, 10);
        int emptyGround = FindArea(elves);

        elves = Parse(lines).ToList();
        int roundStop = Rounds(elves, 10_000);
        
        return emptyGround + "\n" + roundStop;
    }

    private int FindArea(List<Elf> elves)
    {
        int minX = elves.Min(x => x.Position.X);
        int maxX = elves.Max(x => x.Position.X);        
        int minY = elves.Min(x => x.Position.Y);
        int maxY = elves.Max(x => x.Position.Y);

        int sizeX = maxX - minX + 1;
        int sizeY = maxY - minY + 1;

        Print(elves);

        int totalSize = sizeX * sizeY;

        return totalSize - elves.Count;
    }

    private void Print(List<Elf> elves)
    {
        int minX = elves.Min(x => x.Position.X);
        int maxX = elves.Max(x => x.Position.X);        
        int minY = elves.Min(x => x.Position.Y);
        int maxY = elves.Max(x => x.Position.Y);

        int sizeX = maxX - minX + 1;
        int sizeY = maxY - minY + 1;

        char[,] map = new char[sizeX, sizeY];
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                map[x, y] = '.';
            }
        }
        
        foreach (Elf elf in elves)
        {
            int x = elf.Position.X - minX;
            int y = elf.Position.Y - minY;
            map[x, y] = '#';
        }

        StringBuilder sb = new();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                sb.Append(map[x, y]);
            }

            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }

    private int Rounds(List<Elf> elves, int n)
    {
        foreach (var round in Enumerable.Range(0, n))
        {
            if (Round(elves, round % 4))
            {
                return round + 1;
            };
        }

        return n;
    }

    private bool Round(List<Elf> elves, int round)
    {
        Dictionary<Point,Elf> positionElves = elves.ToDictionary(x => x.Position, x => x);
        
        // Propose move
        foreach (Elf elf in elves)
        {
            elf.ChooseMove(positionElves, round % 4);
        }

        // Validate target
        Dictionary<Point, Elf> nextPositions = new();
        foreach (Elf elf in elves)
        {
            if (!nextPositions.ContainsKey(elf.Target))
            {
                nextPositions.Add(elf.Target, elf);
            }
            else
            {
                Elf other = nextPositions[elf.Target];
                other.ResetTarget();
                elf.ResetTarget();
            }
        }

        // Actual move
        bool moved = false;
        foreach (Elf elf in elves)
        {
            moved |= elf.ApplyTarget();
            elf.ResetTarget();
        }

        return !moved;
    }

    private IEnumerable<Elf> Parse(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    yield return new Elf(new Point(x, y));
                }
            }
        }
    }
}
