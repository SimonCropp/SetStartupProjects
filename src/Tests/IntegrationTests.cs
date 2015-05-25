using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenMcdf;
using SetStartupProjects;

[TestFixture]
[Explicit]
public class IntegrationTests
{
    [Test]
    public void SetStartupProjectsForSample()
    {
        var solutionDirectory = Path.GetFullPath("../../../SampleSolution");
        var startupProjectGuids = new List<string>
        {
            "11111111-1111-1111-1111-111111111111",
            "22222222-2222-2222-2222-222222222222"
        };
        var startProjectSuoCreator = new StartProjectSuoCreator();
        startProjectSuoCreator.CreateForSolutionDirectory(solutionDirectory, startupProjectGuids);
    }

    [Test]
    public void SetStartupProjectsForSample2015()
    {
        var solutionDirectory = Path.GetFullPath("../../../SampleSolution");
        var startupProjectGuids = new List<string>
        {
            "11111111-1111-1111-1111-111111111111",
            "22222222-2222-2222-2222-222222222222"
        };
        var startProjectSuoCreator = new StartProjectSuoCreator();
        startProjectSuoCreator.CreateForSolutionDirectory(solutionDirectory, startupProjectGuids, VisualStudioVersions.Vs2015);
    }

    [Test]
    public void Simple()
    {
        var solutionDirectory = Path.GetFullPath("../../../SampleSolution");
        var startupProjectGuids = new StartProjectFinder()
            .GetStartProjects(solutionDirectory)
            .ToList();
        var startProjectSuoCreator = new StartProjectSuoCreator();
        startProjectSuoCreator.CreateForSolutionDirectory(solutionDirectory, startupProjectGuids);
    }

    [Test]
    public void ReadUsingUtf16()
    {
        var suoPath = "../../../SampleSolution/SampleSolution.v12.suo";
        var utf16 = Encoding.GetEncodings()
            .Single(x => x.Name == "utf-16")
            .GetEncoding();
        using (var solutionStream = File.OpenRead(suoPath))
        using (var compoundFile = new CompoundFile(solutionStream, UpdateMode.ReadOnly, true, true, false))
        {
            var configStream = compoundFile.RootStorage.GetStream("SolutionConfiguration");
            var bytes = configStream.GetData();
            Debug.WriteLine(utf16.GetString(bytes));
        }
    }

    [Test]
    public void SaveToFile()
    {
        var suoPath = "../../../SampleSolution/SampleSolution.v12.suo";
        using (var solutionStream = File.OpenRead(suoPath))
        using (var compoundFile = new CompoundFile(solutionStream, UpdateMode.ReadOnly, true, true, false))
        {
            var configStream = compoundFile.RootStorage.GetStream("SolutionConfiguration");
            var bytes = configStream.GetData();

            var utf16 = Encoding.GetEncodings()
                .Single(x => x.Name == "utf-16")
                .GetEncoding();
            File.WriteAllText("temp.txt", utf16.GetString(bytes), utf16);
        }
    }
}