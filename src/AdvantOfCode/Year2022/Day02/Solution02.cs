using System.Collections.Generic;

namespace AdventOfCode.Year2022.Day02;

public enum ERockPaperScissors
{
    Rock = 1,
    Paper = 2,
    Scissors = 3,
};

public static class ERockPaperScissorsExtentions
{
    public static ERockPaperScissors Win(this ERockPaperScissors shape) => shape switch
    {
        ERockPaperScissors.Rock => ERockPaperScissors.Paper,
        ERockPaperScissors.Paper => ERockPaperScissors.Scissors,
        ERockPaperScissors.Scissors => ERockPaperScissors.Rock,
        _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
    };

    public static ERockPaperScissors Lose(this ERockPaperScissors shape) => shape switch
    {
        ERockPaperScissors.Rock => ERockPaperScissors.Scissors,
        ERockPaperScissors.Paper => ERockPaperScissors.Rock,
        ERockPaperScissors.Scissors => ERockPaperScissors.Paper,
        _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
    };
}

public class Solution02 : Solution
{
    private record Round(ERockPaperScissors Opponent, ERockPaperScissors Response);

    public string Run()
    {
        List<string> lines = InputReader.ReadFileLines();
        IEnumerable<Round> rounds = Parse(lines);

        long score = Scores(rounds);

        IEnumerable<Round> changedRounds = ChangeRounds(rounds);
        long scoresChanged = Scores(changedRounds);

        return score + "\n" + scoresChanged;
    }

    private IEnumerable<Round> Parse(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            string[] split = line.Split(' ');
            ERockPaperScissors opponent = Parse(split[0]);
            ERockPaperScissors response = Parse(split[1]);
            yield return new Round(opponent, response);
        }
    }

    private long Scores(IEnumerable<Round> rounds)
    {
        long totalScore = 0;
        foreach (var round in rounds)
        {
            totalScore += Score(round);
        }

        return totalScore;
    }

    private long Score(Round round)
    {
        long shapeScore = (int)round.Response;

        long outcome;
        if (round.Response == round.Opponent)
        {
            outcome = 3;
        }
        else if(round.Response == round.Opponent.Win())
        {
            outcome = 6;
        }
        else
        {
            outcome = 0;
        }

        return shapeScore + outcome;
    }

    private IEnumerable<Round> ChangeRounds(IEnumerable<Round> rounds)
    {
        foreach (var round in rounds)
        {
            ERockPaperScissors response = ChangeRound(round);
            yield return round with { Response = response };
        }
    }

    private ERockPaperScissors ChangeRound(Round round)
    {
        ERockPaperScissors newResponse = round.Response switch
        {
            //X means you need to lose,
            ERockPaperScissors.Rock => round.Opponent.Lose(),
            //Y means you need to end the round in a draw, and
            ERockPaperScissors.Paper => round.Opponent,
            //Z means you need to win."
            ERockPaperScissors.Scissors => round.Opponent.Win(),
            _ => throw new ArgumentOutOfRangeException()
        };

        return newResponse;
    }

    private ERockPaperScissors Parse(string c) =>
        c switch
        {
            "A" or "X" => ERockPaperScissors.Rock,
            "B" or "Y" => ERockPaperScissors.Paper,
            "C" or "Z" => ERockPaperScissors.Scissors,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
}
