public class SolutionProjectExtractorTests
{
    [Fact]
    public Task GetAllProjectFiles()
    {
        var files = SolutionProjectExtractor.GetAllProjectFiles("SampleSolution.txt");
        var toVerify = files.Select(_ =>
            new
            {
                _.Guid,
                _.RelativePath,
                FullPath = new FileInfo(_.FullPath),
            });

        var settings = new VerifySettings();
        settings.UniqueForOSPlatform();

        return Verify(toVerify, settings);
    }
}