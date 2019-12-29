using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Day5
{
    public class ElfComputer
    {
        public enum Opcodes
        {
            Add = 1,
            Multiply = 2,
            Input = 3,
            Output = 4,
            JumpIfTrue = 5,
            JumpIfFalse = 6,
            LessThan = 7,
            Equals = 8,
            RelativeBase = 9,

            Halt = 99,
        }

        public enum ParameterModes
        {
            Position = 0,
            Immediate = 1,
            Relative = 2,
        }

        private List<long> Instructions;
        private int IPtr;
        private int RelativeBase;

        public Func<long> InputFunction;
        public Action<long> OutputAction;

        public ElfComputer(IEnumerable<long> instructions, Func<long> input, Action<long> output)
        {
            InputFunction = input;
            OutputAction = output;
            Instructions = new List<long>(instructions);
            IPtr = 0;
            RelativeBase = 0;
        }

        public long GetData(int id)
        {
            if(id < 0) throw new Exception("Negative adress accessed");
            if(id >= Instructions.Count)
            {
                while(id >= Instructions.Count)
                {
                    Instructions.Add(default(long));
                }
            }
            return Instructions[id];
        }

        public void SetData(int id, long value)
        {
            if(id < 0) throw new Exception("Negative adress accessed");
            if(id >= Instructions.Count)
            {
                while(id >= Instructions.Count)
                {
                    Instructions.Add(default(long));
                }
            }
            Instructions[id] = value;
        }

        public void ProcessInstructions()
        {
            while(true)
            {
                var currentInstruction = Instructions[IPtr];

                Opcodes instruction = (Opcodes) (currentInstruction % 100);
                var C = (ParameterModes)((currentInstruction / 100)   % 10);
                var B = (ParameterModes)((currentInstruction / 1000)  % 10); 
                var A = (ParameterModes)((currentInstruction / 10000) % 10); 
                switch (instruction)
                {
                    case Opcodes.Add:
                        SetValue(A, IPtr + 3, GetValue(C, IPtr + 1) + GetValue(B, IPtr + 2));
                        IPtr += 4;
                        break;
                    case Opcodes.Multiply:
                        SetValue(A, IPtr + 3, GetValue(C, IPtr + 1) * GetValue(B, IPtr + 2));
                        IPtr += 4;
                        break;
                    case Opcodes.Input:
                        var input = InputFunction();
                        SetValue(C, IPtr + 1, input);
                        IPtr += 2;
                        break;
                    case Opcodes.Output:
                        var output = GetValue(C, IPtr + 1);
                        OutputAction(output);
                        IPtr += 2;
                        break;
                    case Opcodes.JumpIfTrue:
                        if(GetValue(C, IPtr + 1) != 0)
                        {
                            IPtr = (int)GetValue(B, IPtr + 2);
                        }
                        else
                        {
                            IPtr += 3;
                        }
                        break;
                    case Opcodes.JumpIfFalse:
                        if(GetValue(C, IPtr + 1) == 0)
                        {
                            IPtr = (int)GetValue(B, IPtr + 2);
                        }
                        else
                        {
                            IPtr += 3;
                        }
                        break;
                    case Opcodes.LessThan:
                        int result = GetValue(C, IPtr + 1) < GetValue(B, IPtr + 2) ? 1 : 0;
                        SetValue(A, IPtr + 3, result);
                        IPtr += 4;
                        break;
                    case Opcodes.Equals:
                        result = GetValue(C, IPtr + 1) == GetValue(B, IPtr + 2) ? 1 : 0;
                        SetValue(A, IPtr + 3, result);
                        IPtr += 4;
                        break;
                    case Opcodes.RelativeBase:
                        RelativeBase += (int)GetValue(C, IPtr + 1);
                        IPtr += 2;
                        break;
                    case Opcodes.Halt:
                        IPtr += 1;
                        return;
                    default:
                        throw new Exception("Unknown instruction: " + instruction);
                }

            }
            //Console.WriteLine(string.Join(',', Instructions));
        }

        private long GetValue(ParameterModes mode, int address)
        {
            var param = Instructions[address];
            switch(mode)
            {
                default:
                case ParameterModes.Position:
                    return GetData((int)param);
                    break;
                case ParameterModes.Immediate:
                    return param;
                    break;
                case ParameterModes.Relative:
                    return GetData(RelativeBase + (int)param);
                    break;
            }
        }

        private void SetValue(ParameterModes mode, int address, long value)
        {
            var param = Instructions[address];
            switch(mode)
            {
                default:
                case ParameterModes.Position:
                    SetData((int)param, value);
                    break;
                case ParameterModes.Immediate:
                    throw new Exception("Invalid instruction, using immediate mode");
                    break;
                case ParameterModes.Relative:
                    SetData(RelativeBase + (int)param, value);
                    break;
            }
        }
    }
}