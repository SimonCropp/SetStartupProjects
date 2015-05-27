using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using ObjectApproval;
using Resourcer;
using SetStartupProjects;

[TestFixture]
public class StartProjectFinderTests
{
    [Test]
    public void Exe_from_OutputType()
    {
        var projectText = Resource.AsString("OutputType_Exe.txt");
        Assert.IsTrue(new StartProjectFinder().ShouldIncludeProjectXml(XDocument.Parse(projectText),""));
    }

    [Test]
    public void WinExe_from_OutputType()
    {
        var projectText = Resource.AsString("OutputType_WinExe.txt");
        string guid;
        Assert.IsTrue(new StartProjectFinder().ShouldIncludeProjectXml(XDocument.Parse(projectText), ""));
    }

    [Test]
    public void StartActionIsProgram()
    {
        var projectText = Resource.AsString("StartActionIsProgram.txt");
        Assert.IsTrue(new StartProjectFinder().ShouldIncludeProjectXml(XDocument.Parse(projectText), ""));
    }

    [Test]
    public void Lib_from_OutputType()
    {
        var projectText = Resource.AsString("OutputType_Lib.txt");
        Assert.IsFalse(new StartProjectFinder().ShouldIncludeProjectXml(XDocument.Parse(projectText), ""));
    }

    [Test]
    public void Multiple_excluded_project_types()
    {
        var projectText = Resource.AsString("Multiple_Exclude.txt");
        Assert.IsFalse(new StartProjectFinder().ShouldIncludeProjectXml(XDocument.Parse(projectText), ""));
    }

    [Test]
    public void Multiple_include_project_types()
    {
        var projectText = Resource.AsString("Multiple_Include.txt");
        Assert.IsTrue(new StartProjectFinder().ShouldIncludeProjectXml(XDocument.Parse(projectText), ""));
    }

    [Test]
    public void Lower_project_types()
    {
        var projectText = Resource.AsString("Lower_Include.txt");
        Assert.IsTrue(new StartProjectFinder().ShouldIncludeProjectXml(XDocument.Parse(projectText), ""));
    }

    [Test]
    public void GetAllProjectFiles()
    {
        var samplesolutionTxt = Path.GetFullPath("SampleSolution.txt");
        var allProjectFiles = new StartProjectFinder().GetAllProjectFiles(samplesolutionTxt);
        var directoryName = Path.GetDirectoryName(samplesolutionTxt);
        ObjectApprover.VerifyWithJson(allProjectFiles,s => s.Replace(@"\\",@"\").Replace(directoryName,""));
    }
}