namespace SetStartupProjects;

/// <summary>
///  Helper class for extracting Visual Studio startup projects by looking at the projects contained in the solution directory and applying some conventions.
///
/// The current conventions are as follows:
///   <list type="bullet">
///     <item>
///       <description>'StartAction' is 'Program'.</description>
///     </item>
///     <item>
///       <description>'OutputType' is 'Exe'.</description>
///     </item>
///     <item>
///       <description>'OutputType' is 'WinExe'.</description>
///     </item>
///     <item>
///       <description>Project extension is `ccproj` ie an Azure Cloud Service.</description>
///     </item>
///     <item>
///       <description>Project type is 'ASP.NET MVC 1.0' ie '603C0E0B-DB56-11DC-BE95-000D561079B0'.</description>
///     </item>
///     <item>
///       <description>Project type is 'ASP.NET MVC 2.0' ie 'F85E285D-A4E0-4152-9332-AB1D724D3325'.</description>
///     </item>
///     <item>
///       <description>Project type is 'ASP.NET MVC 3.0' ie 'E53F8FEA-EAE0-44A6-8774-FFD645390401'.</description>
///     </item>
///     <item>
///       <description>Project type is 'ASP.NET MVC 4.0' ie 'E3E379DF-F4C6-4180-9B81-6769533ABE47'.</description>
///     </item>
///     <item>
///       <description>Project type is 'Web Application' ie '349C5851-65DF-11DA-9384-00065B846F21'.</description>
///     </item>
///     <item>
///       <description>Project type is 'Web Site' ie 'E24C65DC-7377-472B-9ABA-BC803B73C61A'.</description>
///     </item>
///   </list>
/// </summary>
public static class StartProjectFinder
{
    static IEnumerable<string> GuessStartupProjects(string solutionFile) =>
        from project in SolutionProjectExtractor.GetAllProjectFiles(solutionFile)
        where ShouldIncludeProjectFile(project)
        select project.Guid;

    /// <summary>
    /// Get the startup projects by looking at the projects contained in <paramref name="solutionFile"/>.
    /// </summary>
    public static IEnumerable<string> GetStartProjects(string solutionFile)
    {
        Guard.AgainstNullAndEmpty(solutionFile, nameof(solutionFile));
        Guard.AgainstNonExistingFile(solutionFile, nameof(solutionFile));

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(solutionFile);
        var solutionDirectory = Path.GetDirectoryName(solutionFile)!;
        var defaultProjectsTextFile = Path.Combine(solutionDirectory, $"{nameWithoutExtension}.StartupProjects.txt");
        if (!File.Exists(defaultProjectsTextFile))
        {
            foreach (var startProject in GuessStartupProjects(solutionFile))
            {
                yield return startProject;
            }

            yield break;
        }

        var allPossibleProjects = SolutionProjectExtractor.GetAllProjectFiles(solutionFile).ToList();
        var defaultProjects = File.ReadAllLines(defaultProjectsTextFile)
            .Where(_ => !string.IsNullOrWhiteSpace(_));
        foreach (var startupProject in defaultProjects)
        {
            var project = allPossibleProjects.FirstOrDefault(_ => string.Equals(_.RelativePath, startupProject, StringComparison.OrdinalIgnoreCase));
            if (project == null)
            {
                var error = $"Could not find the relative path to the default startup project '{startupProject}'. Ensure `{defaultProjectsTextFile}` contains relative (to the solution directory) paths to project files.";
                throw new(error);
            }

            yield return project.Guid;
        }
    }

    static bool ShouldIncludeProjectFile(Project project)
    {
        var projectFile = project.FullPath;
        Guard.AgainstNonExistingFile(projectFile, "Project");
        try
        {
            if (ShouldIncludeForFileExtension(Path.GetExtension(projectFile)))
            {
                return true;
            }

            using var reader = File.OpenText(projectFile);
            var document = XDocument.Load(reader);
            return ShouldIncludeProjectXml(document, projectFile);
        }
        catch (Exception exception)
        {
            throw new($"Failed {nameof(ShouldIncludeProjectFile)}: {projectFile}", exception);
        }
    }

    static bool ShouldIncludeForFileExtension(string extension) =>
        extension is ".ccproj" or ".sfproj";

    internal static bool ShouldIncludeProjectXml(XDocument document, string projectFile)
    {
        var directoryName = Path.GetDirectoryName(projectFile)!;
        var netCoreLaunchSettingsFile = Path.Combine(directoryName, "Properties", "launchSettings.json");
        if (File.Exists(netCoreLaunchSettingsFile))
        {
            return true;
        }

        document.StripNamespace();
        if (ShouldIncludeForStartAction(document))
        {
            return true;
        }

        if (ShouldIncludeForWebSdk(document))
        {
            return true;
        }

        var element = document.Root!;
        var propertyGroups = element
            .Elements("PropertyGroup");
        foreach (var propertyGroup in propertyGroups)
        {
            if (ShouldIncludeForOutputType(propertyGroup))
            {
                return true;
            }

            if (ShouldIncludeFromProjectTypeGuids(propertyGroup))
            {
                return true;
            }
        }

        return false;
    }

    static bool ShouldIncludeForStartAction(XDocument document) =>
        document.Descendants("StartAction")
            .Any(_ => _.Value == "Program");

    static bool ShouldIncludeForWebSdk(XDocument document)
    {
        var attribute = document.Root!.Attribute("Sdk");
        if (attribute == null)
        {
            return false;
        }

        return attribute.Value == "Microsoft.NET.Sdk.Web";
    }

    static bool ShouldIncludeForOutputType(XElement propertyGroup)
    {
        var xElement = propertyGroup.Element("OutputType");
        // OutputType can be null for xprojs
        if (xElement != null)
        {
            var outputType = xElement.Value;
            if (
                string.Equals(outputType, "Exe", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(outputType, "WinExe", StringComparison.OrdinalIgnoreCase)
            )
            {
                return true;
            }
        }

        return false;
    }

    static bool ShouldIncludeFromProjectTypeGuids(XElement propertyGroup)
    {
        var projectTypes = propertyGroup.Element("ProjectTypeGuids");
        if (projectTypes != null)
        {
            return projectTypes.Value.Split(';')
                .Select(_ => _.Trim('{', '}'))
                .Any(typeGuid => defaultIncludedGuids.Any(_ => string.Equals(_, typeGuid, StringComparison.OrdinalIgnoreCase)));
        }

        return false;
    }

    static List<string> defaultIncludedGuids =
    [
        "603C0E0B-DB56-11DC-BE95-000D561079B0", //ASP.NET MVC 1.0
        "F85E285D-A4E0-4152-9332-AB1D724D3325", //ASP.NET MVC 2.0
        "E53F8FEA-EAE0-44A6-8774-FFD645390401", //ASP.NET MVC 3.0
        "E3E379DF-F4C6-4180-9B81-6769533ABE47", //ASP.NET MVC 4.0
        "349C5851-65DF-11DA-9384-00065B846F21", //Web Application
        "E24C65DC-7377-472B-9ABA-BC803B73C61A" //Web Site
    ];
}