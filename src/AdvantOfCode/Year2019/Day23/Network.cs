using System.Collections.Concurrent;

namespace AdventOfCode.Year2019.Day23;

public class Network
{
    private const int NAT = 255;
    private List<NetworkElfComputer> computers;
    private Packet NATpacket;
    public  Packet FirstPacket;

    public Network(IEnumerable<long> instruction, int cpuCount)
    {   
        computers = new List<NetworkElfComputer> ();
        foreach(var cpuId in Enumerable.Range(0, cpuCount))
        {
            computers.Add(new NetworkElfComputer(instruction.ToList(), cpuId, Route));
        }
    }

    public long Boot()
    {
        List<Task> tasks = new List<Task>();
        foreach(var computer in  computers)
        {
            var task = computer.Boot();
            tasks.Add(task);
        }

        long value = RunNAT();

        Task.WaitAll(tasks.ToArray());
        return value;
    }

    public void Route(Packet packet, long destination, long from)
    {
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
            Console.WriteLine($"Invalid address {destination} for packet ({packet})");
        }
    }

    private void RouteToNAT(Packet packet)
    {
        Console.WriteLine("NAT received " + packet);
        NATpacket = packet;
        if (FirstPacket is null) FirstPacket = packet;
    }

    private long RunNAT()
    {
        Console.WriteLine("Starting NAT");
        long prevY = long.MaxValue;
        //return;
        Task.Run(
            async () => 
            {
                while(true)
                {
                    await Task.Delay(1_000);
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
                                throw new Exception("Done" + prevY);
                            }
                            prevY = NATpacket.Y;
                            Console.WriteLine($"Sending NAT {NATpacket}");
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
        return prevY;
    }

}