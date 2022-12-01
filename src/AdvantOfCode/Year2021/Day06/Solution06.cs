namespace AdventOfCode.Year2021.Day06;

public class Solution06 : Solution
{
    private List<int> simulation;
    private long[] bins;
    public string Run()
    {
        string input = InputReader.ReadFile();

        simulation = input.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
        bins = Bins(simulation);

        SimulateDays(80, Simulate);
        int A = simulation.Count;

        SimulateDays(256, SimulateBin);
        long B = CountBins();
        
        return A  + "\n" + B;
    }


    private void SimulateDays(int days, Action simulate)
    {
        for (int i = 0; i < days; i++)
        {
            simulate();
        }
    }
    
    private void Simulate()
    {
        for (int i = simulation.Count - 1; i >= 0; i--)
        {
            if (simulation[i] == 0)
            {
                simulation[i] = 6;
                simulation.Add(8);
            }
            else
            {
                simulation[i]--;
            }
        }
    }
    
    private void SimulateBin()
    {
        long[] newBins = new long[9];
        for (int i = 0; i < 9; i++)
        {
            if (i == 0)
            {
                newBins[8] += bins[0];
                newBins[6] += bins[0];
            }
            else
            {
                newBins[i - 1] += bins[i];
            }
        }
        
        bins = newBins;
    }

    private long[] Bins(List<int> numbers)
    {
        long[] bins = new long[9];
        for (int i = 0; i < numbers.Count; i++)
        {
            bins[numbers[i]]++;
        }

        return bins;
    }
    
    private long CountBins()
    {
        long count = 0;
        for (int i = 0; i < bins.Length; i++)
        {
            count += bins[i];
        }

        return count;
    }
}
