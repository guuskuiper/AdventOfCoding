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
	private record HandCardJokerGroup(Hand Hand, CardGroup[] Group, int Jokers) : HandCardGroup(Hand, Group);

    public string Run()
    {
        string[] input = this.ReadLines();

        Hand[] hands = input.Select(ParseLine).ToArray();

        List<HandCardGroup> groups = hands.Select(GetCardGroups).ToList();
		List<HandCardGroup> groupsJokers = groups.Select(ApplyJokers).ToList();
        List<HandCardJokerGroup> jokerGroups = groups.Select(x =>
	        new HandCardJokerGroup(x.Hand, x.Group.Where(y => y.Card != 'J').ToArray(),
		        x.Hand.Cards.Count(x => x == 'J'))).ToList();

        groups.Sort(Comparison);
		groupsJokers.Sort(ComparisonJoker2);
		jokerGroups.Sort(ComparisonWithJokers);

		long sum = CountWinning(groups);
        long sum2 = CountWinning(groupsJokers);
        long sum3 = CountWinning(jokerGroups);

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

    private int ComparisonWithJokers(HandCardJokerGroup x, HandCardJokerGroup y)
    {
	    HandType xHandType = GetHandTypeJoker(x);
	    HandType yHandType = GetHandTypeJoker(y);

	    int compare = xHandType.CompareTo(yHandType);

	    if (compare == 0)
	    {
		    for (int i = 0; i < 5; i++)
		    {
			    compare = ComparisonWithJokers(x.Hand.Cards[i], y.Hand.Cards[i]);
			    if (compare != 0) break;
		    }
	    }

	    return compare;
    }

    private HandType GetHandTypeJoker(HandCardJokerGroup hand)
    {
	    HandType type;

		if (hand.Jokers == 0) type = GetHandType(hand.Group);
		else if (hand.Jokers == 5) type = HandType.FiveOfAKind;
		else
		{
			// make sure count does not include jokers
			type = (hand.Group[0].Count, hand.Group.Length > 1 ? hand.Group[1].Count : 0, hand.Jokers) switch
			{
				(4, _, 1) => HandType.FiveOfAKind,
				(3, _, 2) => HandType.FiveOfAKind,
				(2, _, 3) => HandType.FiveOfAKind,
				(1, _, 4) => HandType.FiveOfAKind,
				(3, _, 1) => HandType.FourOfAKind,
				(2, _, 2) => HandType.FourOfAKind,
				(1, _, 3) => HandType.FourOfAKind,
				(2, 2, 1) => HandType.FullHouse,
				(2, _, 1) => HandType.ThreeOfAKind,
				(1, 1, 2) => HandType.ThreeOfAKind,
				(1, _, 1) => HandType.OnePair,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		if (hand.Jokers == 5)
		{
			return HandType.FiveOfAKind;
		}
		int maxCountWithJokers = hand.Group[0].Count + hand.Jokers;
		var group0 = hand.Group[0] with { Count = maxCountWithJokers };
		CardGroup[] group = hand.Group.ToArray();
		group[0] = group0;

		HandType type2 = GetHandType(group);
		Debug.Assert(type == type2);

		return type2;
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

    private int Comparison(HandCardGroup x, HandCardGroup y)
    {
	    HandType xHandType = GetHandType(x.Group);
	    HandType yHandType = GetHandType(y.Group);

	    int compare = xHandType.CompareTo(yHandType);

	    if (compare == 0)
	    {
		    for (int i = 0; i < 5; i++)
		    {
			    compare = Comparison(x.Hand.Cards[i], y.Hand.Cards[i]);
				if(compare != 0) break;
		    }
	    }

	    return compare;
    }

    private int ComparisonJoker2(HandCardGroup x, HandCardGroup y)
    {
	    HandType xHandType = GetHandType(x.Group);
	    HandType yHandType = GetHandType(y.Group);

	    int compare = xHandType.CompareTo(yHandType);

	    if (compare == 0)
	    {
		    for (int i = 0; i < 5; i++)
		    {
			    compare = ComparisonWithJokers(x.Hand.Cards[i], y.Hand.Cards[i]);
			    if (compare != 0) break;
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
		if (jokerGroup is null) return hand;

		List<CardGroup> nonJokerGroups = hand.Group.Where(x => x.Card != 'J').ToList();
		if(nonJokerGroups.Count == 0)
		{
			nonJokerGroups.Add(jokerGroup); // only jokers
		}
		else
		{
			nonJokerGroups[0] = nonJokerGroups[0] with { Count = nonJokerGroups[0].Count + jokerGroup.Count };
		}

		return new HandCardGroup(hand.Hand, nonJokerGroups.ToArray());
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