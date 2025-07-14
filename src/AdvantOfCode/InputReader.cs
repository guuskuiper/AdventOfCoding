using System.Runtime.CompilerServices;
using AdventOfCode.Client;

namespace AdventOfCode;

public static class InputReader
{
    public static string ReadFile(int year, int day, string inputFilePath)
    {
        if (!File.Exists(inputFilePath))
        {
            string directory = Path.GetDirectoryName(inputFilePath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            string? session = AoCClient.GetSession();
            ArgumentNullException.ThrowIfNull(session);
            AoCClient downloader = new AoCClient(session);
            string text = downloader.DownloadInput(year, day);
            File.WriteAllText(inputFilePath, text);
            return text;
        }
        return File.ReadAllText(inputFilePath);
    }

    public static async Task<string> ReadFileAsync(int year, int day, string inputFilePath)
    {
        if (!File.Exists(inputFilePath))
        {
            string directory = Path.GetDirectoryName(inputFilePath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            string? session = await AoCClient.GetSessionAsync();
            ArgumentNullException.ThrowIfNull(session);
            AoCClient downloader = new(session);
            string text = await downloader.DownloadInputAsync(year, day);
            await File.WriteAllTextAsync(inputFilePath, text);
            return text;
        }
        return await File.ReadAllTextAsync(inputFilePath);
    }
}