namespace AdventOfCode.Year2021.Day19;

[DayInfo(2021, 19)]
public class Solution19 : Solution
{
    public enum Axis
    {
        xPos,
        xNeg,
        yPos,
        yNeg,
        zPos,
        zNeg,
    }

    public enum Rotation
    {
        none,
        right,
        down,
        left,
    }
    
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
        public static long ManhattanDistance(Vector3I a, Vector3I b) => 
            Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);

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
        public HashSet<Vector3I> Beacons { get; set; } = new();
        public bool Found;
        public Vector3I Offset;
    }

    private List<(Axis, Rotation)> _orientations;
    private List<ScannerReports> _scanners;

    public Solution19()
    {
        string[] lines = this.ReadLines();
        _scanners = ParseLines(lines);
        InitializeOrientations();
    }
    
    public string Run()
    {
        MatchAllScanners();

        long A = GetBeaconCount();
        long B = GetMaxDistance();
        
        return A + "\n" + B;
    }
    
    private void MatchAllScanners()
    {
        ScannerReports start = _scanners[0];
        start.Found = true;

        Queue<ScannerReports> referenceQueue = new();
        referenceQueue.Enqueue(start);

        while (referenceQueue.Count > 0)
        {
            var reference = referenceQueue.Dequeue();

            foreach (var scanner in _scanners)
            {
                if (scanner.Found) continue;

                bool match = MatchScanners(reference, scanner);
                if (match)
                {
                    referenceQueue.Enqueue(scanner);
                }
            }
        }
    }
    
    private long GetBeaconCount()
    {
        HashSet<Vector3I> beacons = new();
        foreach (var scanner in _scanners)
        {
            beacons.UnionWith(scanner.Beacons);
        }

        return beacons.Count;
    }

    private long GetMaxDistance()
    {
        long maxDistance = 0;

        foreach (var a in _scanners)
        {
            foreach (var b in _scanners)
            {
                long distance = Vector3I.ManhattanDistance(a.Offset, b.Offset);

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
            HashSet<Vector3I> transformedBeacons = new();
            foreach (var otherBeacon in other.Beacons)
            {
                transformedBeacons.Add(Variation(otherBeacon, axis, rotation));
            }
            
            bool match = FindMatchesSets(reference.Beacons, transformedBeacons, mimimumCount, out Vector3I offset);

            if (match)
            {
                //Console.WriteLine($"Match {reference.Id} - {other.Id} => {axis} {rotation} {offset}");
                other.Offset = offset;
                other.Beacons = transformedBeacons.Select(x => x + offset).ToHashSet();
                other.Found = true;
                return true;
            }
        }

        return false;
    }
    
    private bool FindMatchesSets(HashSet<Vector3I> source, HashSet<Vector3I> other, int minimumCount, out Vector3I offset)
    {
        int remainingSource = source.Count;
        foreach (var s in source)
        {
            if (remainingSource < minimumCount)
            {
                // need at least minimumCount matches in the source 
                offset = new Vector3I();
                return false;
            }

            int remainingOther = other.Count;
            foreach (var o in other)
            {
                if (remainingOther < minimumCount)
                {
                    // need at least minimumCount matches
                    break;
                }
                
                Vector3I tryOffset = s - o;

                var transformed = other.Select(x => x + tryOffset).ToHashSet();
                transformed.IntersectWith(source);
                
                if (transformed.Count >= minimumCount)
                {
                    offset = tryOffset;
                    return true;
                }

                remainingOther--;
            }

            remainingSource--;
        }
        
        offset = new Vector3I();
        return false;
    }
    
    
    public Vector3I Variation(Vector3I v, Axis axis, Rotation rotation)
    {
        return FromRotation(FromAxis(v, axis), rotation);
    }

    public Vector3I FromAxis(Vector3I v, Axis axis)
        => axis switch
        {
            Axis.xPos => new Vector3I(v.Y, v.Z, v.X),
            Axis.xNeg => new Vector3I(-v.Y, v.Z, -v.X),
            Axis.yPos => new Vector3I(-v.X, v.Z, v.Y),
            Axis.yNeg => new Vector3I(v.X, v.Z, -v.Y),
            Axis.zPos => new Vector3I(v.X, v.Y, v.Z),
            Axis.zNeg => new Vector3I(v.X, -v.Y, -v.Z),
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

    private List<ScannerReports> ParseLines(string[] lines)
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
