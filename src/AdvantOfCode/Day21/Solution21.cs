using AdventOfCode.Extentions;

namespace AdventOfCode.Day21;

public class Solution21 : Solution
{
    public class DetermanisticDie
    {
        public int Rolled { get; private set; }
        public int Roll() => ++Rolled % 100;
        public int[] Roll(int number) => Enumerable.Range(0, number).Select(_ => Roll()).ToArray();
    }

    private DetermanisticDie _die = new ();
    private int[] _players;
    private int[] _score;
    private int[] _sums = GetSumRolls();
    
    public string Run()
    {
        var lines = InputReader.ReadFileLines();

        ParsePlayers(lines);
        long A = Play();
        
        ParsePlayers(lines);
        ulong B = PlayQuantum();
        
        return A + "\n" + B;
    }

    public class Item
    {
        public Item(int round, int score, int position)
        {
            Round = round;
            Score = score;
            Position = position;
        }

        public int Round { get; init; }
        public int Score { get; init; }
        public int Position { get; init; }
        
    }

    private ulong PlayQuantum()
    {
        var player0 = CalculatePlayerScores(_players[0]);
        var player1 = CalculatePlayerScores(_players[1]);

        int maxRound = player0.GetLength(0);

        ulong universesWins = 0;
        ulong universesLoses = 0;
        ulong previousUniversesP1 = 0;
        for (int round = 0; round < maxRound; round++)
        {
            (ulong p0win, ulong p0notWin) = GetWinLose(player0, round);
            Console.WriteLine($"Round {round} P0 Win {p0win:D3}-{p0notWin:D3} of {p0win + p0notWin:D3}");
            (ulong p1win, ulong p1notWin) = GetWinLose(player1, round);
            Console.WriteLine($"Round {round} P1 Win {p1win:D3}-{p1notWin:D3} of {p1win + p1notWin:D3}");

            universesWins += p0win * previousUniversesP1;
            universesLoses += p1win * p0notWin;

            previousUniversesP1 = p1notWin;
        }
        
        return universesWins > universesLoses ? universesWins: universesLoses;
    }

    private (ulong win, ulong lose) GetWinLose(int[,] player, int round)
    {
        int win = player[round, 21];
        int lose = 0;
        for (int i = 0; i < 21; i++)
        {
            lose += player[round, i];
        }

        return ((ulong)win, (ulong)lose);
    }

    private int[,] CalculatePlayerScores(int start)
    {
        Queue<Item> queue = new Queue<Item>();
        int[,] player = new int[21, 22];
        queue.Enqueue(new Item(-1, 0, start));

        int[,] done = new int[21, 2];

        int options = 0;

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            options++;

            var newSpaces = GetPositionOptionsRolls(cur.Position);
            foreach (var newSpace in newSpaces)
            {
                int round = cur.Round + 1;
                int newScore = Math.Min(21, cur.Score + newSpace);
                player[round, newScore] += 1;
                if (newScore < 21)
                {
                    done[round, 0] += 1;
                    queue.Enqueue(new Item(round, newScore, newSpace));
                }
                else
                {
                    done[round, 1] += 1;
                }
            }
        }

        Console.WriteLine($"Start {start}, options {options}");

        return player;
    }

    private long Play()
    {
        bool done = false;
        int player = 0;
        while (!done)
        {
            done |= Play(player);
            player = (player+1) % 2;
        }

        return _score[player] * _die.Rolled;
    }

    private bool Play(int player)
    {
        var rolls = _die.Roll(3).Sum();
        int newSpace = BoardPosition(player, rolls);

        _players[player] = newSpace;
        _score[player] += newSpace;
        
        return _score[player] >= 1000;
    }

    private int BoardPosition(int player, int totalRoll)
    {
        return GetPosition(_players[player], totalRoll);
    }

    private static int GetPosition(int space, int roll)
    {
        return (space + roll - 1) % 10 + 1;
    }

    private int[] GetPositionOptionsRolls(int space)
    {
        int[] newScores = new int[_sums.Length];

        int index = 0;
        foreach (var roll in _sums)
        {
            newScores[index] = GetPosition(space, roll);
            index++;
        }

        return newScores;
    }
    
    private void ParsePlayers(List<string> lines)
    {
        _players = new int[lines.Count];
        _score = new int[lines.Count];
        foreach (var line in lines)
        {
            ParsePlayer(line);
        }
    }

    private void ParsePlayer(string line)
    {
        var split = line.Split();
        var player = int.Parse(split[1]);
        var start = int.Parse(split[4]);

        _players[player - 1] = start;
    }

    private static int[] GetSumRolls()
    {
        var numbers = Enumerable.Range(1, 3).ToArray();
        var variations = numbers.CombinationsWithRepetition(3);
        return variations.Select(x => x.Sum()).ToArray();
    }
}
