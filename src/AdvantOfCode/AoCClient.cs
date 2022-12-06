using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class AoCClient
{
    private const string UrlDay = "{0}/day/{1}/";
    private const string UrlInput = "input";
    private const string UrlBase = "https://adventofcode.com/";
    private const string UrlAnswer = "answer";

    private readonly HttpClient _client;

    public AoCClient(string session)
    {
        VerifySession(session);
        _client = new HttpClient(new HttpClientHandler());
        _client.BaseAddress = new Uri(UrlBase);

        _client.DefaultRequestHeaders.Add("Cookie", session);
        _client.DefaultRequestHeaders.Add("User-Agent", "C#");
    }

    public async Task<string> DownloadInputAsync(int year, int day)
    {
        string dayUrl = string.Format(UrlDay, year, day);

        string input = await _client.GetStringAsync(dayUrl + UrlInput);
        return input;
    }
   
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
    
    public string UploadAnswer(int year, int day, bool part1, string answer)
    {
        string dayUrl = string.Format(UrlDay, year, day);
        
        KeyValuePair<string, string>[] data =
        {
            new ("answer", answer),
            new ("level", part1 ? "1" : "2")
        };
        
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, dayUrl + UrlAnswer)
        {
            Content = new FormUrlEncodedContent(data)
        };
        
        var response = _client.Send(message);
        using var reader = new StreamReader(response.Content.ReadAsStream());
        string html = reader.ReadToEnd();
        string text = ParseHtml(html);
        return text;
    }
    
    public static async Task<string> DownloadAsync(int year, int day)
    {
        string session = await File.ReadAllTextAsync("SESSION");
        AoCClient downloader = new AoCClient(session);
        return await downloader.DownloadInputAsync(year, day);
    }
    
    public static async Task<string> UploadAsync(int year, int day, bool part1, string answer)
    {
        string session = await File.ReadAllTextAsync("SESSION");
        AoCClient downloader = new AoCClient(session);
        return await downloader.UploadAnswerAsync(year, day, part1, answer);
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
    
    // Verify content of the session string.
    // Only based on the structure of the string.
    // Doesnt call any API to verify whether the session is valid.
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

}