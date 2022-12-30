using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day22;

[DayInfo(2019, 22)]
public class Solution22 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        var deck = new SpaceCards(10007);
        deck.ShuffleN(input, 1);
        int partA = deck.Find(2019);

        // => y = a*x + b % length
        // cut: y = 1*x + length + n % length
        // inverse: (length + index + n) % length;
        // a = 1, b = length + n (inverse)
        // a = 1, b = length - n (normal)
        // dealIntoStack: y = -1*x + length - 1 (% length)
        // a = -1, b = length - 1 (inverse)
        // a = -1, b = length - 1 (normal)
        // inverse: length - 1 - index;
        // dealWith:
        // y = n*x % length? (normal)
        // a = 1, b = 0
        // or
        // y = inv_0*x % length (inverse)

        //y = f(g(x))

        // Cut(n) -> Stack
        // 1 * x + L+N % L -> -1 * x + L - 1
        // => (x + L+N) * (-1x + L-1) = -x^2 + (-1-N)x + (L+N)(L-1)

        var part1 = deck.Inverse(8502, 10007, 1, input);
        var part12 = deck.InverseAB(8502, 10007, 1, input);
        var partB = deck.InverseAB(2020, 119315717514047, 101741582076661, input);

        return partA + "\n" + partB;
    }
}    