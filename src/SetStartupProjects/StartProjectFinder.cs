using System.Xml.Linq;

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
public class StartProjectFinder
{
    IEnumerable<string> GuessStartupProjects(string solutionFile)
    {
        return from project in SolutionProjectExtractor.GetAllProjectFiles(solutionFile)
            where ShouldIncludeProjectFile(project)
            select project.Guid;
    }

    /// <summary>
    /// Get the startup projects by looking at the projects contained in <paramref name="solutionFile"/>.
    /// </summary>
    public IEnumerable<string> GetStartProjects(string solutionFile)
    {
        Guard.AgainstNullAndEmpty(solutionFile, nameof(solutionFile));
        Guard.AgainstNonExistingFile(solutionFile, nameof(solutionFile));

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(solutionFile);
        var solutionDirectory = Path.GetDirectoryName(solutionFile);
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
            .Where(x => !string.IsNullOrWhiteSpace(x));
        foreach (var startupProject in defaultProjects)
        {
            var project = allPossibleProjects.FirstOrDefault(x => string.Equals(x.RelativePath, startupProject, StringComparison.OrdinalIgnoreCase));
            if (project == null)
            {
                var error = $"Could not find the relative path to the default startup project '{startupProject}'. Ensure `{defaultProjectsTextFile}` contains relative (to the solution directory) paths to project files.";
                throw new Exception(error);
            }
            yield return project.Guid;
        }
    }

    bool ShouldIncludeProjectFile(Project project)
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
            var xDocument = XDocument.Load(reader);
            return ShouldIncludeProjectXml(xDocument, projectFile);
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed {nameof(ShouldIncludeProjectFile)}: {projectFile}", exception);
        }
    }

    bool ShouldIncludeForFileExtension(string extension)
    {
        return extension == ".ccproj" ||
               extension == ".sfproj";
    }

    protected internal bool ShouldIncludeProjectXml(XDocument xDocument, string projectFile)
    {
        var directoryName = Path.GetDirectoryName(projectFile);
        var netCoreLaunchSettingsFile = Path.Combine(directoryName, "Properties", "launchSettings.json");
        if (File.Exists(netCoreLaunchSettingsFile))
        {
            return true;
        }

        xDocument.StripNamespace();
        if (ShouldIncludeForStartAction(xDocument))
        {
            return true;
        }
        if (ShouldIncludeForWebSdk(xDocument))
        {
            return true;
        }
        var xElement = xDocument.Root!;
        var propertyGroups = xElement
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

    bool ShouldIncludeForStartAction(XDocument document)
    {
        return document.Descendants("StartAction")
            .Any(x => x.Value == "Program");
    }

    bool ShouldIncludeForWebSdk(XDocument document)
    {
        var attribute = document.Root!.Attribute("Sdk");
        if (attribute == null)
        {
            return false;
        }
        return attribute.Value== "Microsoft.NET.Sdk.Web";
    }

    bool ShouldIncludeForOutputType(XElement propertyGroup)
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

    bool ShouldIncludeFromProjectTypeGuids(XElement propertyGroup)
    {
        var projectTypes = propertyGroup.Element("ProjectTypeGuids");
        if (projectTypes != null)
        {
            return projectTypes.Value.Split(';')
                .Select(x => x.Trim('{', '}'))
                .Any(typeGuid => DefaultIncludedGuids.Any(x => string.Equals(x, typeGuid, StringComparison.OrdinalIgnoreCase)));
        }
        return false;
    }

    static List<string> DefaultIncludedGuids = new()
    {
        "603C0E0B-DB56-11DC-BE95-000D561079B0", //ASP.NET MVC 1.0
        "F85E285D-A4E0-4152-9332-AB1D724D3325", //ASP.NET MVC 2.0
        "E53F8FEA-EAE0-44A6-8774-FFD645390401", //ASP.NET MVC 3.0
        "E3E379DF-F4C6-4180-9B81-6769533ABE47", //ASP.NET MVC 4.0
        "349C5851-65DF-11DA-9384-00065B846F21", //Web Application
        "E24C65DC-7377-472B-9ABA-BC803B73C61A", //Web Site
    };
}