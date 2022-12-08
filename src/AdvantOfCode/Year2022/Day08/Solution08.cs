namespace AdventOfCode.Year2022.Day08;

public class Solution08 : Solution
{
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();

        int[,] heights = Parse(lines);
        bool[,] visible = Visible(heights);

        int count = 0;
        foreach (var b in visible)
        {
            if (b) count++;
        }

        var scores = ScenicScore(heights);
        int maxScore = 0;
        foreach (var score in scores)
        {
            if (score > maxScore)
            {
                maxScore = score;
            }
        }
        
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

    private int[,] ScenicScore(int[,] heights)
    {
        int[,] scores = new int[heights.GetLength(0), heights.GetLength(1)];
        
        for (int x = 1; x < heights.GetLength(0)-1; x++)
        {
            for (int y = 1; y < heights.GetLength(0)-1; y++)
            {
                scores[x, y] = Score(heights, x, y);
            }
        }

        return scores;
    }

    private int Score(int[,] heights, int cx, int cy)
    {
        int cheight = heights[cx, cy];

        int score = 1;

        int left;
        for (left = cx - 1; left > 0; left--)
        {
            int height = heights[left, cy];
            if (height >= cheight) break;
        }

        score *= Math.Abs(cx - left);

        int right;
        for (right = cx + 1; right < heights.GetLength(0) - 1; right++)
        {
            int height = heights[right, cy];
            if (height >= cheight) break;
        }

        score *= Math.Abs(cx - right);
    
        int top;
        for (top = cy - 1; top > 0; top--)
        {
            int height = heights[cx, top];
            if (height >= cheight) break;
        }

        score *= Math.Abs(cy - top);
        
        int bottom;
        for (bottom = cy + 1; bottom < heights.GetLength(1) - 1; bottom++)
        {
            int height = heights[cx, bottom];
            if (height >= cheight) break;
        }

        score *= Math.Abs(cy - bottom);

        return score;
    }

    private bool[,] Visible(int[,]  heights)
    {
        bool[,] visible = new bool[heights.GetLength(0), heights.GetLength(1)];

        // left to right
        for (int y = 0; y < heights.GetLength(1); y++)
        {
            int highest = -1;
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                int current = heights[x, y];
                if (current > highest)
                {
                    highest = current;
                    visible[x, y] = true;
                }
            }
        }
        
        // right to left 
        for (int y = 0; y < heights.GetLength(1); y++)
        {
            int highest = -1;
            for (int x = heights.GetLength(0)-1; x >= 0; x--)
            {
                int current = heights[x, y];
                if (current > highest)
                {
                    highest = current;
                    visible[x, y] = true;
                }
            }
        }
        
        // top to bottom
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            int highest = -1;
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                int current = heights[x, y];
                if (current > highest)
                {
                    highest = current;
                    visible[x, y] = true;
                }
            }
        }
        
        // bottom to top 
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            int highest = -1;
            for (int y = heights.GetLength(1) - 1; y >= 0; y--)
            {
                int current = heights[x, y];
                if (current > highest)
                {
                    highest = current;
                    visible[x, y] = true;
                }
            }
        }

        return visible;
    }
}
