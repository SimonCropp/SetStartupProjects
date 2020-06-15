using System.Threading.Tasks;
using SetStartupProjects;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class SolutionProjectExtractorTests
{
    [Fact]
    public Task GetAllProjectFiles()
    {
        var allProjectFiles = SolutionProjectExtractor.GetAllProjectFiles("SampleSolution.txt");
        return Verifier.Verify(allProjectFiles);
    }
}