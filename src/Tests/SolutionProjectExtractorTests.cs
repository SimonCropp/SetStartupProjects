using System.IO;
using ObjectApproval;
using SetStartupProjects;
using Xunit;

public class SolutionProjectExtractorTests
{
    [Fact]
    public void GetAllProjectFiles()
    {
        var allProjectFiles = SolutionProjectExtractor.GetAllProjectFiles("SampleSolution.txt");
        ObjectApprover.VerifyWithJson(allProjectFiles, s => s.Replace(@"\\", @"\").Replace(Directory.GetCurrentDirectory(), ""));
    }
}