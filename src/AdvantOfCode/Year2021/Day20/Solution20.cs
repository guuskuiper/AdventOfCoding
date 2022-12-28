namespace AdventOfCode.Year2021.Day20;

[DayInfo(2021, 20)]
public class Solution20 : Solution
{
    private const char HIGH = '#';
    private const char LOW = '.';
    
    private string _imageEnhancementAlgorithm;
    private string[] _image;

    private int _step = 0;
    
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        _imageEnhancementAlgorithm = lines[0];
        _image = lines[1..];
        
        Enhance();
        //PrintImage();
        
        Enhance();
        //PrintImage();
        long A = CountPixels();

        Enhance(50 - 2);
        long B = CountPixels();
        
        return A + "\n" + B;
    }

    private void Enhance(int count)
    {
        foreach (var i in Enumerable.Range(0, count))
        {
            Enhance();
        }
    }

    private void PrintImage()
    {
        Console.WriteLine("Show image:");
        foreach (var row in _image)
        {
            Console.WriteLine(row);
        }
    }

    private long CountPixels()
    {
        long count = 0;
        foreach (var row in _image)
        {
            foreach (var pixel in row)
            {
                if (pixel == HIGH)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void Enhance()
    {
        int newXLength = _image.Length + 2;
        int newYLength = _image[0].Length + 2;

        string[] newImage = new string[_image.Length + 2];
        string currentLine = "";
        for (int x = 0; x < newXLength; x++)
        {
            for (int y = 0; y < newYLength; y++)
            {
                int value = GetWindow(x - 1, y - 1);
                char c = _imageEnhancementAlgorithm[value];
                currentLine += c;
            }
            newImage[x] = currentLine;
            currentLine = "";
        }

        _image = newImage;
        _step++;
    }

    private int GetWindow(int x, int y)
    {
        string bits = "";
        for (int dx = -1; dx < 2; dx++)
        {
            for (int dy = -1; dy < 2; dy++)
            {
                bits += GetPixel(x + dx, y + dy) ? '1' : '0';
            }
        }

        return Convert.ToInt32(bits, 2);
    }

    private bool GetPixel(int x, int y)
    {
        if (!InRange(x, y))
        {
            if (_step % 2 == 1)
            {
                return _imageEnhancementAlgorithm[0] == HIGH;
            }

            // should always be false
            // _imageEnhancementAlgorithm[255] is low when _imageEnhancementAlgorithm[0] is high
            // otherwise there is a infinite number of high pixels :)
            return false;
        }

        return _image[x][y] == HIGH;
    }
    
    private bool InRange(int x, int y)
     => x >= 0 && x < _image.Length &&
        y >= 0 && y < _image[0].Length;
}
