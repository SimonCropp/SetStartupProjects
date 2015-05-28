using System;
using System.Collections.Generic;
using System.IO;

namespace SetStartupProjects
{
    /// <summary>
    /// Helper that extracts all the project paths and guids from a solution. 
    /// </summary>
    public static class SolutionProjectExtractor
    {

        public static IEnumerable<Project> GetAllProjectFiles(string solutionFile)
        {
            var solutionDirectory = Path.GetDirectoryName(solutionFile);
            foreach (var line in File.ReadAllLines(solutionFile))
            {
                if (!line.StartsWith("Project("))
                {
                    continue;
                }
                var strings = line.Split(new[] { "\", \"" },StringSplitOptions.RemoveEmptyEntries);
                var guid = strings[2].Trim('{', '}', '"');
                var fullPath = Path.Combine(solutionDirectory, strings[1]);
                yield return new Project
                {
                    FullPath = fullPath,
                    RelativePath = strings[1],
                    Guid = guid
                };
            }
        }
    }

        public class Project
        {
            public string Guid;
            public string RelativePath;
            public string FullPath;
        }

}