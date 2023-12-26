using System.Drawing;
using System.Runtime.InteropServices.JavaScript;

namespace AdventOfCode.Year2022.Day17;

[DayInfo(2022, 17)]
public class Solution17 : Solution
{
    private const bool Debug = false;
    
    private record Shape(Size Size, Size[] Spaces);
    private static Shape HLINE = new Shape(new Size(4, 1), new []{ new Size(0, 0), new Size(1, 0), new Size(2, 0), new Size(3, 0)});
    private static Shape PLUS = new Shape(new Size(3, 3), new []{ new Size(1, 0), new Size(0, 1), new Size(1, 1), new Size(2, 1), new Size(1, 2)});
    private static Shape CORNER = new Shape(new Size(3, 3), new []{ new Size(0, 0), new Size(1, 0), new Size(2, 0), new Size(2, 1), new Size(2, 2)});
    private static Shape VLINE = new Shape(new Size(1, 4), new []{ new Size(0, 0), new Size(0, 1), new Size(0, 2), new Size(0, 3)});
    private static Shape SQUARE = new Shape(new Size(2, 2), new []{ new Size(0, 0), new Size(1, 0), new Size(0, 1), new Size(1, 1)});
    
    private string _jets;
    private int _jetsPosition = 0;
    private List<char[]> _chamber = new ();
    private int _topRock = 0;

    private void Reset()
    {
        _jetsPosition = 0;
        _topRock = 0;
        _chamber = new List<char[]>();
        _chamber.Add(new string('-', 7).ToCharArray());
    }
    
    public string Run()
    {
        const string example = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";
        string[] lines = this.ReadLines();
        _jets = lines[0];

        Reset();
        long rocksA = Steps(2022);

        Reset();
        long rocksB = StepsB();
        
        return rocksA + "\n" + rocksB;
    }


    private void Print(List<char[]> chamber, string caption)
    {
        if(!Debug) return;
        
        foreach (var line in chamber.AsEnumerable().Reverse())
        {
            Console.WriteLine($"|{new string(line)}|");
        }
        Console.WriteLine(caption);

        Console.WriteLine();
    }

    private void AddEmptyLine(List<char[]> chamber)
    {
        chamber.Add(new string('.', 7).ToCharArray());
    }

    private char Jet()
    {
        char c = _jets[_jetsPosition];
        _jetsPosition = (_jetsPosition + 1) % _jets.Length;
        return c;
    }

    private long Steps(int count)
    {
        for(int i = 0; i < count; i++)
        {
            int shapeId = i % 5;
            Step(shapeId);
        }

        return _topRock;
    }

    private void Step(int shapeId)
    {
        Shape shape = shapeId switch
        {
            0 => HLINE,
            1 => PLUS,
            2 => CORNER,
            3 => VLINE,
            4 => SQUARE,
            _ => throw new ArgumentOutOfRangeException()
        };

        Point spawn = new Point(2, _topRock + 3 + 1);
        int linesToAdd = spawn.Y + shape.Size.Height - _chamber.Count;
        for (int i = 0; i < linesToAdd; i++)
        {
            AddEmptyLine(_chamber);
        }
        
        Draw(shape, spawn, '@');
        Print(_chamber, "Spawn");
        
        Point current = spawn;
        bool stopped = false;
        while (!stopped)
        {
            // Push by jet
            char jet = Jet();
            Point target = current + (jet == '>' ? new Size(1, 0) : new Size(-1, 0));
            
            // Check possible
            if (target.X < 0 || target.X + shape.Size.Width > 7 || Collides(shape, target))
            {
                // Dont move
                target = current;
            }

            if (target != current)
            {
                Draw(shape, current, '.');
                Draw(shape, target, '@');
            }
            Print(_chamber, $"Move {jet}");

            // Fall
            Point downTarget = target + new Size(0, -1);
            if (downTarget.Y < 1 || Collides(shape, downTarget))
            {
                stopped = true;
                downTarget = target;
                
                // Update topRock
                int top = target.Y + shape.Size.Height - 1;
                _topRock = Math.Max(_topRock, top);
            }

            if (target != downTarget || stopped)
            {
                Draw(shape, target, '.');
                Draw(shape, downTarget, stopped ? '#' : '@');
            }
            Print(_chamber, $"Move down");

            current = downTarget;
        }
    }

    private bool Collides(Shape shape, Point position)
    {
        bool collides = false;
        foreach (Size shapeSpace in shape.Spaces)
        {
            Point offset = position + shapeSpace;
            if (_chamber[offset.Y][offset.X] == '#')
            {
                collides = true;
                break;
            }
        }

        return collides;
    }

    private void Draw(Shape shape, Point position, char c)
    {
        foreach (Size shapeSpace in shape.Spaces)
        {
            Point offset = position + shapeSpace;
            _chamber[offset.Y][offset.X] = c;
        }
    }
    
    private record State(string TopLine, string BelowTop, int ShapeId, int JetPosition);

    private record Output(int Count, int Toprock);
    private record StepData(State State, Output Output);

    private long StepsB()
    {
        Dictionary<State, Output> states = new();
        List<StepData> stepData = new() { new StepData(new State("", "", 0, 0), new Output(0, -1)) };

        Output startPeriod = null;
        Output repeatedStart = null;
        int previousRepeated = 0;
        for(int i = 0;; i++)
        {
            int shapeId = i % 5;
            Step(shapeId);
            State state = new State(new string(_chamber[_topRock]), new string(_chamber[_topRock - 1]), shapeId, _jetsPosition);
            Output output = new Output(i+1, _topRock);
            stepData.Add(new StepData(state, output));
            if (states.ContainsKey(state))
            {
                if (previousRepeated == 0)
                {
                    startPeriod = states[state];
                    repeatedStart = output;
                }
                else if (previousRepeated > 3000)
                {
                    Console.WriteLine($"Repeated state after {i}");
                    break;
                }

                previousRepeated++;
            }
            else
            {
                startPeriod = null;
                repeatedStart = null;
                previousRepeated = 0;
                states.Add(state, output);
            }

            if (i > 10_000)
            {
                throw new Exception("Can't find period");
            }
        }

        long period = repeatedStart.Count - startPeriod.Count;
        long rocksPerPeriod = repeatedStart.Toprock - startPeriod.Toprock;

        StepData startData = stepData[startPeriod.Count];
        for (int i = startPeriod.Count; i < stepData.Count; i += (int)period)
        {
            StepData data = stepData[i];
            if (data.State != startData.State)
            {
                throw new Exception("Fake period calculated");
            }
        }

        long initialCount = startPeriod!.Count;
        const long Target = 1_000_000_000_000;
        long periodicPart = Target - initialCount;
        long periods = periodicPart / period;
        long remainder = periodicPart % period;

        long topRock = stepData[(int)(initialCount + remainder)].Output.Toprock;
        long totalRock = topRock + periods * rocksPerPeriod;
        return totalRock;
    }
}
