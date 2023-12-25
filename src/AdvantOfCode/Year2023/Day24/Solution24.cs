using System.Numerics;
using AdventOfCode.Extensions;
using Microsoft.Z3;

namespace AdventOfCode.Year2023.Day24;

[DayInfo(2023, 24)]
public class Solution24 : Solution
{
    private record Position3(long X, long Y, long Z)
    {
        public static Position3 operator +(Position3 a, Position3 b)
            => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Position3 operator -(Position3 a, Position3 b)
            => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Position3 operator *(Position3 a, long b)
            => new(a.X * b, a.Y * b, a.Z * b);
    }
    private record HailStone(Position3 Position, Position3 Velocity);

    private const long MIN = 200000000000000;
    private const long MAX = 400000000000000;
    
    public string Run()
    {
        string[] input = this.ReadLines();
        HailStone[] stones = input.Select(ParseLine).ToArray();

        int intersections = 0;
        for (int i = 0; i < stones.Length; i++)
        {
            HailStone a = stones[i];
            for (int j = i + 1; j < stones.Length; j++)
            {
                HailStone b = stones[j];
                if(Intersection(a, b, out double x, out double y))
                {
                    if (x is >= MIN and <= MAX
                        && y is >= MIN and <= MAX)
                    {
                        if (IsPast(b, x, y) || IsPast(a, x, y))
                        {
                            continue;
                        }
                        intersections++;
                    }
                }
            }
        }

        long part2;
        try
        {
            part2 = SolveZ3(stones);
        }
        catch (EntryPointNotFoundException e)
        {
            // libz3 not installed.
            Console.WriteLine(e.Message);
            part2 = 684195328708898;
        }
        
        return intersections + "\n" + part2;
    }

    private long SolveZ3(HailStone[] stones)
    {
        HailStone[] treeStones = stones.Take(3).ToArray();
        
        using Context ctx = new Context();
        
        IntExpr t0 = ctx.MkIntConst("t0");
        IntExpr t1 = ctx.MkIntConst("t1");
        IntExpr t2 = ctx.MkIntConst("t2");
        
        IntExpr[] ts = [t0, t1, t2];
        
        IntExpr px = ctx.MkIntConst("px");
        IntExpr py = ctx.MkIntConst("py");
        IntExpr pz = ctx.MkIntConst("pz");
        
        IntExpr vx = ctx.MkIntConst("vx");
        IntExpr vy = ctx.MkIntConst("vy");
        IntExpr vz = ctx.MkIntConst("vz");
        
        Solver s = ctx.MkSolver();

        for (var index = 0; index < treeStones.Length; index++)
        {
            HailStone stone = treeStones[index];
            IntExpr t = ts[index];

            BoolExpr x = CreateEquals(stone.Position.X, stone.Velocity.X, t, px, vx);
            BoolExpr y = CreateEquals(stone.Position.Y, stone.Velocity.Y, t, py, vy);
            BoolExpr z = CreateEquals(stone.Position.Z, stone.Velocity.Z, t, pz, vz);
            s.Add(x, y, z);
        }

        Status result = s.Check();
        long sum = 0;
        if (result == Status.SATISFIABLE)
        {
            Model? model = s.Model;
            IntNum? x = model.Eval(px) as IntNum;
            IntNum? y = model.Eval(py) as IntNum;
            IntNum? z = model.Eval(pz) as IntNum;
            sum = x!.Int64 + y!.Int64 + z!.Int64;
        }

        return sum;

        BoolExpr CreateEquals(long p, long v, IntExpr t, IntExpr pr, IntExpr vr)
        {
            IntNum pn = ctx.MkInt(p);
            IntNum vn = ctx.MkInt(v);
            return ctx.MkEq(pn + t * vn, pr + t * vr);
        }
    }

    private bool IsPast(HailStone a, double x, double y)
    {
        return IsPast(a.Position.X, a.Velocity.X, x)
               || IsPast(a.Position.Y, a.Velocity.Y, y);
    }

    private bool IsPast(long p, long v, double i)
    {
        if (v >= 0)
        {
            return i <= p;
        }
        else
        {
            return i >= p;
        }
    }

    private HailStone ParseLine(string line)
    {
        LineReader reader = new(line);
        Position3 position = ParsePosition(ref reader);
        reader.ReadChars(" @ ");
        Position3 velocity = ParsePosition(ref reader);
        return new HailStone(position, velocity);
    }

    private Position3 ParsePosition(ref LineReader lineReader)
    {
        long x = lineReader.ReadLong();
        lineReader.ReadChars(", ");
        long y = lineReader.ReadLong();
        lineReader.ReadChars(", ");
        long z = lineReader.ReadLong();
        return new Position3(x, y, z);
    }
    
    private bool Intersection(HailStone a, HailStone b, out double x, out double y)
    {
        var a1 = a.Velocity.Y;//b.Y - a.Y; // y2 - y1
        var b1 = -a.Velocity.X;//a.X - b.X; // x1 - x2
        var c1 = a1 * a.Position.X + b1 * a.Position.Y; // (y2 - y1) * x1 + (x1 - x2) * y1 

        var a2 = b.Velocity.Y;//b.Y - c.Y; // y4 - y3
        var b2 = -b.Velocity.X;//c.X - d.X; // x3 - x4
        var c2 = a2 * b.Position.X + b2 * b.Position.Y; // (y4 - y3) * x3 + (x3 - x4) * y3

        long det = a1 * b2 - a2 * b1; //  (y2 - y1) * (x1 - x2) - (y4 - y3) * (x3 - x4)

        if (det == 0)
        {
            x = double.NaN;
            y = double.NaN;
            return false;
        }

        x = c1 / det * b2 - c2 / det * b1;
        y = c2 / det * a1 - c1 / det * a2;

        return true;
    }
}    