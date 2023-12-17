using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode.Client;
using AdventOfCode.Client.PrivateLeaderboard;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Test;

public class AoCClientTest
{
    private static string SkipReason => File.Exists("SESSION") ? "No session string" : string.Empty; 
    
    private readonly ITestOutputHelper _testOutputHelper;

    public AoCClientTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task ParseLeaderboard()
    {
        List<string> fileNames = GetTestJsonFileNames();
        foreach (var fileName in fileNames)
        {
            string jsonString = await File.ReadAllTextAsync(fileName);
            AoCPrivateLeaderboard leaderboard = AoCClient.ParseLeaderboardJson(jsonString);
            Assert.NotNull(leaderboard);
        }
    }

    [Fact]
    public async Task GetDayRanking()
    {
        List<string> fileNames = GetTestJsonFileNames();
        foreach (var fileName in fileNames)
        {
            string jsonString = await File.ReadAllTextAsync(fileName);
            AoCPrivateLeaderboard leaderboard = AoCClient.ParseLeaderboardJson(jsonString);
            string name = Path.GetFileNameWithoutExtension(jsonString);
            OutputLeaderboardRanking(leaderboard, fileName, 9);
            Assert.NotNull(leaderboard);
        }
    }

    [Fact]
    public async Task GetLeaderboardUpdates()
    {
        List<string> fileNames = GetTestJsonFileNames();
        foreach (var fileName in fileNames)
        {
            string jsonString = await File.ReadAllTextAsync(fileName);
            AoCPrivateLeaderboard leaderboard = AoCClient.ParseLeaderboardJson(jsonString);
            DateTime previousCheck = new DateTime(2023, 12, 7);
            var changes = AoCPrivateLeaderboard
                .ChangesSince(leaderboard, previousCheck)
                .OrderByDescending(x => x.Time).ToList();
            DateTime lastChange = changes.FirstOrDefault()?.Time ?? previousCheck;
            Assert.NotNull(leaderboard);
            Assert.True(lastChange > previousCheck);
        }
    }
    
    [SkippableFact]
    public async Task<AoCPrivateLeaderboard> DownloadLeaderboard()
    {
        string? session = await AoCClient.GetSessionAsync();
        Skip.If(string.IsNullOrEmpty(session), "No session");
        
        AoCClient client = new AoCClient(session);
        var leaderboard = await client.GetLeaderboard("2023", 1117050);
        return leaderboard;
    }
    
    [SkippableTheory]
    [InlineData(1117050)]
    [InlineData(1503320)]
    [InlineData(782191)]
    public async Task UpdateLeaderboard(int groupCode)
    {
        string? session = await AoCClient.GetSessionAsync();
        Skip.If(string.IsNullOrEmpty(session), "No session");

        AoCClient client = new AoCClient(session);
        AoCPrivateLeaderboard leaderboard = await client.GetLeaderboard("2023", groupCode);
        OutputLeaderboardRanking(leaderboard, groupCode.ToString(), DateTime.Now.Day);
    }
    
    [SkippableFact]
    public async Task<string> Download()
    {
        string? session = await AoCClient.GetSessionAsync();
        Skip.If(string.IsNullOrEmpty(session), "No session");
        
        AoCClient client = new AoCClient(session);
        var response = await client.DownloadInputAsync(2022, 5);
        return response;
    } 
    
    [SkippableFact]
    public async Task<string> Upload()
    {
        string? session = await AoCClient.GetSessionAsync();
        Skip.If(string.IsNullOrEmpty(session), "No session");

        AoCClient client = new AoCClient(session);
        var response = await client.UploadAnswerAsync(2022, 1, true, "TQRFCBSJJ");
        return response;
    }
    
    [Fact]
    public string ParseHtml()
    {
        string text = AoCClient.ParseHtml(Html);
        return text;
    }
    
    private static List<string> GetTestJsonFileNames()
    {
        var directory = Directory.GetCurrentDirectory();
        var fileNames = Directory.EnumerateFiles(directory + "\\Data", "*.json").ToList();
        return fileNames;
    }
    
    private void OutputLeaderboardRanking(AoCPrivateLeaderboard leaderboard, string leaderboardName, int day)
    {
        AoCPrivateRanking ranking = new(leaderboard);
        string result = ranking.GetRankingByDay(day);
        _testOutputHelper.WriteLine($"Group {leaderboardName}");
        _testOutputHelper.WriteLine(result);
    }

    private const string Html = """
    <body>
<header><div><h1 class="title-global"><a href="/">Advent of Code</a></h1><nav><ul><li><a href="/2020/about">[About]</a></li><li><a href="/2020/events">[Events]</a></li><li><a href="https://teespring.com/stores/advent-of-code" tar
get="_blank">[Shop]</a></li><li><a href="/2020/settings">[Settings]</a></li><li><a href="/2020/auth/logout">[Log Out]</a></li></ul></nav><div class="user">guuskuiper</div></div><div><h1 class="title-event">&nbsp;&nbsp;&nbsp;&nbsp
;&nbsp;&nbsp;&nbsp;<span class="title-event-wrap">?y.</span><a href="/2020">2020</a><span class="title-event-wrap"></span></h1><nav><ul><li><a href="/2020">[Calendar]</a></li><li><a href="/2020/support">[AoC++]</a></li><li><a hre
f="/2020/sponsors">[Sponsors]</a></li><li><a href="/2020/leaderboard">[Leaderboard]</a></li><li><a href="/2020/stats">[Stats]</a></li></ul></nav></div></header>

<div id="sidebar">
<div id="sponsor"><div class="quiet">Our <a href="/2020/sponsors">sponsors</a> help make Advent of Code possible:</div><div class="sponsor"><a href="https://aoc.infi.nl/" target="_blank" onclick="if(ga)ga('send','event','sponsor'
,'sidebar',this.href);" rel="noopener">Infi</a> - Bepakt en bezakt gaat de Kerstman eropuit om de cadeautjes te bezorgen. Alles staat al bijna klaar, maar: Passen de pakjes wel in de zak?</div></div>
</div><!--/sidebar-->

<main>
<article><p>That's not the right answer.  If you're stuck, make sure you're using the full input data; there are also some general tips on the <a href="/2020/about">about page</a>, or you can ask for hints on the <a href="https:/
/www.reddit.com/r/adventofcode/" target="_blank">subreddit</a>.  Please wait one minute before trying again. (You guessed <span style="white-space:nowrap;"><code>TQRFCBSJJ</code>.)</span> <a href="/2020/day/1">[Return to Day 1]</
a></p></article>
</main>

<!-- ga -->
<script>
(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
})(window,document,'script','//www.google-analytics.com/analytics.js','ga');
ga('create', 'UA-69522494-1', 'auto');
ga('set', 'anonymizeIp', true);
ga('send', 'pageview');
</script>
<!-- /ga -->
</body>
</html>
""";
}