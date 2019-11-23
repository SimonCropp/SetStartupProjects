using System.Threading.Tasks;
using SetStartupProjects;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class SolutionProjectExtractorTests :
    VerifyBase
{
    [Fact]
    public Task GetAllProjectFiles()
    {
        var allProjectFiles = SolutionProjectExtractor.GetAllProjectFiles("SampleSolution.txt");
        return Verify(allProjectFiles);
    }

    public SolutionProjectExtractorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}