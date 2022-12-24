using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace AdventOfCode.Year2022.Day15;

public class Solution15 : Solution
{
    private const bool ExecuteParallel = true;
    private record Sensor(Point Location, Point Beacon);
    
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
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

        List<(Sensor, Sensor)> pairs = new List<(Sensor, Sensor)>();
        for (int i = 0; i < sensors.Count; i++)
        {
            Sensor first = sensors[i];
            int firstBeaconDistance = Distance(first);
            for (int j = i + 1; j < sensors.Count; j++)
            {
                Sensor second = sensors[j];
                int secondBeaconDistance = Distance(second);
                int sensorDistance = Distance(first.Location, second.Location);
                if (sensorDistance < (firstBeaconDistance + secondBeaconDistance + 2))
                {
                    if (firstBeaconDistance < secondBeaconDistance)
                    {
                        pairs.Add((first, second));
                    }
                    else
                    {
                        pairs.Add((second, first));
                    }
                }
            }
        }

        ConcurrentDictionary<Point, HashSet<Point>> boundaries = new();
        ConcurrentDictionary<Point, int> intersections = new();

        if (ExecuteParallel)
        {
            Parallel.ForEach(pairs, (sensorPair) =>
            {
                FindIntersections(sensorPair, boundaries, intersections);
            });
        }
        else
        {
            foreach (var sensorPair in pairs)
            {
                FindIntersections(sensorPair, boundaries, intersections);
            }
        }

        List<Point> validPoints = new();
        foreach (var (point, value) in intersections.Where(x => x.Value > 1))
        {
            bool inside = false;
            foreach (var sensor in sensors)
            {
                int distance = Distance(sensor);
                int boundDistance = Distance(point, sensor.Location);
                if (boundDistance <= distance)
                {
                    inside = true;
                }
            }

            if (!inside)
            {
                validPoints.Add(point);
            }
        }

        var target = validPoints[0];
        BigInteger tuning = Tuning(target);

        return count + "\n" + tuning;
    }

    private void FindIntersections((Sensor, Sensor) sensorPair, ConcurrentDictionary<Point, HashSet<Point>> boundaries,
        ConcurrentDictionary<Point, int> intersections)
    {
        var leftSensor = sensorPair.Item1;
        var rightSensor = sensorPair.Item2;
        HashSet<Point> leftBound;
        if (!boundaries.ContainsKey(leftSensor.Location))
        {
            int distance = Distance(leftSensor);
            int boundDistance = distance + 1;
            leftBound = GetBoundInRange(leftSensor.Location, boundDistance, 0, 4000000);
            boundaries[leftSensor.Location] = leftBound;
        }
        else
        {
            leftBound = boundaries[leftSensor.Location];
        }

        int distanceRight = Distance(rightSensor);
        int rightBound = distanceRight + 1;
        foreach (var point in leftBound)
        {
            if (Distance(point, rightSensor.Location) == rightBound)
            {
                intersections.AddOrUpdate(point, 1, (_, i) => i + 1);
            }
        }
    }

    private BigInteger Tuning(Point p) => p.X * new BigInteger(4000000) + p.Y;

    private HashSet<Point> GetBoundInRange(Point p, int distance, int min, int max)
    {
        Size[] Offset =
        {
            new (1, 1), 
            new (1, -1),
            new (-1, -1),
            new (-1, +1),
        };

        HashSet<Point> points = new HashSet<Point>();

        Point current = p + new Size(-distance, 0);

        foreach (var offset in Offset)
        {
            foreach (var _ in Enumerable.Range(0, distance))
            {
                current += offset;

                if (current.X < min || current.Y < min) continue;
                if (current.X > max || current.Y > max) continue;

                points.Add(current);
            }
        }

        return points;
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
