public class StartProjectFinderTests
{
    [Fact]
    public void Exe_from_OutputType()
    {
        var projectText = File.ReadAllText("OutputType_Exe.txt");
        var document = XDocument.Parse(projectText);
        Assert.True(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void WebApplication_webSdk()
    {
        var projectText = File.ReadAllText("WebApplication_websdk.txt");
        var document = XDocument.Parse(projectText);
        Assert.True(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void GetStartupProjectsWithDefault()
    {
        var startupProjects = StartProjectFinder.GetStartProjects("SimpleSolutionWithDefault/SimpleSolution.sln");
        Assert.Equal("11111111-1111-1111-1111-111111111111", startupProjects.Single());
    }

    [Fact]
    public void Conditional_Exe_from_OutputType()
    {
        var projectText = File.ReadAllText("OutputType_Conditional_Exe.txt");
        var document = XDocument.Parse(projectText);
        Assert.True(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void WinExe_from_OutputType()
    {
        var projectText = File.ReadAllText("OutputType_WinExe.txt");
        var document = XDocument.Parse(projectText);
        Assert.True(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void StartActionIsProgram()
    {
        var projectText = File.ReadAllText("StartActionIsProgram.txt");
        var document = XDocument.Parse(projectText);
        Assert.True(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void Lib_from_OutputType()
    {
        var projectText = File.ReadAllText("OutputType_Lib.txt");
        var document = XDocument.Parse(projectText);
        Assert.False(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void WebApplication_xproj()
    {
        var projectText = File.ReadAllText("WebApplication_xproj.txt");
        var document = XDocument.Parse(projectText);
        Assert.False(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void Multiple_excluded_project_types()
    {
        var projectText = File.ReadAllText("Multiple_Exclude.txt");
        var document = XDocument.Parse(projectText);
        Assert.False(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void Multiple_include_project_types()
    {
        var projectText = File.ReadAllText("Multiple_Include.txt");
        var document = XDocument.Parse(projectText);
        Assert.True(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }

    [Fact]
    public void Lower_project_types()
    {
        var projectText = File.ReadAllText("Lower_Include.txt");
        var document = XDocument.Parse(projectText);
        Assert.True(StartProjectFinder.ShouldIncludeProjectXml(document, "/dir/project.csproj"));
    }
}