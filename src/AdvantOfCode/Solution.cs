using JetBrains.Annotations;

namespace AdventOfCode;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
// ReSharper disable once InconsistentNaming
public interface Solution : SolutionAsync
{
    string Run();

    /// <summary>
    /// For backwards compatibility for old Solution types, make sure we can call the RunAsync method.
    /// Under the hood, it will call the Run method and return it as a Task.
    /// </summary>
    /// <returns></returns>
    Task<string> SolutionAsync.RunAsync() => Task.FromResult(Run());
}

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
// ReSharper disable once InconsistentNaming
public interface SolutionAsync
{
    Task<string> RunAsync();
}