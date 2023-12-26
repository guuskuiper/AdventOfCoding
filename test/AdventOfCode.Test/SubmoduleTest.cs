using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace AdventOfCode.Test;

public class SubmoduleTest
{
    [Fact]
    public void InputReadmeExists()
    {
        string path = CallingFilePath();
        string readmePath = Path.Combine(path, "..", "..", "input", "README.md");
        string readmePathAbs = Path.GetFullPath(readmePath);
        
        bool exists = File.Exists(readmePathAbs);
        
        Assert.True(exists);
    }

    private string CallingFilePath([CallerFilePath] string path = "")
    {
        return Path.GetDirectoryName(path)!;
    }
}