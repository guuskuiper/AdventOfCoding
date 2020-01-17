using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Day5;

namespace Day23
{
    public class Network
    {
        private const int NAT = 255;
        private List<NetworkElfComputer> computers;
        private Packet NATpacket;

        public Network(IEnumerable<long> instruction, int cpuCount)
        {   
            computers = new List<NetworkElfComputer> ();
            foreach(var cpuId in Enumerable.Range(0, cpuCount))
            {
                computers.Add(new NetworkElfComputer(instruction.ToList(), cpuId, Route));
            }
        }

        public void Boot()
        {
            List<Task> tasks = new List<Task>();
            foreach(var computer in  computers)
            {
                var task = computer.Boot();
                tasks.Add(task);
            }

            RunNAT();

            Task.WaitAll(tasks.ToArray());
        }

        public void Route(Packet packet, long destination, long from)
        {
            //System.Console.WriteLine($"Routing adress {destination} from {from} for packet ({packet})");
            if(destination < computers.Count)
            {
                computers[(int)destination].Enqueue(packet);
            }
            else if(destination == NAT)
            {
                RouteToNAT(packet);
            }
            else
            {
                System.Console.WriteLine($"Invalid adress {destination} for packet ({packet})");
            }
        }

        private void RouteToNAT(Packet packet)
        {
            System.Console.WriteLine("NAT received " + packet);
            NATpacket = packet;
        }

        private void RunNAT()
        {
            System.Console.WriteLine("Starting NAT");
            long prevY = long.MaxValue;
            Task.Run(
                async () => 
                {
                    while(true)
                    {
                        await Task.Delay(1000);
                        if(NATpacket != null)
                        {
                            bool idle = true;
                            foreach(var cpu in computers)
                            {
                                idle &= cpu.IsIdle();
                            }
                            if(idle)
                            {
                                if(NATpacket.Y == prevY)
                                {
                                    System.Console.WriteLine("DONE" + prevY);
                                    throw new Exception("DONE" + prevY);
                                }
                                prevY = NATpacket.Y;
                                System.Console.WriteLine($"Sending NAT {NATpacket}");
                                computers[0].Enqueue(NATpacket);
                                await Task.Yield();
                            }
                            else
                            {
                                //System.Console.WriteLine("NAT YIELD");
                                await Task.Yield();
                            }
                        }
                        else
                        {
                            await Task.Yield();
                        }
                    }
                }
            );
        }
    }
}