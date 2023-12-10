namespace AdventOfCode.Year2021.Day17;

[DayInfo(2021, 17)]
public class Solution17 : Solution
{
    private class Range
    {
        public int Min { get; set; }
        public int Max { get; set; }
        
        public Range(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public bool InRange(int v)
        {
            return v >= Min && v <= Max;
        }
    }

    private class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point a, Point b) => new (a.X + b.X, a.Y + b.Y);
        public override string ToString() => $"({X},{Y})";
    };

    private Range _Xtarget;
    private Range _Ytarget;

    private Point _location;
    private Point _velocity;
    
    public string Run()
    {
        var line = InputReader.ReadFileLines()[0];
        ParseLine(line);

        int A = SimulateRange();

        int B = SimulateDistinct();
        
        return A + "\n" + B;
    }
    
    private int SimulateDistinct()
    {
        int count = 0;
        for (int x = 0; x <= _Xtarget.Max; x++)
        {
            for (int y = _Ytarget.Min; y < 100; y++)
            {
                var velocity = new Point(x, y);

                bool success = Simulate(velocity, out _);
                if (success)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private int SimulateRange()
    {
        int maxHeight = 0;
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                var velocity = new Point(x, y);

                bool success = Simulate(velocity, out int height);
                if (success && height > maxHeight)
                {
                    maxHeight = height;
                }
            }
        }

        return maxHeight;
    }

    private bool Simulate(Point initialVelocity, out int maxY)
    {
        _velocity = initialVelocity;
        _location = new Point(0, 0);
        int step = 0;
        maxY = 0;

        bool success = false;
        while (true)
        {
            Step();
            step++;
            //Console.WriteLine($"{step}- {_location} {_velocity}");

            if (_location.Y > maxY)
            {
                maxY = _location.Y;
            }

            if (InArea())
            {
                success = true;
                break;
            }

            if (MissedArea())
            {
                break;
            }
        }

        return success;
    }

    private bool InArea()
        => _Xtarget.InRange(_location.X) && _Ytarget.InRange(_location.Y);

    private bool MissedArea()
        => _location.Y < _Ytarget.Min ||
           _location.X > _Xtarget.Max ||
           _velocity.X == 0 && !_Xtarget.InRange(_location.X);

    private void Step()
    {
        _location += _velocity;
        _velocity.X -= Math.Sign(_velocity.X);
        _velocity.Y -= 1;
    }

    private void ParseLine(string line)
    {
        int indexX = line.IndexOf('x');
        var coordinates = line[indexX..].Split(", ");

        _Xtarget = ParseRange(coordinates[0][2..]);
        _Ytarget = ParseRange(coordinates[1][2..]);
    }

    private Range ParseRange(string range)
    {
        var rangeSplit = range.Split("..");
        return new Range(int.Parse(rangeSplit[0]), int.Parse(rangeSplit[1]));
    }
}
