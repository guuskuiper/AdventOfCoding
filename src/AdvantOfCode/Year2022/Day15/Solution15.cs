using System.Drawing;

namespace AdventOfCode.Year2022.Day15;

public class Solution15 : Solution
{
    private record Sensor(Point Location, Point Beacon);
    
    public string Run()
    {
        string example = """ 
Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3
""";
        var lines = InputReader.ReadFileLinesArray();
        //var lines = example.Split("\r\n");
        List<Sensor> sensors = Parse(lines);

        int xMin = int.MaxValue;
        int xMax = int.MinValue;
        for (int s = 0; s < sensors.Count; s++)
        {
            int x = sensors[s].Location.X;
            int distance = Distance(sensors[s]);
            xMin = Math.Min(xMin, x - distance);
            xMax = Math.Max(xMax, x + distance);
        }

        int count = CountY(sensors, 2000000, xMin, xMax);
        
        return count + "\n";
    }

    private int CountY(List<Sensor> sensors, int yTarget, int xMin, int xMax)
    {
        List<int> distances = sensors.Select(Distance).ToList();
        HashSet<Point> beacons = new(sensors.Select(x => x.Beacon));

        int count = 0;
        for(int x = xMin; x <= xMax; x++)
        {
            bool beaconPossible = true;
            Point current = new Point(x, yTarget);
            
            if(beacons.Contains(current)) continue;
            
            for (var index = 0; index < sensors.Count; index++)
            {
                Sensor sensor = sensors[index];
                int distanceCurrentSensor = Distance(current, sensor.Location);
                int distanceSensorBeacon = distances[index];
                if (distanceCurrentSensor <= distanceSensorBeacon)
                {
                    beaconPossible = false;
                    break;
                }
            }

            if (!beaconPossible) count++;
        }
        
        return count;
    }

    private int Distance(Sensor s) => Distance(s.Location, s.Beacon);

    private int Distance(Point a, Point b)
    {
        int dx = b.X - a.X;
        int dy = b.Y - a.Y;
        return Math.Abs(dx) + Math.Abs(dy);
    }

    private List<Sensor> Parse(IEnumerable<string> lines)
    {
        List<Sensor> sensors = new();
        foreach (var line in lines)
        {
            sensors.Add(Parse(line));
        }

        return sensors;
    }

    private Sensor Parse(ReadOnlySpan<char> line)
    {
        const string SensorAtX = "Sensor at x=";
        const string BeacontX = ": closest beacon is at x=";

        LineReader reader = new LineReader(line);
        reader.Skip(SensorAtX.Length);
        int xs = reader.ReadInt();
        reader.Skip(", y=".Length);
        int ys = reader.ReadInt();
        
        reader.Skip(BeacontX.Length);
        int xb = reader.ReadInt();
        reader.Skip(", y=".Length);
        int yb = reader.ReadInt();
        
        return new Sensor(new Point(xs, ys), new Point(xb, yb));
    }
    
    
}
