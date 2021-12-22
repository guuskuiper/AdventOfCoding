using System.Diagnostics;

namespace AdventOfCode.Day22;

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
            // return new Cube()
            // {
            //     On = On,
            //     Xmin = Xmin,
            //     Xmax = Xmax,
            //     Ymin = Ymin,
            //     Y
            // };
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
    private List<Cube> _newCubes = new ();
    private long _onCount = 0;
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        ParseLines(lines[..20]);
        ApplyCubes();

        PreventOverlap();
        long B = Count();
        
        return _onCount + "\n"+ B;
    }

    private void ApplyCubes()
    {
        foreach (var cube in _cubes)
        {
            ApplyCube(cube);
        }
    }

    private void PreventOverlap()
    {
        for (var i = 0; i < _cubes.Count; i++)
        {
            var a = _cubes[i];
            bool remove = false;
            for (var j = i + 1; j < _cubes.Count; j++)
            {
                var b = _cubes[j];
                if(Collides(a, b))
                {
                    // will be removed
                    // so continue to the next a cube
                    remove = true;
                    break;
                }
            }

            if (!remove)
            {
                _newCubes.Add(a);
            }
        }
    }

    private long Count()
    {
        long total = 0;
        foreach (var cube in _newCubes)
        {
            if(!cube.On) continue;

            long cubeCount = (cube.Xmax - cube.Xmin + 1) *
                             (cube.Ymax - cube.Ymin + 1) *
                             (cube.Zmax - cube.Zmin + 1);
            total += cubeCount;
        }

        return total;
    }

    private bool Collides(Cube a, Cube b)
    {
        bool remove = false;

        bool inRange = InRange(a, b, c => c.Xmin, c => c.Xmax) &&
                       InRange(a, b, c => c.Ymin, c => c.Ymax) &&
                       InRange(a, b, c => c.Zmin, c => c.Zmax);
        if (!inRange) return false;
            
        remove |= CollidesGeneral(a, b, c => c.Xmin, c => c.Xmax, (c, v) => c.Xmin = v, (c, v) => c.Xmax = v);
        remove |= CollidesGeneral(a, b, c => c.Ymin, c => c.Ymax, (c, v) => c.Ymin = v, (c, v) => c.Ymax = v);
        remove |= CollidesGeneral(a, b, c => c.Zmin, c => c.Zmax, (c, v) => c.Zmin = v, (c, v) => c.Zmax = v);
        return remove;
    }

    private bool InRange(Cube a, Cube b, Func<Cube, int> getMin, Func<Cube, int> getMax)
    {
        int aMin = getMin(a);
        int aMax = getMax(a);
        int bMin = getMin(b);
        int bMax = getMax(b);

        return !(bMax < aMin || bMin > aMax);
    }

    private bool CollidesGeneral(Cube a, Cube b, Func<Cube, int> getMin, Func<Cube, int> getMax, Action<Cube, int> setMin, Action<Cube, int> setMax)
    {
        bool add = true;
        
        int aMin = getMin(a);
        int aMax = getMax(a);
        int bMin = getMin(b);
        int bMax = getMax(b);
        
        if (bMin > aMin && bMax < aMax)
        {
            // split A in 2

            var a2 = a.Clone();
            setMin(a2, bMax + 1);
            _newCubes.Add(a2);
            
            setMax(a, bMin - 1);
        }
        else if (bMin < aMin && bMax > aMax)
        {
            // remove A
            //_cubes.Remove(a);
            add = false;
        }
        else if (bMin > aMin && bMin < aMax && bMax > aMax)
        {
            setMin(a, bMin + 1);
        }
        else if(bMin < aMin && bMax > aMin && bMax < aMax)
        {
            setMax(a, bMax - 1);
        }
        
        return add;
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
        return x >= -50 && x <= 50 &&
               y >= -50 && y <= 50 &&
               z >= -50 && z <= 50;
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

