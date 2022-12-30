namespace AdventOfCode.Year2019.Day06;

public class Orbit
{
    public string Name;
    public List<Orbit> Children;

    public Orbit(string name)
    {
        Name = name;

        Children = new List<Orbit> ();
    }

    public int Traverse(int depth)
    {
        var currentSum = depth;
        foreach(var child in Children)
        {
            currentSum += child.Traverse(depth + 1);
        }

        return currentSum;
    }

    public List<Orbit> PathTo(List<Orbit> current, string target)
    {
        var newList = new List<Orbit> (current);
        newList.Add(this);

        if(Name == target)
        {
            Console.WriteLine(string.Join(')', newList.Select(x => x.Name)));
            return newList;
        }

        foreach(var child in Children)
        {
            var res = child.PathTo(newList, target);
            if(res != null)
            {
                return res;
            }
        }

        return null;
    }
}

public class Orbits
{
    Dictionary<string, Orbit> CurrentOrbits;

    public Orbits(IEnumerable<string> orbits)
    {
        CurrentOrbits = new Dictionary<string, Orbit> ();
        var com = new Orbit("COM");
        CurrentOrbits.Add("COM", com);

        foreach(var orbit in orbits)
        {
            var orbitNames = orbit.Split(')');

            var leftName = orbitNames[0];
            Orbit leftOrbit;
            if(!CurrentOrbits.ContainsKey(leftName))
            {
                leftOrbit = new Orbit(leftName);
                CurrentOrbits.Add(leftName, leftOrbit);
            }
            else
            {
                leftOrbit = CurrentOrbits[leftName];
            }

            var rightName = orbitNames[1];
            Orbit rightOrbit;
            if(!CurrentOrbits.ContainsKey(rightName))
            {
                rightOrbit = new Orbit(rightName);
                CurrentOrbits.Add(rightName, rightOrbit);
            }
            else
            {
                rightOrbit = CurrentOrbits[rightName];
            }

            leftOrbit.Children.Add(rightOrbit);
        }
    }

    public int GetOrbitCount()
    {
        var start = CurrentOrbits["COM"];
        return start.Traverse(0);
    }

    public void GetPathTo(string target)
    {
        var start = CurrentOrbits["COM"];
        start.PathTo(new List<Orbit> (), target);
    }

    public int GetPathBetween(string from, string to)
    {
        var start = CurrentOrbits["COM"];
        var toFrom = start.PathTo(new List<Orbit> (), from);
        var toTo = start.PathTo(new List<Orbit> (), to);

        var min = Math.Min(toFrom.Count, toTo.Count);
        Orbit last;
        int lastCount = 0;
        for(int i = 0; i < min; i++)
        {
            if(toFrom[i] == toTo[i])
            {
                last = toFrom[i];
                lastCount = i;
            }
        }

        var fromLastCount = toFrom.Count - 2 - lastCount;
        var toLastCount = toTo.Count - 2 - lastCount;
        var total = fromLastCount + toLastCount;
        return total;
    }
}