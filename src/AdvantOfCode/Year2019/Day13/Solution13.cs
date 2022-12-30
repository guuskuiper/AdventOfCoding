using System.Runtime.CompilerServices;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day13;

[DayInfo(2019, 13)]
public class Solution13 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        
        var instructions = input[0].Split(',').Select(long.Parse).ToArray();
            
        var arcade = new Arcade(instructions);
        var blockCount = arcade.Start();

        var commands = Commands().Select(int.Parse).ToList();
        arcade = new Arcade(instructions, commands);
        arcade.Start();
        int scoreB = arcade.Score;

        return blockCount + "\n" + scoreB;
    }

    private IEnumerable<string> Commands()
    {
        string path = CurrentPath();
        FileInfo file = new FileInfo(path);
        string commandPath = Path.Combine(file.Directory.FullName, "commands.txt");
        return File.ReadAllLines(commandPath);
    }
    private string CurrentPath([CallerFilePath] string path = "") => path;
}    