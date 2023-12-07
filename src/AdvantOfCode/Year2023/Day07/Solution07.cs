using System.Diagnostics;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day07;

[DayInfo(2023, 07)]
public class Solution07 : Solution
{
    private enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
		ThreeOfAKind,
	    FullHouse,
	    FourOfAKind,
	    FiveOfAKind,
    }

	private record Hand(char[] Cards, int Bid);
	private record CardGroup(char Card, int Count);
	private record HandCardGroup(Hand Hand, CardGroup[] Group);

    public string Run()
    {
        string[] input = this.ReadLines();

        Hand[] hands = input.Select(ParseLine).ToArray();

        List<HandCardGroup> groups = hands.Select(GetCardGroups).ToList();
		List<HandCardGroup> groupsJokers = groups.Select(ApplyJokers).ToList();

        groups.Sort(Comparison);
		groupsJokers.Sort(ComparisonJokers);

		long sum = CountWinning(groups);
        long sum2 = CountWinning(groupsJokers);

		return sum + "\n" + sum2;
	}

    private int CountWinning(IEnumerable<HandCardGroup> hands)
    {
	    int sum = 0;
	    int rank = 1;
	    foreach (var hand in hands)
	    {
		    sum += rank * hand.Hand.Bid;
		    rank++;
	    }

	    return sum;
    }

    private int ComparisonWithJokers(char x, char y)
    {
	    const string orderRev = "J23456789TQKA"; // 'J' now lowest
	    int xInt = orderRev.IndexOf(x);
	    int yInt = orderRev.IndexOf(y);
		Debug.Assert(xInt >= 0);
		Debug.Assert(yInt >= 0);
	    return xInt.CompareTo(yInt);
    }

	private int Comparison(char x, char y)
    {
	    const string orderRev = "23456789TJQKA";
	    int xInt = orderRev.IndexOf(x);
        int yInt = orderRev.IndexOf(y);
        Debug.Assert(xInt >= 0);
        Debug.Assert(yInt >= 0);
		return xInt.CompareTo(yInt);
    }

	private int Comparison(HandCardGroup x, HandCardGroup y) => Comparison(x, y, Comparison);
	private int ComparisonJokers(HandCardGroup x, HandCardGroup y) => Comparison(x, y, ComparisonWithJokers);
    private int Comparison(HandCardGroup x, HandCardGroup y, Comparison<char> charComparer)
    {
	    HandType xHandType = GetHandType(x.Group);
	    HandType yHandType = GetHandType(y.Group);

	    int compare = xHandType.CompareTo(yHandType);

	    if (compare == 0)
	    {
		    for (int i = 0; i < 5; i++)
		    {
			    compare = charComparer(x.Hand.Cards[i], y.Hand.Cards[i]);
				if(compare != 0) break;
		    }
	    }

	    return compare;
    }

    private HandType GetHandType(CardGroup[] group)
    {
	    HandType type = (group[0].Count, group.Length > 1 ? group[1].Count : 0) switch
	    {
		    (5, 0) => HandType.FiveOfAKind,
		    (4, 1) => HandType.FourOfAKind,
		    (3, 2) => HandType.FullHouse,
		    (3, _) => HandType.ThreeOfAKind,
		    (2, 2) => HandType.TwoPair, 
		    (2, _) => HandType.OnePair,
		    _ => HandType.HighCard
	    };

	    return type;
    }

	private Hand ParseLine(string line)
    {
        LineReader reader = new LineReader(line);
        char[] chars = reader.ReadLettersAndDigits().ToArray();
        reader.SkipWhitespaces();
        int number = reader.ReadInt();

        return new Hand(chars, number);
    }

	private HandCardGroup ApplyJokers(HandCardGroup hand)
	{
		var jokerGroup = hand.Group.FirstOrDefault(x => x.Card == 'J');
		if (jokerGroup is null || hand.Group.Length == 1) return hand;

		CardGroup[] nonJokerGroups = hand.Group.Where(x => x.Card != 'J').ToArray();
		nonJokerGroups[0] = nonJokerGroups[0] with { Count = nonJokerGroups[0].Count + jokerGroup.Count };

		return hand with { Group = nonJokerGroups };
	}

    private HandCardGroup GetCardGroups(Hand hand)
    {
	    var groups = hand.Cards
		    .GroupBy(x => x)
		    .Select(x => new CardGroup(x.Key, x.Count()))
		    .OrderByDescending(x => x.Count)
		    .ToArray();

	    return new HandCardGroup(hand, groups);
    }
}    