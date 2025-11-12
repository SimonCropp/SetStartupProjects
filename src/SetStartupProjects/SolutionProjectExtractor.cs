namespace SetStartupProjects;

/// <summary>
/// Helper that extracts all the project paths and guids from a solution.
/// </summary>
public static class SolutionProjectExtractor
{
    public static IEnumerable<Project> GetAllProjectFiles(string solutionFile)
    {
        Ensure.FileExists(solutionFile, nameof(solutionFile));
        var solutionDirectory = Path.GetDirectoryName(solutionFile)!;
        foreach (var line in File.ReadAllLines(solutionFile))
        {
            if (!line.StartsWith("Project("))
            {
                continue;
            }
            var strings = line.Split(["\", \""], StringSplitOptions.RemoveEmptyEntries);
            var guidType = strings[0].Split('{', '}')[1];
            var guid = strings[2].Trim('{', '}', '"');
            var relativePath = Path.DirectorySeparatorChar == '\\' ? strings[1] : strings[1].Replace('\\', Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(solutionDirectory, relativePath));
            if (guidType == "2150E333-8FDC-42A3-9474-1A3956D46DE8")
            {
                //this is a Solution Folder and can be ignored
                continue;
            }
            yield return new(
                fullPath: fullPath,
                relativePath: relativePath,
                guid: guid
            );
        }
    }
}