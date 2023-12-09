using System.Text.Json.Serialization;

namespace AdventOfCode;

public class AoCPrivateLeaderboard
{
    [JsonPropertyName("event")]
    public string Event { get; set; }
    
    [JsonPropertyName("owner_id")]
    public int OwnerId { get; set; }
    
    [JsonPropertyName("members")]
    public IDictionary<string, AoCMember> Members { get; set; }

    public static IEnumerable<AocUpdate> ChangesSince(AoCPrivateLeaderboard leaderboard, DateTime timeStamp)
    {
        List<AocUpdate> updates = [];
        
        foreach (var currentMember in leaderboard.Members.Values)
        {
            if(currentMember.LastStarTimeStamp <= timeStamp) continue;

            foreach ((var day, AoCDay? value) in currentMember.DayCompletion)
            {
                if (value.Part1.CompletionTimeStamp > timeStamp)
                {
                    AocUpdate update = new AocUpdate(currentMember.Name, leaderboard.Event, day, "1",
                        value.Part1.CompletionTimeStamp);
                    updates.Add(update);
                }
                
                if (value.Part2?.CompletionTimeStamp > timeStamp)
                {
                    AocUpdate update = new AocUpdate(currentMember.Name, leaderboard.Event, day, "2",
                        value.Part2.CompletionTimeStamp);
                    updates.Add(update);
                }
            }
        }

        return updates;
    }
}

public record AocUpdate(string UserName, string Event, string Day, string Part, DateTime Time);

public class AoCMember
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("stars")]
    public int Stars { get; set; }
    
    [JsonPropertyName("local_score")]
    public int LocalScore { get; set; }
    
    [JsonPropertyName("Global_score")]
    public int GlobalScore { get; set; }
    
    [JsonPropertyName("last_star_ts")]
    public DateTime LastStarTimeStamp { get; set; }
    
    [JsonPropertyName("completion_day_level")]
    public IDictionary<string,AoCDay> DayCompletion { get; set; }
}

public class AoCDay
{
    [JsonPropertyName("1")]
    public AocDayPart Part1 { get; set; }
    
    [JsonPropertyName("2")]
    public AocDayPart? Part2 { get; set; }
}

public class AocDayPart
{
    [JsonPropertyName("get_star_ts")]
    public DateTime CompletionTimeStamp { get; set; }
    
    [JsonPropertyName("star_index")]
    public int StarIndex { get; set; }
}

