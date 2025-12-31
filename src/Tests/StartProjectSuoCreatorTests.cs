public class StartProjectSuoCreatorTests
{
    /// <summary>
    /// This test verifies that suo files are properly written to disk and can be
    /// immediately zipped without corruption.
    ///
    /// Related to: https://github.com/SimonCropp/SetStartupProjects/pull/634
    ///
    /// The issue is that when CreateForSolutionFile returns, the file may not be
    /// fully flushed to disk, causing truncated/corrupted files when immediately zipped.
    /// </summary>
    [Fact]
    public void SuoFile_ShouldNotBeTruncated_WhenZippedImmediately()
    {
        using var tempDirectory = new TempDirectory();
        var solutionFolder = Path.Combine(tempDirectory, "Solution");
        var imaginarySolutionFile = Path.Combine(solutionFolder, "MyFakeSolution.sln");
        var zipFilePath = Path.Combine(tempDirectory, "test.zip");

        List<string> startProjectGuids =
        [
            "D4DCF868-A625-4B0B-BB20-C150553A6548",
            "CD42E5DF-D4A7-4933-8017-1398B2E9560F",
            "9BF02A43-6D9D-4B49-A06D-603A66C6BCB5",
            "41F0D809-8FA4-4139-9131-09441D69AFB1",
            "6DE93070-C47B-4FA6-86FF-421115654E6C"
        ];

        Directory.CreateDirectory(solutionFolder);
        File.WriteAllText(
            imaginarySolutionFile,
            "This file needs to exist on disk, but is never read and does not need to be a real solution");

        StartProjectSuoCreator.CreateForSolutionFile(
            imaginarySolutionFile,
            startProjectGuids,
            VisualStudioVersions.Vs2022);

        // Immediately zip the directory - this is the real-world scenario where the bug manifests
        ZipFile.CreateFromDirectory(
            solutionFolder,
            zipFilePath,
            CompressionLevel.NoCompression,
            false);

        // Extract and verify the suo file from the zip
        using var zipArchive = ZipFile.OpenRead(zipFilePath);
        var suoEntry = zipArchive.GetEntry(".vs/MyFakeSolution/v17/.suo");
        Assert.NotNull(suoEntry);

        using var suoStream = suoEntry.Open();
        using var memoryStream = new MemoryStream();
        suoStream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        // Verify the file can be opened as a valid CFB and contains the startup configuration
        using var cfbStorage = RootStorage.Open(memoryStream);
        var solutionConfigStream = cfbStorage.OpenStream("SolutionConfiguration");

        using var configMemoryStream = new MemoryStream();
        solutionConfigStream.CopyTo(configMemoryStream);
        var configBytes = configMemoryStream.ToArray();
        var configContent = Encoding.Unicode.GetString(configBytes);

        Assert.True(
            configBytes.Length > 0,
            "SolutionConfiguration stream is empty - file is corrupted/truncated");

        // Verify all project guids are present in the configuration
        foreach (var guid in startProjectGuids)
        {
            var expectedMarker = $"{{{guid}}}.dwStartupOpt";
            Assert.True(
                configContent.Contains(expectedMarker, StringComparison.OrdinalIgnoreCase),
                $"Expected SolutionConfiguration to contain '{expectedMarker}' but it was not found.");
        }
    }
}
