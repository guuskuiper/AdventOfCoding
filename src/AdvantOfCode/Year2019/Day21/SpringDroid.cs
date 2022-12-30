namespace AdventOfCode.Year2019.Day21;

class SpringDroid
{
    private ElfComputer computer;
    private string inputData;
    private int inputCount;

    public SpringDroid(IEnumerable<long> input)
    {
        computer = new ElfComputer(input, Input, Output);
    }

    public long OutputValue;

    private long Input()
    {
        var ch = inputData[inputCount];
        inputCount++;
        return Convert.ToInt32(ch);
    }

    private void Output(long output)
    {
        if(output > 0x7F)
        {
            OutputValue = output;
        }
    }

    public void Start(string inputs)
    {
        inputData = inputs;
        inputCount = 0;

        computer.ProcessInstructions();
    }
}