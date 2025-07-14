namespace AdventOfCode.Year2023.Day01;

[DayInfo(2023, 01)]
public class Solution01f : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        int sum = Functional.Year2023.Day01.solveALines(input);
        
        int sumB = Functional.Year2023.Day01.solveBLines(input);
        
        return sum + "\n" + sumB;
    }

    public async Task<string> RunAsync()
    {
        string[] input = await this.ReadLinesAsync();
        
        int sum = Functional.Year2023.Day01.solveALines(input);
        
        int sumB = Functional.Year2023.Day01.solveBLines(input);
        int sumB2 = Functional.Year2023.Day01.solveBLinesReplace(input);
        
        return sum + "\n" + sumB;
    }
}    