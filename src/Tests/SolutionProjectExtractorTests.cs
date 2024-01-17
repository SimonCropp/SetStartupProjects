using SetStartupProjects;

public class SolutionProjectExtractorTests
{
    [Fact]
    public Task GetAllProjectFiles()
    {
        var allProjectFiles = SolutionProjectExtractor.GetAllProjectFiles("SampleSolution.txt");
        return Verify(allProjectFiles);
    }
}