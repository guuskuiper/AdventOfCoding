using Microsoft.VisualBasic;

namespace AdventOfCode.Year2022.Day20;

[DayInfo(2022, 20)]
public class Solution20 : Solution
{
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();
        long[] input = lines.Select(long.Parse).ToArray();
        //long[] input = { 1, 2, -3, 3, -2, 0, 4 };
        LinkedList<long> linkedList = new LinkedList<long>(input);
        List<LinkedListNode<long>> startNodes = CreateListOfNodes(linkedList);
        Mixing(startNodes, linkedList);
        var groveSum = GroveSum(linkedList);

        const long Key = 811589153L;
        long[] inputB = input.Select(x => x * Key).ToArray();
        
        LinkedList<long> linkedListB = new LinkedList<long>(inputB);
        List<LinkedListNode<long>> startNodesB = CreateListOfNodes(linkedListB);

        for (int i = 0; i < 10; i++)
        {
            Mixing(startNodesB, linkedListB);
            //Console.WriteLine(string.Join(',', linkedListB));
        }
        var groveSumB = GroveSum(linkedListB);
        return groveSum + "\n" + groveSumB;
    }

    private static List<LinkedListNode<T>> CreateListOfNodes<T>(LinkedList<T> linkedList)
    {
        List<LinkedListNode<T>> startNodes = new();
        LinkedListNode<T> first = linkedList.First!;
        LinkedListNode<T> current = first;
        while (current.Next != null)
        {
            startNodes.Add(current);

            current = current.Next;
        }

        startNodes.Add(current);
        return startNodes;
    }

    private long GroveSum(LinkedList<long> linkedList)
    {
        LinkedListNode<long> nulNode = linkedList.Find(0)!;
        int[] groveNumbers = { 1000, 2000, 3000 };
        long[] groves = groveNumbers.Select(x => GetGroveNumber(nulNode, x)).ToArray();
        long groveSum = groves.Sum();
        return groveSum;
    }

    private void Mixing(List<LinkedListNode<long>> startNodes, LinkedList<long> linkedList)
    {
        foreach (LinkedListNode<long> startNode in startNodes)
        {
            var dir = Math.Sign(startNode.Value);
            int toMove = (int)(Math.Abs(startNode.Value) % (linkedList.Count - 1));
            if (toMove == 0)
            {
                // nothing
            }
            else if (dir > 0)
            {
                LinkedListNode<long> next = MoveForward(startNode, toMove);
                linkedList.Remove(startNode);
                if (next != linkedList.Last)
                {
                    linkedList.AddAfter(next, startNode);
                }
                else
                {
                    linkedList.AddFirst(startNode);
                }
            }
            else
            {
                LinkedListNode<long> previous = MoveBackward(startNode, toMove);
                linkedList.Remove(startNode);
                if (previous != linkedList.First)
                {
                    linkedList.AddBefore(previous, startNode);
                }
                else
                {
                    linkedList.AddLast(startNode);
                }
            }

            //Console.WriteLine(string.Join(',', linkedList));
        }
    }

    private long GetGroveNumber(LinkedListNode<long> nulNode, int number)
    { 
        return MoveForward(nulNode, number).Value;
    }
    
    private LinkedListNode<T> MoveForward<T>(LinkedListNode<T> current, int number)
    {
        LinkedListNode<T> first = current.List!.First!;
        LinkedListNode<T> currentNode = current;
        for (int i = 0; i < number; i++)
        {
            currentNode = currentNode.Next ?? first;
        }

        return currentNode;
    }

    private LinkedListNode<T> MoveBackward<T>(LinkedListNode<T> current, int number)
    {
        LinkedListNode<T> last = current.List!.Last!;
        LinkedListNode<T> currentNode = current;
        for (int i = 0; i < number; i++)
        {
            currentNode = currentNode.Previous ?? last;
        }

        return currentNode;
    }
}
