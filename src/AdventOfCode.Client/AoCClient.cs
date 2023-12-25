using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AdventOfCode.Client.PrivateLeaderboard;

namespace AdventOfCode.Client;

public class AoCClient
{
    private const int CacheTimeSeconds = 900; 
    private const string UrlDay = "{0}/day/{1}/";
    private const string UrlInput = "input";
    private const string UrlBase = "https://adventofcode.com/";
    private const string UrlLeaderboard = "{0}/leaderboard/private/view/{1}.json";
    private const string UrlAnswer = "answer";
    private const string AdventofcodeFolder = "AdventOfCode";

    private readonly HttpClient _client;

    /// <summary>
    /// Creates a client based on a session string.
    /// The session content is verified (offline), and should start with `session=` and contain 128 character after the '='.
    /// Throws and <see cref="Exception"/> when the session content does not pass the offline verification.
    /// </summary>
    /// <param name="session"></param>
    public AoCClient(string session)
    {
        VerifySession(session);
        _client = new HttpClient(new HttpClientHandler())
        {
            BaseAddress = new Uri(UrlBase),
        };

        _client.DefaultRequestHeaders.Add("Cookie", session);
        _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "github.com/guuskuiper/AdventtOfCoding");
    }

    /// <summary>
    /// Download an input.
    /// The input text is cached forever in ProgramData\AdventOfCode.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    public async Task<string> DownloadInputAsync(int year, int day)
    {
        var cacheDirectory = GetCacheEventPath(year.ToString());
        string cachedInputPath = Path.Combine(cacheDirectory, $"input{day:D2}.txt");

        string dayUrl = string.Format(UrlDay, year, day);
        string inputText = await RequestOrCacheAsync(dayUrl + UrlInput, cachedInputPath, -1);

        return inputText;
    }
   
    /// <summary>
    /// Upload an answer and return the response.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="day"></param>
    /// <param name="part1"></param>
    /// <param name="answer"></param>
    /// <returns></returns>
    public async Task<string> UploadAnswerAsync(int year, int day, bool part1, string answer)
    {
        string dayUrl = string.Format(UrlDay, year, day);
        
        KeyValuePair<string, string>[] data =
        {
            new ("answer", answer),
            new ("level", part1 ? "1" : "2")
        };

        var response = await _client.PostAsync(dayUrl + UrlAnswer, new FormUrlEncodedContent(data));
        string html = await response.Content.ReadAsStringAsync();
        string text = ParseHtml(html);
        return text;
    }

    /// <summary>
    /// Fetches private leaderboard Json, and return the parsed result.
    /// Caches the result in ProgramData\AdventOfCode.
    /// A new request is only performed once ever 900s, otherwise the cached response is used.
    /// </summary>
    /// <param name="eventName">The year, so "2023" for example, 1ste part of the url to a private group</param>
    /// <param name="groupCode">Private group number, end of the url</param>
    /// <returns></returns>
    public async Task<AoCPrivateLeaderboard> GetLeaderboard(string eventName, int groupCode)
    {
        var cacheDirectory = GetCacheEventPath(eventName);
        string cacheJsonFilePath = Path.Combine(cacheDirectory, $"{groupCode}.json");
        
        string url = string.Format(UrlLeaderboard, eventName, groupCode);
        string jsonText = await RequestOrCacheAsync(url, cacheJsonFilePath, CacheTimeSeconds);
        
        AoCPrivateLeaderboard leaderboard = ParseLeaderboardJson(jsonText);

        return leaderboard;
    }

    /// <summary>
    /// Parses the json into <see cref="AoCPrivateLeaderboard"/>
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static AoCPrivateLeaderboard ParseLeaderboardJson(string json)
    {
        JsonSerializerOptions options = new() { Converters = { new DateTimeConverterForCustomStandardFormatR() } };
        AoCPrivateLeaderboard leaderboard = JsonSerializer.Deserialize<AoCPrivateLeaderboard>(json, options)!;
        return leaderboard;
    }
    
    public static string ParseHtml(ReadOnlySpan<char> html)
    {
        const string ArticleStart = "<article>";
        const string ArticleEnd = "</article>";
        
        int indexStart = html.IndexOf(ArticleStart);
        html = html.Slice(indexStart + ArticleStart.Length);
        
        int indexEnd = html.IndexOf(ArticleEnd);
        html = html.Slice(0, indexEnd);

        string noNewLines = html.ToString().Replace("\n", string.Empty);
        
        Regex stripTags = new Regex("<.*?>");
        string text = stripTags.Replace(noNewLines, string.Empty);
        return text;
    }
    
    public static async Task<string?> GetSessionAsync()
    {
        string? session = Environment.GetEnvironmentVariable("SESSION");
        if (session is null && File.Exists("SESSION"))
        {
            session = await File.ReadAllTextAsync("SESSION");
        }

        return session;
    }
    

    /// <summary>
    /// Verify content of the session string.
    /// Only based on the structure of the string.
    /// Doesnt call any API to verify whether the session is valid.
    /// </summary>
    /// <param name="session"></param>
    /// <exception cref="Exception"></exception>
    private void VerifySession(ReadOnlySpan<char> session)
    {
        const string Session = "session=";
        const int ValueLength = 128;
        
        if (!session.StartsWith(Session))
        {
            throw new Exception($"SESSION should start with: {Session}");
        }

        ReadOnlySpan<char> value = session.Slice(Session.Length);

        if (value.Length != ValueLength)
        {
            throw new Exception($"The session value (after the '=' character) should be {ValueLength} characters long, now its {value.Length}");
        }
    }
    
    private async Task<string> RequestOrCacheAsync(string url, string cacheFilePath, int cacheExpiration = -1)
    {
        string text;
        FileInfo cacheFile = new FileInfo(cacheFilePath);
        DirectoryInfo? cacheFileDirectory = cacheFile.Directory;
        ArgumentNullException.ThrowIfNull(cacheFileDirectory, nameof(cacheFilePath));
        
        if (!cacheFileDirectory.Exists)
        {
            cacheFileDirectory.Create();
        }
        
        if(cacheFile.Exists && (cacheExpiration < 0 || (DateTime.Now - cacheFile.LastWriteTime).TotalSeconds < CacheTimeSeconds))
        {
            text = await File.ReadAllTextAsync(cacheFilePath);
        }
        else
        {
            text = await _client.GetStringAsync(url);
            await File.WriteAllTextAsync(cacheFilePath, text);
        }
        return text;
    }
    
    private static string GetCacheEventPath(string eventName) =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            AdventofcodeFolder,
            eventName);

    private class DateTimeConverterForCustomStandardFormatR : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long timestamp = reader.GetInt64();
            return DateTime.UnixEpoch.AddSeconds(timestamp);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Only Deserialize implemented");
        }
    }
}