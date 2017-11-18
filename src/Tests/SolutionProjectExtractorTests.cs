using System.IO;
using NUnit.Framework;
using ObjectApproval;
using SetStartupProjects;

[TestFixture]
public class SolutionProjectExtractorTests
{
    [Test]
    public void GetAllProjectFiles()
    {
        var sampleSolutionTxt = Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleSolution.txt");
        var directoryName = Path.GetDirectoryName(sampleSolutionTxt);
        var allProjectFiles = SolutionProjectExtractor.GetAllProjectFiles(sampleSolutionTxt);
        ObjectApprover.VerifyWithJson(allProjectFiles, s => s.Replace(@"\\", @"\").Replace(directoryName, ""));
    }
}