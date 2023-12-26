using System.Diagnostics;

namespace AdventOfCode.Year2021.Day22;

[DayInfo(2021, 22)]
public class Solution22 : Solution
{
    public class Cube
    {
        public bool On { get; set; }
        public int Xmin { get; set; }
        public int Xmax { get; set; }
        public int Ymin { get; set; }
        public int Ymax { get; set; }
        public int Zmin { get; set; }
        public int Zmax { get; set; }

        public Cube Clone()
        {
            return (Cube)MemberwiseClone();
        }

        public static Cube Create(int x, int y, int z, int radius)
        {
            return new Cube()
            {
                On = true,
                Xmin = x - radius,
                Xmax = x + radius,
                Ymin = y - radius,
                Ymax = y + radius,
                Zmin = z - radius,
                Zmax = z + radius,
            };
        }

        public long Count()
        {
            long xCount = (Xmax - Xmin + 1);
            long yCount = (Ymax - Ymin + 1);
            long zCount = (Zmax - Zmin + 1);
            return xCount * yCount * zCount;
        }

        public static Cube Parse(string line)
        {
            var split = line.Split();
            bool on = split[0] == "on";
            var ranges = split[1].Split(',');
            var xs = ranges[0].Substring(2).Split("..").Select(x => int.Parse(x)).ToList();
            var ys = ranges[1].Substring(2).Split("..").Select(x => int.Parse(x)).ToList();
            var zs = ranges[2].Substring(2).Split("..").Select(x => int.Parse(x)).ToList();

            return new Cube()
            {
                On = on,
                Xmin = xs[0],
                Xmax = xs[1],
                Ymin = ys[0],
                Ymax = ys[1],
                Zmin = zs[0],
                Zmax = zs[1],
            };
        } 

        public override string ToString()
        {
            return $"X={Xmin}..{Xmax},Y={Ymin}..{Ymax},Z={Zmin}..{Zmax}";
        }
    }
    
    private const int OFFSET = 50;
    private bool[,,] _grid = new bool[101, 101, 101];
    private List<Cube> _cubes;
    private long _onCount = 0;
    public string Run()
    {
        string[] lines = this.ReadLines();

        ParseLines(lines[0..20]);
        
        ApplyCubes();

        ParseLines(lines);
        long B = PreventOverlap();
        
        return _onCount + "\n"+ B; 
    }
    
    private long PreventOverlap()
    {
        List<Cube> results = new();
        for (int i = 0; i < _cubes.Count; i++)
        {
            Cube current = _cubes[i];
            List<Cube> other = _cubes.GetRange(i + 1, _cubes.Count - i - 1);
            var split = Alg(current, other);
            results.AddRange(split);
        }
        
        long count = Count(results);

        int off = 0;
        foreach (Cube result in results)
        {
            if (!result.On)
            {
                off++;
            }
        }

        return count;
    }

    private static List<Cube> Alg(Cube a, List<Cube> collisions)
    {
        Queue<Cube> fromA = new();
        fromA.Enqueue(a);
        List<Cube> result = new();
        while (fromA.Count > 0)
        {
            var cube = fromA.Dequeue();
            bool noCollision = true;
            foreach (var test in collisions)
            {
                bool isSplit = CollidesCore(cube, test, out List<Cube> splitCubes);
                if (isSplit)
                {
                    noCollision = false;
                    foreach (Cube newCube in splitCubes)
                    {
                        fromA.Enqueue(newCube);
                    }
                    break;
                }
            }

            if (noCollision)
            {
                result.Add(cube);
            }
        }
        
        return result;
    }

    private static long Count(List<Cube> cubes)
    {
        long total = 0;
        foreach (var cube in cubes)
        {
            if(!cube.On) continue;

            long cubeCount = cube.Count();
            
            total += cubeCount;
        }

        return total;
    }

    private static bool CollidesCore(Cube a, Cube b, out List<Cube> newCubes)
    {
        bool inRange = InRange(a, b, c => c.Xmin, c => c.Xmax) &&
                       InRange(a, b, c => c.Ymin, c => c.Ymax) &&
                       InRange(a, b, c => c.Zmin, c => c.Zmax);
        if (!inRange)
        {
            newCubes = new List<Cube>();
            return false;
        }
        
        //long beforeCount = Count(new List<Cube> { a });

        List<Cube> result = new();
        var outX = CollidesGeneral(a, b, c => c.Xmin, c => c.Xmax, (c, v) => c.Xmin = v, (c, v) => c.Xmax = v);
        foreach (Cube x in outX)
        {
            if (!InRange(x, b, c => c.Xmin, c => c.Xmax))
            {
                result.Add(x);
                continue;
            }
            var outY = CollidesGeneral(x, b, c => c.Ymin, c => c.Ymax, (c, v) => c.Ymin = v, (c, v) => c.Ymax = v);
            foreach (var y in outY)
            {
                if (!InRange(y, b, c => c.Ymin, c => c.Ymax))
                {
                    result.Add(y);
                    continue;
                }
                var outZ = CollidesGeneral(y, b, c => c.Zmin, c => c.Zmax, (c, v) => c.Zmin = v, (c, v) => c.Zmax = v);
                result.AddRange(outZ);
            }
        }
        
        // long afterCount = Count(result);
        //
        // if (beforeCount != afterCount)
        // {
        //     Console.WriteLine("Error");
        // }

        List<Cube> resultNotSame = new();
        foreach (Cube r in result)
        {
            bool inside = InRange(r, b, c => c.Xmin, c => c.Xmax) &&
                           InRange(r, b, c => c.Ymin, c => c.Ymax) &&
                           InRange(r, b, c => c.Zmin, c => c.Zmax);
            if (!inside)
            {
                resultNotSame.Add(r);
            }
        }

        newCubes = resultNotSame;
        return true;
    }

