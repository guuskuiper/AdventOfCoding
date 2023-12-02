using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day02;

[DayInfo(2023, 02)]
public class Solution02 : Solution
{
    private record Game(int Number, Round[] Rounds);
    private record Round(int Red, int Green, int Blue);
    
    public string Run()
    {
        string[] input = this.ReadLines();

        List<Game> games = input.Select(ParseGame).ToList();
        
        List<Game> possibleGames = PossibleGames(games);
        int numberSum = possibleGames.Sum(x => x.Number);

        int power = games.Select(Powers).Sum();
        
        return numberSum + "\n" + power;
    }

    private List<Game> PossibleGames(List<Game> games) =>
        games.Where(g => 
                g.Rounds.All(r => r is { Red: <= 12, Green: <= 13, Blue: <= 14 }))
            .ToList();

    private int Powers(Game game)
    {
        int red = game.Rounds.Max(x => x.Red);
        int blue = game.Rounds.Max(x => x.Blue);
        int green = game.Rounds.Max(x => x.Green);

        return red * blue * green;
    }

    private Game ParseGame(string line)
    {
        LineReader reader = new LineReader(line);
        reader.ReadChars("Game");
        reader.SkipWhitespaces();
        int number = reader.ReadInt();
        reader.ReadChar(':');
        reader.SkipWhitespaces();

        List<Round> rounds = [];
        while (!reader.IsDone)
        {
            Round round = ParseRound(ref reader);
            rounds.Add(round);
        }
        
        return new Game(number, rounds.ToArray());
    }

    private Round ParseRound(ref LineReader reader)
    {
        int red = 0;
        int blue = 0;
        int green = 0;

        while (true)
        {
            int number = reader.ReadInt();
            reader.SkipWhitespaces();
            ReadOnlySpan<char> letters = reader.ReadLetters();
            switch (letters)
            {
                case "red":
                    red += number;
                    break;
                case "blue":
                    blue += number;
                    break;
                case "green":
                    green += number;
                    break;
            }
            
            if(reader.IsDone) break;

            char next = reader.ReadChar();
            reader.SkipWhitespaces();
            if(next == ';') break;
        }

        return new Round(red, green, blue);
    }
}    