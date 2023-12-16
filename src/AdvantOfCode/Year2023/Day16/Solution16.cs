using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day16;

[DayInfo(2023, 16)]
public class Solution16 : Solution
{
    private Size North = new(0, -1);
    private Size West = new(-1, 0);
    private Size South = new(0, 1);
    private Size East = new(1, 0);
    
    private char[,] grid;
    
    public string Run()
    {
        string[] input = this.ReadLines();
        grid = input.ToGrid();
        int sum = RunBeam(new Beam(new (0,0), new(1, 0)));

        List<Beam> starts = [];
        for (int y = 0; y < grid.Heigth(); y++)
        {
            Beam beam = new Beam(new(0, y), East);
            Beam beamReverse = new Beam(new(grid.Width()-1, y), West);
            starts.Add(beam);
            starts.Add(beamReverse);
        }
        for (int x = 0; x < grid.Width(); x++)
        {
            Beam beam = new(new(x, 0), South);
            Beam beamReverse = new(new(x, grid.Heigth()-1), North);
            starts.Add(beam);
            starts.Add(beamReverse);
        }

        int max = starts.Select(RunBeam).Max();
        
        return sum + "\n" + max;
    }

    private record Beam(Point Position, Size Direction);
    private int RunBeam(Beam startBeam)
    {
        bool[,] energy = new bool[grid.Width(), grid.Heigth()];

        energy[startBeam.Position.X, startBeam.Position.Y] = true;

        Dictionary<Beam, Beam> path = new()
        {
            [startBeam] = startBeam
        };
        AQueue<Beam> frontier = new();

        frontier.Add(startBeam);

        while (!frontier.Empty)
        {
            Beam current = frontier.Get();

            foreach (var split in Next(current, grid[current.Position.X, current.Position.Y]))
            {
                if(path.ContainsKey(split)) continue;
                
                if (InRange(split.Position))
                {
                    frontier.Add(split);
                    path[split] = current;
                    energy[split.Position.X, split.Position.Y] = true;
                }
            }
        }

        int sum = 0;
        for (int x = 0; x < energy.Width(); x++)
        {
            for (int y = 0; y < energy.Heigth(); y++)
            {
                sum += energy[x, y] ? 1 : 0;
            }
        }

        return sum;
    }

    private IEnumerable<Beam> Next(Beam beam, char current)
    {
        Size[] directions = (beam.Direction, current) switch
        {
            (_, '.') => [beam.Direction],
            (_, '\\') => [new Size(beam.Direction.Height, beam.Direction.Width)],
            (_, '/') => [new Size(-beam.Direction.Height, -beam.Direction.Width)],
            ( {Height: 0}, '-') => [beam.Direction],
            ( {Height: not 0}, '-') => [West, East],
            ( {Width: 0}, '|') => [beam.Direction],
            ( {Width: not 0}, '|') => [North, South],
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var direction in directions)
        {
            Point newPos = beam.Position + direction;
            yield return new Beam(newPos, direction);
        }
    }
    
    bool InRange (Point p) => p.X >= 0 && p.X < grid.Width() &&
                              p.Y >= 0 && p.Y < grid.Heigth();
}    