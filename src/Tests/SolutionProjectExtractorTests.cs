using System.IO;
using SetStartupProjects;
using Xunit;
using Xunit.Abstractions;

public class SolutionProjectExtractorTests :
    XunitApprovalBase
{
    [Fact]
    public void GetAllProjectFiles()
    {
        var allProjectFiles = SolutionProjectExtractor.GetAllProjectFiles("SampleSolution.txt");
        ObjectApprover.Verify(allProjectFiles, s => s.Replace(@"\\", @"\").Replace(Directory.GetCurrentDirectory(), ""));
    }

    public SolutionProjectExtractorTests(ITestOutputHelper output) :
        base(output)
    {
    }
}