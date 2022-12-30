using System.Collections;
using System.Text;

namespace AdventOfCode.Year2019.Day25;

class ASCII
{
    private const char NEWLINE = '\n';
    private ElfComputer computer;

    private string inputData;
    private int inputCount;

    public ASCII(IEnumerable<long> instructions)
    {
        computer = new ElfComputer(instructions, Input, Output);
    }
    
    public StringBuilder Builder = new();

    public void Start()
    {
        computer.ProcessInstructions();
    }

    public void Start(string commands)
    {
        inputCount = 0;
        var input = commands.Replace(Environment.NewLine, NEWLINE.ToString());

        var options = string.Join("west\n", GenerateOptions());

        inputData = input + options;
        Start();
    }

    public long Input()
    {
        int input;
        if(inputCount < inputData.Length)
        {
            input = Convert.ToInt32(inputData[inputCount]);
        }
        else
        {
            input = Console.Read();
        }
        inputCount++;
        return input;
    }


    public void Output(long output)
    {
        if(output > 0x7F)
        {
            return;
        }
        
        var ch = Convert.ToChar(output);

        Builder.Append(ch);
    }

    private IEnumerable<string> GenerateOptions()
    {
        string[] items =  
        {
            "jam", 
            "bowl of rice",
            "antenna",
            "manifold",
            "hypercube",
            "dehydrated water",
            "candy cane",
            "dark matter",
        };
        var options = Enumerable.Range(0, (int)Math.Pow(2, items.Length));
        foreach(var option in options)            
        {
            var sb = new StringBuilder();
            var bits = new BitArray(BitConverter.GetBytes(option));
            for(int i = 0; i < items.Length; i++)
            {
                sb.AppendLine( (bits[i] ? "take" : "drop") + " " + items[i]);
            }
            yield return sb.ToString().Replace(Environment.NewLine, NEWLINE.ToString());
        }
    }
}