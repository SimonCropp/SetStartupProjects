﻿namespace SetStartupProjects;

/// <summary>
/// Helper that extracts all the project paths and guids from a solution.
/// </summary>
public static class SolutionProjectExtractor
{
    public static IEnumerable<Project> GetAllProjectFiles(string solutionFile)
    {
        Guard.AgainstNullAndEmpty(solutionFile, nameof(solutionFile));
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
            var fullPath = Path.GetFullPath(Path.Combine(solutionDirectory, strings[1]));
            if (guidType == "2150E333-8FDC-42A3-9474-1A3956D46DE8")
            {
                //this is a Solution Folder and can be ignored
                continue;
            }
            yield return new(
                fullPath: fullPath,
                relativePath: strings[1],
                guid: guid
            );
        }
    }
}