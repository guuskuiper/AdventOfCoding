using System.Collections.Generic;

namespace AdventOfCode.Year2022.Day02;

public class Solution02 : Solution
{
    private record Round(ERockPaperScissors Opponent, ERockPaperScissors Response);
    private enum ERockPaperScissors
    {
        Rock,
        Paper,
        Scissors,
    };

    public string Run()
    {
        List<string> lines = InputReader.ReadFileLines();
        List<Round> rounds = Parse(lines);

        long score = Scores(rounds);

        List<Round> changedRounds = ChangeRounds(rounds);
        long scoresChanged = Scores(changedRounds);

        return score + "\n" + scoresChanged;
    }

    private List<Round> Parse(IEnumerable<string> lines)
    {
        List<Round> rounds = new List<Round>();
        foreach (var line in lines)
        {
            string[] split = line.Split(' ');
            ERockPaperScissors opponent = Parse(split[0]);
            ERockPaperScissors response = Parse(split[1]);
            rounds.Add(new Round(opponent, response));
        }
        return rounds;
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
        long shapeScore = round.Response switch
        {
            ERockPaperScissors.Rock => 1,
            ERockPaperScissors.Paper => 2,
            ERockPaperScissors.Scissors => 3,
            _ => throw new ArgumentOutOfRangeException()
        };

        long outcome;
        if (round.Response == round.Opponent)
        {
            outcome = 3;
        }
        else if(round.Response == Win(round.Opponent))
        {
            outcome = 6;
        }
        else
        {
            outcome = 0;
        }

        return shapeScore + outcome;
    }

    private List<Round> ChangeRounds(List<Round> rounds)
    {
        List<Round> newRounds = new List<Round>();
        foreach (var round in rounds)
        {
            ERockPaperScissors response = ChangeRound(round);
            newRounds.Add(round with { Response = response });
        }
        return newRounds;
    }

    private ERockPaperScissors ChangeRound(Round round)
    {
        ERockPaperScissors newResponse = round.Response switch
        {
            //X means you need to lose,
            ERockPaperScissors.Rock => Lose(round.Opponent),
            //Y means you need to end the round in a draw, and
            ERockPaperScissors.Paper => round.Opponent,
            //Z means you need to win."
            ERockPaperScissors.Scissors => Win(round.Opponent),
            _ => throw new ArgumentOutOfRangeException()
        };

        return newResponse;
    }

    private ERockPaperScissors Win(ERockPaperScissors shape) => shape switch
    {
        ERockPaperScissors.Rock => ERockPaperScissors.Paper,
        ERockPaperScissors.Paper => ERockPaperScissors.Scissors,
        ERockPaperScissors.Scissors => ERockPaperScissors.Rock,
        _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
    };

    private ERockPaperScissors Lose(ERockPaperScissors shape) => shape switch
    {
        ERockPaperScissors.Rock => ERockPaperScissors.Scissors,
        ERockPaperScissors.Paper => ERockPaperScissors.Rock,
        ERockPaperScissors.Scissors => ERockPaperScissors.Paper,
        _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
    };

    private ERockPaperScissors Parse(string c) =>
        c switch
        {
            "A" => ERockPaperScissors.Rock,
            "B" => ERockPaperScissors.Paper,
            "C" => ERockPaperScissors.Scissors,
            "X" => ERockPaperScissors.Rock,
            "Y" => ERockPaperScissors.Paper,
            "Z" => ERockPaperScissors.Scissors,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
}
