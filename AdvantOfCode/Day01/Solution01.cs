namespace AdventOfCode.Day01;

public class Solution01 : Solution
{
    private List<int> numbers = new List<int>();

    public string Run()
    {
        string input = File.ReadAllText("Day01/input01.txt");
        string[] inputs = input.Split('\n');

        foreach(var i in inputs)
        {
            if(string.IsNullOrEmpty(i)) continue;
            int n = int.Parse(i);
            numbers.Add(n);
        }

      

        int increases = 0;
        {
            int prev = numbers[0];
            for(int i = 1; i < numbers.Count; i++)
            {
                int c = numbers[i];
                if(c > prev) increases++;
                prev = c;
            }
        }


        int increasesWindow = 0;
        {
            int prevWindow = GetWindowSum(2);
            for(int i = 3; i < numbers.Count; i++)
            {
                int c = GetWindowSum(i);
                if(c > prevWindow) increasesWindow++;
                prevWindow = c;
            }
        }

        return increases.ToString() + "," + increasesWindow.ToString() ;
    }

    private int GetWindowSum(int i)
    {
        return numbers[i] + numbers[i - 1] + numbers[i - 2]; 
    }
}
