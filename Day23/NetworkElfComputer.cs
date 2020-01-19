using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Day5;

namespace Day23
{
    public class NetworkElfComputer
    {
        private ElfComputer Cpu;
        private ConcurrentQueue<Packet> NetworkQueue;
        private int Id;
        private int inputCount;
        private List<long> outputs;
        public Action<Packet, long, long> RouteAction;
        private Task task;
        private bool useX = true;
        private bool idle = false;

        public NetworkElfComputer(IEnumerable<long> instructions, int id, Action<Packet, long, long> route)
        {
            Id = id;
            Cpu = new ElfComputer(instructions, InputAsyncWrapper, Output);
            NetworkQueue = new ConcurrentQueue<Packet>();
            RouteAction = route;
            outputs= new List<long>();
        }

        public Task Boot()
        {
            System.Console.WriteLine($"Booting {Id}");
            task = Task.Run( () => Cpu.ProcessInstructions() );
            return task;
        }

        public long Input()
        {
            long input;
            if(inputCount == 0)
            {
                input = Id;
            }
            else
            {
                if(!NetworkQueue.IsEmpty)
                {
                    idle = false;
                    if(useX)
                    {
                        if(NetworkQueue.TryPeek(out var result))
                        {
                            input = result.X;
                            useX = false;
                        }
                        else
                        {
                            input = -1;
                        }
                    }
                    else
                    {
                        if(NetworkQueue.TryDequeue(out var result))
                        {
                            input = result.Y;
                            useX = true;
                        }
                        else
                        {
                            input = -1;
                        }
                    }
                }
                else
                {
                    input = -1;
                }
            }
            inputCount++;

            return input;
        }

        public long InputAsyncWrapper()
        {
            var t = Task.Run(async () => await InputAsync());
            return t.Result;
        }

        public async Task<long> InputAsync()
        {
            var input = Input();
            if(input == -1)
            {
                idle = true;
                await Task.Yield();
            }
            return await Task.FromResult<long>(input);
        }

        public void Output(long output)
        {
            idle = false;
            outputs.Add(output);
            if(outputs.Count == 3)
            {
                RouteAction(new Packet {X = outputs[1], Y = outputs[2]}, outputs[0], Id);
                outputs.Clear();
            }
        }

        public void Enqueue(Packet packet)
        {
            NetworkQueue.Enqueue(packet);
        }

        public bool IsIdle()
        {
            return NetworkQueue.IsEmpty && idle;
        }
    }
}
