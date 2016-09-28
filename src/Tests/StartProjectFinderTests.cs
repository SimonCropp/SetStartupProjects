using System.Xml.Linq;
using NUnit.Framework;
using Resourcer;
using SetStartupProjects;

[TestFixture]
public class StartProjectFinderTests
{
    [Test]
    public void Exe_from_OutputType()
    {
        var projectText = Resource.AsString("OutputType_Exe.txt");
        var finder = new StartProjectFinder();
        Assert.IsTrue(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Test]
    public void WinExe_from_OutputType()
    {
        var projectText = Resource.AsString("OutputType_WinExe.txt");
        var finder = new StartProjectFinder();
        Assert.IsTrue(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Test]
    public void StartActionIsProgram()
    {
        var projectText = Resource.AsString("StartActionIsProgram.txt");
        var finder = new StartProjectFinder();
        Assert.IsTrue(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Test]
    public void Lib_from_OutputType()
    {
        var projectText = Resource.AsString("OutputType_Lib.txt");
        var finder = new StartProjectFinder();
        Assert.IsFalse(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Test]
    public void WebApplication_xproj()
    {
        var projectText = Resource.AsString("WebApplication_xproj.txt");
        var finder = new StartProjectFinder();
        Assert.IsFalse(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Test]
    public void Multiple_excluded_project_types()
    {
        var projectText = Resource.AsString("Multiple_Exclude.txt");
        var finder = new StartProjectFinder();
        Assert.IsFalse(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Test]
    public void Multiple_include_project_types()
    {
        var projectText = Resource.AsString("Multiple_Include.txt");
        var finder = new StartProjectFinder();
        Assert.IsTrue(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Test]
    public void Lower_project_types()
    {
        var projectText = Resource.AsString("Lower_Include.txt");
        var finder = new StartProjectFinder();
        Assert.IsTrue(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }
}