    private static bool InRange(Cube a, Cube b, Func<Cube, int> getMin, Func<Cube, int> getMax)
    {
        int aMin = getMin(a);
        int aMax = getMax(a);
        int bMin = getMin(b);
        int bMax = getMax(b);

        return !(bMax < aMin || bMin > aMax);
    }

    private static List<Cube> CollidesGeneral(Cube a, Cube b, Func<Cube, int> getMin, Func<Cube, int> getMax, Action<Cube, int> setMin, Action<Cube, int> setMax)
    {
        List<Cube> output = new();
        
        int aMin = getMin(a);
        int aMax = getMax(a);
        int bMin = getMin(b);
        int bMax = getMax(b);
        
        if (bMin > aMin && bMax < aMax)
        {
            // B entirely inside A
            // split A in 3
            
            var a1 = a.Clone();
            setMax(a1, bMin - 1); 
            output.Add(a1); // aMin --- bMin-1
            
            var a0 = a.Clone();
            setMin(a0, bMin);
            setMax(a0, bMax);
            output.Add(a0); // bMin --- bMax

            var a2 = a.Clone();
            setMin(a2, bMax + 1);
            output.Add(a2); // bMax+1 --- aMax
        }
        else if (bMin <= aMin && bMax >= aMax)
        {
            // B covers A entirely, dont split
            output.Add(a.Clone());
        }
        else if (bMin > aMin && bMin <= aMax && bMax >= aMax)
        {
            // B start in between A, B end after A
            var a1 = a.Clone();
            setMax(a1, bMin - 1);
            output.Add(a1); // aMin -- bMin-1
            
            var a0 = a.Clone();
            setMin(a0, bMin);
            output.Add(a0); // bMin -- aMax
        }
        else if(bMin <= aMin && bMax >= aMin && bMax < aMax)
        {
            // B start before A, B end in between A
            
            var a0 = a.Clone();
            setMax(a0, bMax);
            output.Add(a0); // aMin --- bMax
            
            var a1 = a.Clone();
            setMin(a1, bMax + 1);
            output.Add(a1); 
        }
        else
        {
            // no overlap, 
            Debug.Fail("Should not happen");
            output.Add(a.Clone());
        }

        return output;
    }
    
    private void ApplyCubes()
    {
        foreach (var cube in _cubes)
        {
            ApplyCube(cube);
        }
    }

    private void ApplyCube(Cube cube)
    {
        for (int x = cube.Xmin; x <= cube.Xmax; x++)
        {
            for (int y = cube.Ymin; y <= cube.Ymax; y++)
            {
                for (int z = cube.Zmin; z <= cube.Zmax; z++)
                {
                    if (cube.On)
                    {
                        if (!GetSet(x, y, z))
                        {
                            _onCount++;
                        }
                    }
                    else
                    {
                        if (!GetClear(x, y, z))
                        {
                            _onCount--;
                        }
                    }
                }
            }
        }
    }
    
    private bool GetClear(int x, int y, int z)
    {
        bool wasClear = !Get(x, y, z);
        Set(false, x, y, z);
        return wasClear;
    }

    private bool GetSet(int x, int y, int z)
    {
        bool wasSet = Get(x, y, z);
        Set(true, x, y, z);
        return wasSet;
    }

    private void Set(bool b, int x, int y, int z)
    {
        _grid[x + OFFSET, y + OFFSET, z + OFFSET] = b;
    }
    
    private bool Get(int x, int y, int z)
    {
        return _grid[x + OFFSET, y + OFFSET, z + OFFSET];
    }

    private bool InRange(int x, int y, int z)
    {
        return x is >= -50 and <= 50 && 
               y is >= -50 and <= 50 && 
               z is >= -50 and <= 50;
    }

    private void ParseLines(string[] lines)
    {        
        _cubes = new();
        foreach (var line in lines)
        {
            _cubes.Add(Cube.Parse(line));
        }
    }
}

