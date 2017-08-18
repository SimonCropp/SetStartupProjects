using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SetStartupProjects
{
    /// <summary>
    ///  Helper class for extracting Visual Studio startup projects by looking at the projects contained in the solution directory and allplying some conventions.
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
        /// <summary>
        /// Get the startup projects by looking at the projects contained in <paramref name="solutionFile"/>.
        /// </summary>
        public IEnumerable<string> GetStartProjects(string solutionFile)
        {
            Guard.AgainstNullAndEmpty(solutionFile, nameof(solutionFile));
            Guard.AgainstNonExistingFile(solutionFile, nameof(solutionFile));

            return from project in SolutionProjectExtractor.GetAllProjectFiles(solutionFile)
                where ShouldIncludeProjectFile(project)
                select project.Guid;
        }

        protected internal bool ShouldIncludeProjectFile(Project project)
        {
            var projectFile = project.FullPath;
            Guard.AgainstNonExistingFile(projectFile, "Project");
            try
            {

                if (ShouldIncludeForFileExtension(Path.GetExtension(projectFile)))
                {
                    return true;
                }
                using (var reader = File.OpenText(projectFile))
                {
                    var xDocument = XDocument.Load(reader);
                    return ShouldIncludeProjectXml(xDocument, projectFile);
                }
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
            var xElement = xDocument.Root;
            var propertyGroup = xElement
                .Element("PropertyGroup");

            if (ShouldIncludeForStartAction(xDocument))
            {
                return true;
            }
            if (ShouldIncludeForOutputType(propertyGroup))
            {
                return true;
            }
            if (ShouldIncludeFromProjectTypeGuids(propertyGroup))
            {
                return true;
            }
            return false;
        }

        protected internal bool ShouldIncludeForStartAction(XDocument xDocument)
        {
            return xDocument.Descendants("StartAction")
                .Any(x => x.Value == "Program");
        }

        protected internal bool ShouldIncludeForOutputType(XElement propertyGroup)
        {
            var xElement = propertyGroup.Element("OutputType");
            // OutputType can be null for xprojs
            if (xElement != null)
            {
                var outputType = xElement.Value;
                if (
                    outputType == "Exe" ||
                    outputType == "WinExe"
                )
                {
                    return true;
                }
            }
            return false;
        }

        protected internal bool ShouldIncludeFromProjectTypeGuids(XElement propertyGroup)
        {
            var projectTypes = propertyGroup.Element("ProjectTypeGuids");
            if (projectTypes != null)
            {
                return projectTypes.Value.Split(';')
                    .Select(x => x.Trim('{', '}'))
                    .Any(typeGuid => DefaultIncludedGuids.Any(x => x == typeGuid));
            }
            return false;
        }

        protected internal List<string> DefaultIncludedGuids = new List<string>
        {
            "603C0E0B-DB56-11DC-BE95-000D561079B0", //ASP.NET MVC 1.0
            "F85E285D-A4E0-4152-9332-AB1D724D3325", //ASP.NET MVC 2.0
            "E53F8FEA-EAE0-44A6-8774-FFD645390401", //ASP.NET MVC 3.0
            "E3E379DF-F4C6-4180-9B81-6769533ABE47", //ASP.NET MVC 4.0
            "349C5851-65DF-11DA-9384-00065B846F21", //Web Application
            "E24C65DC-7377-472B-9ABA-BC803B73C61A", //Web Site
        };
    }
}