using System.Text;

namespace AdventOfCode.Client;

public class AoCPrivateRanking
{
    private readonly AoCPrivateLeaderboard _leaderboard;
    
    public AoCPrivateRanking(AoCPrivateLeaderboard leaderboard)
    {
        _leaderboard = leaderboard;
    }

    private record DayStats(string UserName, DateTime Part1, DateTime? Part2);

    public string GetRankingByDay(int day)
    {
        List<DayStats> stats = new();

        foreach ((var memberId, AoCMember? member) in _leaderboard.Members)
        {
            if (member.DayCompletion.TryGetValue(day, out AoCDay? dayResult))
            {
                DayStats stat = new DayStats(member.Name, dayResult.Part1.CompletionTimeStamp,
                    dayResult.Part2?.CompletionTimeStamp);
                stats.Add(stat);
            }
        }

        const int DateTimeLength = 8;
        int maxUserNameLength = stats.Count > 0 ? stats.Max(x => x.UserName.Length) : 4;
        StringBuilder sb = new();
        sb.AppendLine($"AoC result for Day{day} - {_leaderboard.Event}:");
        sb.Append("User".PadRight(maxUserNameLength));
        sb.Append('|');
        sb.Append("Part 1".PadRight(8));
        sb.Append('|');
        sb.Append("Part 2".PadRight(8));
        sb.Append('|');
        sb.AppendLine("Diff".PadRight(8));
        sb.AppendLine(new string('-', maxUserNameLength + 3 * DateTimeLength + 3 * 1));

        foreach (DayStats stat in stats.OrderBy(x => x.Part2))
        {
            sb.Append(stat.UserName.PadRight(maxUserNameLength));
            sb.Append(' ');
            sb.Append(stat.Part1.ToLocalTime().ToString("HH:mm:ss"));
            sb.Append(' ');
            sb.Append(stat.Part2?.ToLocalTime().ToString("HH:mm:ss") ?? "-".PadRight(8)) ;
            sb.Append(' ');

            TimeSpan diff = stat.Part2 is null ? TimeSpan.Zero : stat.Part2.Value - stat.Part1;
            sb.AppendLine(diff.ToString());
        }
        
        return sb.ToString();
    }
}