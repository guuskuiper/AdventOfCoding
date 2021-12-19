using System.Numerics;

namespace AdventOfCode.Day19;

public class Solution19 : Solution
{
    public struct Vector3I : IEquatable<Vector3I>
    {
        public Vector3I(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; set; }
        public int Y { get; set; }   
        public int Z { get; set; }   
        
        public static Vector3I operator +(Vector3I a, Vector3I b) => new (a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3I operator -(Vector3I a, Vector3I b) => new (a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static bool operator ==(Vector3I a, Vector3I b) => a.Equals(b);
        public static bool operator !=(Vector3I a, Vector3I b) => !(a == b);

        public override bool Equals(object? obj) => obj is Vector3I v && Equals(v);
        
        public bool Equals(Vector3I v)
        {
            return X == v.X && Y == v.Y && Z == v.Z;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override string ToString()
        {
            return $"<{X},{Y},{Z}>";
        }
    }
    
    public class ScannerReports
    {
        public int Id { get; set; }
        public List<Vector3I> Beacons { get; set; } = new();

        public bool Found;
        public Vector3I Offset;
    }

    private List<(Axis, Rotation)> _orientations;
    private List<ScannerReports> _scanners;

    public Solution19()
    {
        //var lines = InputReader.ReadFileLines(@"C:\Users\zane\git\AdventOfCoding\src\AdvantOfCode\Day19\example19.txt");
        var lines = InputReader.ReadFileLines();
        _scanners = ParseLines(lines);
        InitializeOrientations();
    }
    
    public string Run()
    {

        //Combination(new Vector3I(1, 2, 3));
      
        // Test();
        // return "";

        ScannerReports reference = _scanners[0];
        reference.Found = true;

        Queue<int> toCheckIndices = new Queue<int>();
        toCheckIndices.Enqueue(0);

        while (toCheckIndices.Count > 0)
        {
            int index = toCheckIndices.Dequeue();
            
            for (int j = 0; j < _scanners.Count; j++)
            {
                var scanner = _scanners[j];
                
                if(scanner.Found) continue;
                
                bool match = MatchScanners(_scanners[index], _scanners[j]);
                if (match)
                {
                    toCheckIndices.Enqueue(j);
                }
            }
        }

        HashSet<Vector3I> beacons = new();
        foreach (var scanner in _scanners)
        {
            foreach (var beacon in scanner.Beacons)
            {
                beacons.Add(beacon);
            }
        }

        long A = beacons.Count;

        // Console.WriteLine(FromAxis(new Vector3I(1, 2, 3), Axis.ZNeg));
        //Console.WriteLine(Variation(new Vector3I(1, 2, 3), Axis.zPos, Rotation.none));

        long B = GetMaxDistance();
        
        
        return A + "\n" + B;
    }

    private long GetMaxDistance()
    {
        long maxDistance = 0;

        for (int i = 0; i < _scanners.Count; i++)
        {
            for (int j = 0; j < _scanners.Count; j++)
            {
                var offsetI = _scanners[i].Offset;
                var offsetJ = _scanners[j].Offset;
                long distance = Math.Abs(offsetI.X - offsetJ.X) + 
                                Math.Abs(offsetI.Y - offsetJ.Y) +
                                Math.Abs(offsetI.Z - offsetJ.Z);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
            }
        }

        return maxDistance;
    }

    private bool MatchScanners(ScannerReports reference, ScannerReports other, int mimimumCount = 12)
    {
        foreach (var (axis, rotation) in _orientations)
        {
            List<Vector3I> transformedBeacons = new();
            foreach (var otherBeacon in other.Beacons)
            {
                transformedBeacons.Add(Variation(otherBeacon, axis, rotation));
            }

            bool match = FindMatches(reference.Beacons, transformedBeacons, mimimumCount, out Vector3I offset);

            if (match)
            {
                Console.WriteLine($"Match {reference.Id} - {other.Id} => {axis} {rotation} {offset}");
                other.Offset = offset;
                other.Beacons = transformedBeacons.Select(x => x + offset).ToList();
                other.Found = true;
                return true;
            }
        }

        return false;
    }

    private bool FindMatches(List<Vector3I> source, List<Vector3I> other, int minimumCount, out Vector3I offset)
    {
        foreach (var s in source)
        {
            foreach (var o in other)
            {
                Vector3I tryOffset = s - o;
                int count = 0;

                foreach (var ts in source)
                {
                    foreach (var to in other)
                    {
                        if (ts - to == tryOffset)
                        {
                            count++;
                        }
                    }
                }

                if (count >= minimumCount)
                {
                    offset = tryOffset;
                    return true;
                }
            }
        }
        
        offset = new Vector3I();
        return false;
    }

    private void InitializeOrientations()
    {
        _orientations = new List<(Axis, Rotation)>(24);
        foreach (var axisNumber in Enumerable.Range(0, 6))
        {
            Axis axis = (Axis)axisNumber;
            foreach (var upNumber in Enumerable.Range(0, 4))
            {
                Rotation rotation = (Rotation)upNumber;
                _orientations.Add((axis, rotation));
            }
        }
    }

    private List<ScannerReports> ParseLines(List<string> lines)
    {
        List<ScannerReports> scanners = new();
        List<string> scannerLine = new();
        foreach (var line in lines)
        {
            if (line.StartsWith("---") && scannerLine.Count > 0)
            {
                scanners.Add(ParseScanner(scannerLine));
                scannerLine.Clear();
            }
            scannerLine.Add(line);
        }

        if (scannerLine.Count > 0)
        {
            scanners.Add(ParseScanner(scannerLine));
        }

        return scanners;
    }

    public Vector3I Variation(Vector3I v, Axis axis, Rotation rotation)
    {
        return FromRotation(FromAxis(v, axis), rotation);
    }

    public enum Axis
    {
        xPos,
        xNeg,
        yPos,
        yNeg,
        zPos,
        ZNeg,
    }

    public enum Rotation
    {
        none,
        right,
        down,
        left,
    }

    public Vector3I FromAxis(Vector3I v, Axis axis)
        => axis switch
        {
            Axis.xPos => new Vector3I(v.Y, v.Z, v.X),
            Axis.xNeg => new Vector3I(-v.Y, v.Z, -v.X),
            Axis.yPos => new Vector3I(-v.X, v.Z, v.Y),
            Axis.yNeg => new Vector3I(v.X, v.Z, -v.Y),
            Axis.zPos => new Vector3I(v.X, v.Y, v.Z),
            Axis.ZNeg => new Vector3I(v.X, -v.Y, -v.Z),
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };

    public Vector3I FromRotation(Vector3I v, Rotation r)
        => r switch
        {
            Rotation.none => v,
            Rotation.right => Rotate90(v),
            Rotation.down => Rotate180(v),
            Rotation.left => Rotate270(v),
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
        };

    public Vector3I Rotate270(Vector3I v)
        => Rotate90(Rotate90(Rotate90(v)));
    public Vector3I Rotate180(Vector3I v)
        => Rotate90(Rotate90(v));

    public Vector3I Rotate90(Vector3I v)
        => new (-v.Y, v.X, v.Z);

    private ScannerReports ParseScanner(List<string> lines)
    {
        ScannerReports scannerReports = new ScannerReports();
        scannerReports.Id = int.Parse(lines[0].Split()[2]);

        foreach (var line in lines.Skip(1))
        {
            var numbers = line.Split(',').Select(int.Parse).ToArray();
            Vector3I beacon = new Vector3I(numbers[0], numbers[1], numbers[2]);
            
            scannerReports.Beacons.Add(beacon);
        }

        return scannerReports;
    }
}
