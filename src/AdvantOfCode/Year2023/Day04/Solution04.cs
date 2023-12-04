using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day04;

[DayInfo(2023, 04)]
public class Solution04 : Solution
{
	private record Card(int Id, int[] Winning, int[] Numbers);
	private record CardCopies(Card Card, int Copies = 1);

    public string Run()
    {
        string[] input = this.ReadLines();

        List<Card> cards = input.Select(ParseLine).ToList();

        long score = cards.Select(Score).Sum();

        Dictionary<int, CardCopies> cardCopies = cards.OrderBy(x => x.Id)
	        .Select(c => new CardCopies(c, 1))
	        .ToDictionary(c => c.Card.Id, c => c);

        ProcessCopies(cardCopies);

        long cardCopiesSum = cardCopies.Values.Sum(x => x.Copies);

        return score + "\n" + cardCopiesSum;
    }

    private void ProcessCopies(Dictionary<int, CardCopies> cardCopies)
    {
	    foreach ((int cardId, CardCopies card) in cardCopies)
	    {
		    long wins = Wins(card.Card);
		    for (int i = 0; i < wins; i++)
		    {
			    int copyId = cardId + i + 1;
			    if (cardCopies.TryGetValue(copyId, out CardCopies? other))
			    {
                    int newCount = other.Copies + card.Copies;
                    cardCopies[copyId] = other with { Copies = newCount};
			    }
		    }
	    }
    }

    private long Score(Card card)
    {
	    int wins = Wins(card);
	    if (wins == 0) return 0;
	    return (long)System.Numerics.BigInteger.Pow(2, wins - 1);
    }

    private int Wins(Card card)
    {
	    int wins = 0;
	    foreach (var cardNumber in card.Numbers)
	    {
		    if (card.Winning.Contains(cardNumber))
		    {
			    wins++;
		    }
	    }
	    return wins;
    }

	private Card ParseLine(string line)
    {
        LineReader reader = new LineReader(line);
        reader.ReadChars("Card");
        reader.SkipWhitespaces();
        int id = reader.ReadInt();
        reader.ReadChar(':');
        reader.SkipWhitespaces();

        List<int> winning = new();
        while (!reader.NextEquals('|'))
        {
            int number = reader.ReadInt();
            winning.Add(number);
            reader.SkipWhitespaces();
        }

        reader.ReadChar('|');
        reader.SkipWhitespaces();

        List<int> numbers = new();
        while (!reader.IsDone)
        {
	        int number = reader.ReadInt();
	        numbers.Add(number);
	        reader.SkipWhitespaces();
        }

        return new Card(id, winning.ToArray(), numbers.ToArray());
    }
}    