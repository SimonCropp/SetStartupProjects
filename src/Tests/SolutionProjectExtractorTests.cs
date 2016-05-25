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
        var samplesolutionTxt = Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleSolution.txt");
        var directoryName = Path.GetDirectoryName(samplesolutionTxt);
        var allProjectFiles = SolutionProjectExtractor.GetAllProjectFiles(samplesolutionTxt);
        ObjectApprover.VerifyWithJson(allProjectFiles, s => s.Replace(@"\\", @"\").Replace(directoryName, ""));
    }
}