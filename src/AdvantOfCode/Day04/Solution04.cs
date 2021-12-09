using System.Text;

namespace AdventOfCode.Day04;

public class Solution04 : Solution
{
    public class Board
    {
        public const int SIZE = 5;
        public BoardValue[,] Value = new BoardValue[SIZE, SIZE];
        public bool Win = false;
    }

    public class BoardValue
    {
        public int Number;
        public bool Mark;
    }

    private List<Board> boards;

    public string Run()
    {
        List<string> input = InputReader.ReadFileLines();

        List<int> drawNumbers = input[0].Split(',',StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

        CreateBoards(input.Skip(1).ToList());
        int score = Play(drawNumbers);

        CreateBoards(input.Skip(1).ToList());
        int scoreLast = PlayLast(drawNumbers);

        return score + "\n" + scoreLast;
    }

    private int Play(List<int> drawNumbers)
    {
        foreach (var number in drawNumbers)
        {
            foreach (Board board in boards)
            {
                MarkBoard(board, number);
                if (CheckWinner(board))
                {
                    Console.WriteLine(Board2String(board));
                    return GetScore(board, number);
                }
            }
        }

        return -1;
    }

    private int PlayLast(List<int> drawNumbers)
    {
        int winners = 0;
        foreach (var number in drawNumbers)
        {
            foreach (Board board in boards)
            {
                MarkBoard(board, number);
                if (!board.Win && CheckWinner(board))
                {
                    winners++;
                    board.Win = true;
                    if(winners == boards.Count)
                    {
                        Console.WriteLine(Board2String(board));
                        return GetScore(board, number);
                    }
                }
            }
        }

        return -1;
    }

    private void CreateBoards(List<string> input)
    {
        boards = new List<Board>();
        for (int i = 0; i < input.Count; i+=Board.SIZE)
        {
            boards.Add(CreateBoard(input.Skip(i).Take(Board.SIZE).ToList()));
        }
    }

    private Board CreateBoard(List<string> boardInput)
    {
        Board board = new Board();
        for (int row = 0; row < Board.SIZE; row++)
        {
            int[] rowNumbers = boardInput[row].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            for (int col = 0; col < Board.SIZE; col++)
            {
                board.Value[row, col] = new BoardValue 
                { 
                    Number = rowNumbers[col], 
                    Mark = false 
                };
            }
        }

        return board;
    }

    private void MarkBoard(Board board, int number)
    {
        for (int row = 0; row < Board.SIZE; row++)
        {
            for (int col = 0; col < Board.SIZE; col++)
            {
                BoardValue boardValue = board.Value[row, col];
                if (boardValue.Number == number)
                {
                    boardValue.Mark = true;
                }
            }
        }
    }

    private bool CheckWinner(Board board)
    {
        bool winner = false;
        winner |= CheckRows(board);
        winner |= CheckCols(board);
        return winner;
    }

    private bool CheckRows(Board board)
    {
        return Check(board, (board, i, j) => board.Value[i, j].Mark);
    }

    private bool CheckCols(Board board)
    {
        return Check(board, (board, i, j) => board.Value[j, i].Mark);
    }

    private bool Check(Board board, Func<Board, int, int, bool> getMark)
    {
        bool winner = false;
        for (int i = 0; i < Board.SIZE; i++)
        {
            bool nonMarked = false;
            for (int j = 0; j < Board.SIZE; j++)
            {
                if (!getMark(board, i, j))
                {
                    nonMarked = true;
                    break;
                }
            }

            if (!nonMarked)
            {
                winner = true;
                break;
            }
        }

        return winner;
    }

    private int GetScore(Board board, int winningNumber)
    {
        int score = 0;
        for (int i = 0; i < Board.SIZE; i++)
        {
            for (int j = 0; j < Board.SIZE; j++)
            {
                BoardValue boardValue = board.Value[i, j];
                if (!boardValue.Mark)
                {
                    score += boardValue.Number;
                }
            }
        }

        return score * winningNumber;
    }

    private string Board2String(Board board)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < Board.SIZE; i++)
        {
            for (int j = 0; j < Board.SIZE; j++)
            {
                BoardValue boardValue = board.Value[i, j];
                sb.AppendFormat("{0:D2}{1}", boardValue.Number, boardValue.Mark ? '*' : '.');
                if(j < Board.SIZE - 1) 
                { 
                    sb.Append(' '); 
                };
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
