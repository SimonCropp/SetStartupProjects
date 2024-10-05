public class SolutionProjectExtractorTests
{
    [Fact]
    public Task GetAllProjectFiles()
    {
        var files = SolutionProjectExtractor.GetAllProjectFiles("SampleSolution.txt");
        return Verify(
            files.Select(_ =>
                new
                {
                    _.Guid,
                    _.RelativePath,
                    FullPath = new FileInfo(_.FullPath),
                }));
    }
}