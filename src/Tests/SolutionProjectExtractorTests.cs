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
        var samplesolutionTxt = Path.GetFullPath("SampleSolution.txt");
        var directoryName = Path.GetDirectoryName(samplesolutionTxt);
        var allProjectFiles = SolutionProjectExtractor.GetAllProjectFiles(samplesolutionTxt);
        ObjectApprover.VerifyWithJson(allProjectFiles, s => s.Replace(@"\\", @"\").Replace(directoryName, ""));
    }
}