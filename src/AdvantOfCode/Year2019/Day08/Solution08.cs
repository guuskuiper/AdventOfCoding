using System.Text;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day08;

[DayInfo(2019, 08)]
public class Solution08 : Solution
{
    private const char FullBlock = (char)0x2588;
    private const char VisibleChar = FullBlock;
    private const char InvisibleChar = ' ';
    
    const int WIDTH = 25;
    const int HEIGTH = 6;
    
    public string Run()
    {
        string[] input = this.ReadLines();

        List<int[,]> image = new List<int[,]>();

        foreach (var chunk in input[0].Chunk(WIDTH*HEIGTH))
        {
            int index = 0;
            image.Add(new int[WIDTH, HEIGTH]);
            var curLayer = image[image.Count - 1];
            for(int x = 0 ; x < HEIGTH; x++)
            {
                for(int y = 0; y < WIDTH; y++)
                {
                    var ch = chunk[index++];
                    var pixel = (int)char.GetNumericValue(ch);
                    if(pixel < 0) 
                    {
                        throw new Exception("Invalid image");
                    }
                    curLayer[y, x] = pixel;
                }
            }
        }

        var leastZeros = int.MaxValue;
        var ones = 0;
        var twos = 0;

        foreach(var layer in image)
        {
            var groups = layer.Cast<int>().GroupBy( x => x).ToDictionary(v => v.Key, v => v.Count());
            var curZeros = groups[0];
            if(curZeros < leastZeros)
            {
                leastZeros = curZeros;
                ones = groups[1];
                twos = groups[2];
            }
        }

        int result = ones * twos;

        var decoded = DecodeImage(image);


        StringBuilder sb = new();
        for (int y = 0; y < decoded.GetLength(1); y++)
        {
            for (int x = 0; x < decoded.GetLength(0); x++)
            {
                sb.Append(decoded[x, y] == 1 ? VisibleChar : InvisibleChar);
            }

            sb.AppendLine();
        }

        Console.WriteLine(sb);

        return result + "\n" + sb;
    }

    static int[,] DecodeImage(IEnumerable<int[,]> image)
    {
        var decoded = new int[WIDTH, HEIGTH];

        for(int x = 0 ; x < HEIGTH; x++)
        {
            for(int y = 0; y < WIDTH; y++)
            {
                foreach(var layer in image)
                {
                    var pixel = layer[y,x];
                    if(pixel != 2)
                    {
                        decoded[y, x] = pixel;
                        break;
                    }
                }
            }
        }
        
        return decoded;
    }
}    