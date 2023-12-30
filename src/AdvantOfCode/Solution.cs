using JetBrains.Annotations;

namespace AdventOfCode;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
// ReSharper disable once InconsistentNaming
public interface Solution
{
    string Run();
}