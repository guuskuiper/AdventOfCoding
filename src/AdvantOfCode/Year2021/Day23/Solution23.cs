namespace AdventOfCode.Year2021.Day23;

public class Solution23 : Solution
{
    public string Run()
    {
        long A = PartA();
        long B = PartB();
        Console.WriteLine(_path);
        return A + "\n" + B; // B: > 47200 and < 47236 (not 47230)
    }

    private int PartA()
    {
        int a = 6 + 3 + 9 + 2;
        int b = 5 + 3 + 6;
        int c = 5 + 5;
        int d = 9 + 9;

        return a * 1 + b * 10 + c * 100 + d * 1000;
    }
    
    private long PartB()
    {
        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;

        a += 7;
        b += 5;
        a += 8;
        b += 5;
        c += 7;
        c += 7;
        b += 4;
        c += 8;
        b += 5;
        b += 6;
        b += 7;
        b += 6;
        a += 4;
        c += 6;
        a += 5;
        d += 11;
        d += 11;
        d += 11;
        d += 11;
        a += 5;
        a += 5;
        a += 9;
        a += 9;

        return a * 1 + b * 10 + c * 100 + d * 1000;
    }

    private string _path = @"
#############
#...........#
###D#C#A#B###
  #D#C#B#A#
  #D#B#A#C#
  #D#C#B#A#
  #########

#############
#A..........#
###D#C#.#B###
  #D#C#B#A#
  #D#B#A#C#
  #D#C#B#A#
  #########
A +7
#############
#A........B.#
###D#C#.#B###
  #D#C#.#A#
  #D#B#A#C#
  #D#C#B#A#
  #########
B +5 
#############
#AA.......B.#
###D#C#.#B###
  #D#C#.#A#
  #D#B#.#C#
  #D#C#B#A#
  #########
A +8
#############
#AA.....B.B.#
###D#C#.#B###
  #D#C#.#A#
  #D#B#.#C#
  #D#C#.#A#
  #########
B +5  
#############
#AA.....B.B.#
###D#.#.#B###
  #D#.#.#A#
  #D#B#C#C#
  #D#C#C#A#
  #########
C +7
C +7
#############
#AA.B...B.B.#
###D#.#.#B###
  #D#.#.#A#
  #D#.#C#C#
  #D#C#C#A#
  #########
B +4   
#############
#AA.B...B.B.#
###D#.#.#B###
  #D#.#C#A#
  #D#.#C#C#
  #D#.#C#A#
  #########
C +8  
#############
#AA.........#
###D#B#.#.###
  #D#B#C#A#
  #D#B#C#C#
  #D#B#C#A#
  #########
B +5
B +6
B +7
B +6 
#############
#AA........A#
###D#B#.#.###
  #D#B#C#.#
  #D#B#C#C#
  #D#B#C#A#
  #########
A +4
#############
#A........AA#
###D#B#C#.###
  #D#B#C#.#
  #D#B#C#.#
  #D#B#C#A#
  #########
C +6
#############
#AA.......AA#
###D#B#C#.###
  #D#B#C#.#
  #D#B#C#.#
  #D#B#C#.#
  #########
A +5
#############
#AA.......AA#
###.#B#C#D###
  #.#B#C#D#
  #.#B#C#D#
  #.#B#C#D#
  #########
D +11
D +11
D +11
D +11
#############
#AA.......AA#
###.#B#C#D###
  #.#B#C#D#
  #.#B#C#D#
  #.#B#C#D#
  #########
A +5
A +5
A +9
A +9";

}

