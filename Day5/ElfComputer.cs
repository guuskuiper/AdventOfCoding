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

            Halt = 99,
        }

        public enum ParameterModes
        {
            Position = 0,
            Immediate = 1,
        }

        private List<int> Instructions;
        private int IPtr;

        public ElfComputer(IEnumerable<int> instructions)
        {
            Instructions = new List<int>(instructions);
            IPtr = 0;
        }

        public int GetData(int id)
        {
            return Instructions[id];
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
                        SetValue(IPtr + 3, GetValue(C, IPtr + 1) + GetValue(B, IPtr + 2));
                        IPtr += 4;
                        break;
                    case Opcodes.Multiply:
                        SetValue(IPtr + 3, GetValue(C, IPtr + 1) * GetValue(B, IPtr + 2));
                        IPtr += 4;
                        break;
                    case Opcodes.Input:
                        System.Console.WriteLine("Enter input: ");
                        var input = int.Parse(Console.ReadLine());
                        SetValue(IPtr + 1, input);
                        IPtr += 2;
                        break;
                    case Opcodes.Output:
                        var output = GetValue(C, IPtr + 1);
                        System.Console.WriteLine("Output: " + output);
                        IPtr += 2;
                        break;
                    case Opcodes.JumpIfTrue:
                        if(GetValue(C, IPtr + 1) != 0)
                        {
                            IPtr = GetValue(B, IPtr + 2);
                        }
                        else
                        {
                            IPtr += 3;
                        }
                        break;
                    case Opcodes.JumpIfFalse:
                        if(GetValue(C, IPtr + 1) == 0)
                        {
                            IPtr = GetValue(B, IPtr + 2);
                        }
                        else
                        {
                            IPtr += 3;
                        }
                        break;
                    case Opcodes.LessThan:
                        int result = GetValue(C, IPtr + 1) < GetValue(B, IPtr + 2) ? 1 : 0;
                        SetValue(IPtr + 3, result);
                        IPtr += 4;
                        break;
                    case Opcodes.Equals:
                        result = GetValue(C, IPtr + 1) == GetValue(B, IPtr + 2) ? 1 : 0;
                        SetValue(IPtr + 3, result);
                        IPtr += 4;
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

        private int GetValue(ParameterModes mode, int address)
        {
            var param = Instructions[address];
            switch(mode)
            {
                default:
                case ParameterModes.Position:
                    return Instructions[param];
                    break;
                case ParameterModes.Immediate:
                    return param;
                    break;
            }
        }

        private void SetValue(int address, int value)
        {
            var param = Instructions[address];
            Instructions[param] = value;
        }

        private bool ProcessInstruction(Opcodes instruction, int param0, int param1, int param2)
        {
            switch (instruction)
            {
                case Opcodes.Add:
                    Instructions[param2] = Instructions[param0] + Instructions[param1];
                    break;
                case Opcodes.Multiply:
                    Instructions[param2] = Instructions[param0] * Instructions[param1];
                    break;
                case Opcodes.Halt:
                    return true;
                    break;
                default:
                    throw new Exception("Unknown instruction: " + instruction);
                    break;
            }

            return false;
        }
    }
}