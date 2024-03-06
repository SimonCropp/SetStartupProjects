namespace SetStartupProjects;

/// <summary>
/// Creates suo files that contain startup projects.
/// </summary>
public static class StartProjectSuoCreator
{
    /// <summary>
    /// Create suo startup files for a specific solution file.
    /// </summary>
    /// <remarks>
    /// All existing suo files will be overwritten.
    /// </remarks>
    public static void CreateForSolutionFile(string solutionFilePath, List<string> startupProjectGuids, VisualStudioVersions visualStudioVersions = VisualStudioVersions.All)
    {
        Guard.AgainstNullAndEmpty(solutionFilePath, nameof(solutionFilePath));
        Guard.AgainstNonExistingFile(solutionFilePath, nameof(solutionFilePath));
        if (startupProjectGuids.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startupProjectGuids), $"For solutionFilePath: '{solutionFilePath}'");
        }

        var solutionDirectory = Path.GetDirectoryName(solutionFilePath)!;

        if (visualStudioVersions.HasFlag(VisualStudioVersions.Vs2022))
        {
            var solutionName = Path.GetFileNameWithoutExtension(solutionFilePath);
            var suoDirectoryPath = Path.Combine(solutionDirectory, ".vs", solutionName, "v17");
            Directory.CreateDirectory(suoDirectoryPath);
            var suoFilePath = Path.Combine(suoDirectoryPath, ".suo");
            File.Delete(suoFilePath);
            using var templateStream = Resource.AsStream("Solution2022.suotemplate");
            WriteToStream(suoFilePath, startupProjectGuids, templateStream);
        }

        if (visualStudioVersions.HasFlag(VisualStudioVersions.Vs2019))
        {
            var solutionName = Path.GetFileNameWithoutExtension(solutionFilePath);
            var suoDirectoryPath = Path.Combine(solutionDirectory, ".vs", solutionName, "v16");
            Directory.CreateDirectory(suoDirectoryPath);
            var suoFilePath = Path.Combine(suoDirectoryPath, ".suo");
            File.Delete(suoFilePath);
            using var templateStream = Resource.AsStream("Solution2019.suotemplate");
            WriteToStream(suoFilePath, startupProjectGuids, templateStream);
        }

        if (visualStudioVersions.HasFlag(VisualStudioVersions.Vs2017))
        {
            var solutionName = Path.GetFileNameWithoutExtension(solutionFilePath);
            var suoDirectoryPath = Path.Combine(solutionDirectory, ".vs", solutionName, "v15");
            Directory.CreateDirectory(suoDirectoryPath);
            var suoFilePath = Path.Combine(suoDirectoryPath, ".suo");
            File.Delete(suoFilePath);
            using var templateStream = Resource.AsStream("Solution2017.suotemplate");
            WriteToStream(suoFilePath, startupProjectGuids, templateStream);
        }

        if (visualStudioVersions.HasFlag(VisualStudioVersions.Vs2015))
        {
            var solutionName = Path.GetFileNameWithoutExtension(solutionFilePath);
            var suoDirectoryPath = Path.Combine(solutionDirectory, ".vs", solutionName, "v14");
            Directory.CreateDirectory(suoDirectoryPath);
            var suoFilePath = Path.Combine(suoDirectoryPath, ".suo");
            File.Delete(suoFilePath);
            using var templateStream = Resource.AsStream("Solution2015.suotemplate");
            WriteToStream(suoFilePath, startupProjectGuids, templateStream);
        }

        if (visualStudioVersions.HasFlag(VisualStudioVersions.Vs2013))
        {
            var suoFilePath = Path.ChangeExtension(solutionFilePath, ".v12.suo");
            File.Delete(suoFilePath);
            using var templateStream = Resource.AsStream("Solution2013.suotemplate");
            WriteToStream(suoFilePath, startupProjectGuids, templateStream);
        }

        if (visualStudioVersions.HasFlag(VisualStudioVersions.Vs2012))
        {
            var suoFilePath = Path.ChangeExtension(solutionFilePath, ".v11.suo");
            File.Delete(suoFilePath);
            using var templateStream = Resource.AsStream("Solution2012.suotemplate");
            WriteToStream(suoFilePath, startupProjectGuids, templateStream);
        }
    }

    static void WriteToStream(string suoFilePath, List<string> startupProjectGuids, Stream templateStream)
    {
        try
        {
            using var compoundFile = new CompoundFile(templateStream, CFSUpdateMode.ReadOnly, CFSConfiguration.SectorRecycle | CFSConfiguration.EraseFreeSectors);
            compoundFile.RootStorage.Delete("SolutionConfiguration");
            var solutionConfiguration = compoundFile.RootStorage.AddStream("SolutionConfiguration");

            SetSolutionConfigValue(solutionConfiguration, startupProjectGuids);
            compoundFile.SaveAs(suoFilePath);
        }
        catch (Exception exception)
        {
            var joinedGuids = string.Join(" ", startupProjectGuids);
            var message = $"Could not create .suo file for '{suoFilePath}'. Guids: {joinedGuids}";
            throw new(message, exception);
        }
    }

    static void SetSolutionConfigValue(CFStream cfStream, IEnumerable<string> startupProjectGuids)
    {
        var single = Encoding.GetEncodings()
            .Single(_ => string.Equals(_.Name, "utf-16", StringComparison.OrdinalIgnoreCase));
        var encoding = single.GetEncoding();
        var nul = '\u0000';
        var dc1 = '\u0011';
        var etx = '\u0003';
        var soh = '\u0001';

        var builder = new StringBuilder();
        builder.Append(dc1);
        builder.Append(nul);
        builder.Append("MultiStartupProj");
        builder.Append(nul);
        builder.Append('=');
        builder.Append(etx);
        builder.Append(soh);
        builder.Append(nul);
        builder.Append(';');
        foreach (var startupProjectGuid in startupProjectGuids)
        {
            builder.Append('4');
            builder.Append(nul);
            builder.AppendFormat("{{{0}}}.dwStartupOpt", startupProjectGuid);
            builder.Append(nul);
            builder.Append('=');
            builder.Append(etx);
            builder.Append(dc1);
            builder.Append(nul);
            builder.Append(';');
        }

        var newBytes = encoding.GetBytes(builder.ToString());
        cfStream.SetData(newBytes);
    }
}