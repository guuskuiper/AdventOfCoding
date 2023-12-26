using AdventOfCode.Extensions;

namespace AdventOfCode.Year2022.Day08;

[DayInfo(2022, 08)]
public class Solution08 : Solution
{
    private int[,] _heigths;
    
    public string Run()
    {
        string[] lines = this.ReadLines();

        _heigths = Parse(lines);
        bool[,] visible = Visible();

        int count = visible.ToEnumerable().Count(x => x);

        int maxScore = MaxScenicScore();
        
        return count + "\n" + maxScore;
    }

    private int[,] Parse(string[] lines)
    {
        int heigth = lines.Length;
        int width = lines[0].Length;
        int[,] heights = new int[lines.Length,lines[0].Length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < heigth; y++)
            {
                heights[x, y] = lines[x][y] - '0';
            }
        }

        return heights;
    }

    private int MaxScenicScore()
    {
        int max = -1;
        int size = _heigths.GetLength(0);
        
        for (int x = 1; x < size - 1; x++)
        {
            for (int y = 1; y < size - 1; y++)
            {
                int referenceHeight = _heigths[x, y];
                
                int score = 1;
                score *= LeftSteps(x, y, referenceHeight);
                score *= RightSteps(x, y, size, referenceHeight);
                score *= TopSteps(x, y, referenceHeight);
                score *= BottomSteps(x, y, size, referenceHeight);
 
                if (score > max) max = score;
            }
        }

        return max;
    }

    private int BottomSteps(int x, int y, int size, int referenceHeight)
    {
        int bottom = 0;
        for (int k = y + 1; k < size; k++)
        {
            bottom++;
            if (_heigths[x, k] >= referenceHeight) break;
        }
        return bottom;
    }

    private int TopSteps(int x, int y, int referenceHeight)
    {
        int top = 0;
        for (int k = y - 1; k >= 0; k--)
        {
            top++;
            if (_heigths[x, k] >= referenceHeight) break;
        }
        return top;
    }

    private int RightSteps(int x, int y, int size, int referenceHeight)
    {
        int right = 0;
        for (int k = x + 1; k < size; k++)
        {
            right++;
            if (_heigths[k, y] >= referenceHeight) break;
        }
        return right;
    }

    private int LeftSteps(int x, int y, int referenceHeight)
    {
        int left = 0;
        for (int k = x - 1; k >= 0; k--)
        {
            left++;
            if (_heigths[k, y] >= referenceHeight) break;
        }
        return left;
    }

    private bool[,] Visible()
    {
        bool[,] visible = new bool[_heigths.GetLength(0), _heigths.GetLength(1)];

        int max = _heigths.GetLength(0) - 1;
        
        for (int i = 0; i <= max; i++)
        {
            int maxLR = -1;
            int maxRL = -1;
            int maxTB = -1;
            int maxBT = -1; 
            for (int j = 0; j <= max; j++)
            {
                if(_heigths[i, j] > maxLR) { maxLR = _heigths[i, j]; visible[i, j] = true;}
                if(_heigths[i, max-j] > maxRL) { maxRL = _heigths[i, max- j]; visible[i, max - j] = true;}
                if(_heigths[j, i] > maxTB) { maxTB = _heigths[j, i]; visible[j, i] = true;}
                if(_heigths[max - j, i] > maxBT) { maxBT = _heigths[max - j, i]; visible[max - j, i] = true;}
            }
        }

        return visible;
    }
}
