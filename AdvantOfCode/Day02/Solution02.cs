namespace AdventOfCode.Day02;

public class Solution02 : Solution
{
    private int x = 0;
    private int z = 0;
    private int aim = 0;

    public string Run()
    {
        string input = File.ReadAllText("Day02/input02.txt");
        List<string> lines = input.Split('\n').ToList();

        string a = Parse(lines, this.ParseLineA);
        string b = Parse(lines, this.ParseLineB);

        return a + '\n' + b;
    }

    private string Parse(List<string> lines, Action<string> func)
    {
        this.z = 0;
        this.x = 0;
        this.aim = 0;

        foreach (string line in lines)
        {
            func(line);
        }

        return (this.x * this.z).ToString();
    }

    private void ParseLineA(string line)
    {
        var split = line.Split(' ');
        if (split.Length != 2) return;
        int value = int.Parse(split[1]);

        switch (split[0])
        {
            case "forward":
                this.x += value;
                break;
            case "down":
                this.z += value;
                break;
            case "up":
                this.z -= value;
                break;
            default:
                throw new Exception();
        }

    }

    private void ParseLineB(string line)
    {
        var split = line.Split(' ');
        if (split.Length != 2) return;
        int value = int.Parse(split[1]);

        switch (split[0])
        {
            case "forward":
                this.z += this.aim * value;
                this.x += value;
                break;
            case "down":
                this.aim += value;
                break;
            case "up":
                this.aim -= value;
                break;
            default:
                throw new Exception();
        }
    }
}
