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
        List<long> cpuInstructions = instruction.ToList();
        computers = new List<NetworkElfComputer> ();
        foreach(var cpuId in Enumerable.Range(0, cpuCount))
        {
            computers.Add(new NetworkElfComputer(cpuInstructions, cpuId, Route));
        }
    }

    public long Boot()
    {
        foreach(var computer in  computers)
        {
            computer.Boot();
        }

        return RunNAT();
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
        //Console.WriteLine("NAT received " + packet);
        NATpacket = packet;
        if (FirstPacket is null) FirstPacket = packet;
    }
    
    private long RunNAT()
    {
        long prevY = long.MaxValue;
        while(true)
        {
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
                        return prevY;
                    }
                    prevY = NATpacket.Y;
                    //Console.WriteLine($"Sending NAT {NATpacket}");
                    computers[0].Enqueue(NATpacket);
                }
            }
        }
    }
}