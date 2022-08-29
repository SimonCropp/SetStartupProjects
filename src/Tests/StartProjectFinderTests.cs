using System.Xml.Linq;
using SetStartupProjects;

public class StartProjectFinderTests
{
    [Fact]
    public void Exe_from_OutputType()
    {
        var projectText = File.ReadAllText("OutputType_Exe.txt");
        var finder = new StartProjectFinder();
        Assert.True(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void WebApplication_webSdk()
    {
        var projectText = File.ReadAllText("WebApplication_websdk.txt");
        var finder = new StartProjectFinder();
        Assert.True(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void GetStartupProjectsWithDefault()
    {
        var finder = new StartProjectFinder();
        var startupProjects = finder.GetStartProjects("SimpleSolutionWithDefault/SimpleSolution.sln");
        Assert.Equal("11111111-1111-1111-1111-111111111111", startupProjects.Single());
    }

    [Fact]
    public void Conditional_Exe_from_OutputType()
    {
        var projectText = File.ReadAllText("OutputType_Conditional_Exe.txt");
        var finder = new StartProjectFinder();
        Assert.True(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void WinExe_from_OutputType()
    {
        var projectText = File.ReadAllText("OutputType_WinExe.txt");
        var finder = new StartProjectFinder();
        Assert.True(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void StartActionIsProgram()
    {
        var projectText = File.ReadAllText("StartActionIsProgram.txt");
        var finder = new StartProjectFinder();
        Assert.True(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void Lib_from_OutputType()
    {
        var projectText = File.ReadAllText("OutputType_Lib.txt");
        var finder = new StartProjectFinder();
        Assert.False(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void WebApplication_xproj()
    {
        var projectText = File.ReadAllText("WebApplication_xproj.txt");
        var finder = new StartProjectFinder();
        Assert.False(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void Multiple_excluded_project_types()
    {
        var projectText = File.ReadAllText("Multiple_Exclude.txt");
        var finder = new StartProjectFinder();
        Assert.False(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void Multiple_include_project_types()
    {
        var projectText = File.ReadAllText("Multiple_Include.txt");
        var finder = new StartProjectFinder();
        Assert.True(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }

    [Fact]
    public void Lower_project_types()
    {
        var projectText = File.ReadAllText("Lower_Include.txt");
        var finder = new StartProjectFinder();
        Assert.True(finder.ShouldIncludeProjectXml(XDocument.Parse(projectText), "/dir/project.csproj"));
    }
}