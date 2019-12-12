using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Day2
{
    public class ElfComputer
    {
        public enum Opcodes
        {
            Add = 1,
            Multiply = 2,

            Halt = 99,
        }

        private List<int> Instructions;

        public ElfComputer(IEnumerable<int> instructions)
        {
            Instructions = new List<int>(instructions);
        }

        public int GetData(int id)
        {
            return Instructions[id];
        }

        public void ProcessInstructions()
        {
            for (int currentInstructionId = 0; currentInstructionId < Instructions.Count; currentInstructionId += 4)
            {
                Opcodes instruction = (Opcodes) Instructions[currentInstructionId + 0];
                var arg0 = Instructions[currentInstructionId + 1];
                var arg1 = Instructions[currentInstructionId + 2];
                var arg2 = Instructions[currentInstructionId + 3];
                if(ProcessInstruction(instruction, arg0, arg1, arg2)) break;
            }

            //Console.WriteLine(string.Join(',', Instructions));
        }

        private bool ProcessInstruction(Opcodes instruction, int arg0, int arg1, int arg2)
        {
            switch (instruction)
            {
                case Opcodes.Add:
                    Instructions[arg2] = Instructions[arg0] + Instructions[arg1];
                    break;
                case Opcodes.Multiply:
                    Instructions[arg2] = Instructions[arg0] * Instructions[arg1];
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