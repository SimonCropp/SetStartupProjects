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
            Guard.AgainstNullAndEmpty(solutionFile, "solutionFile");
            Guard.AgainstNonExistingFile(solutionFile, "solutionFile");

            return from project in GetAllProjectFiles(solutionFile) 
                   where ShouldIncludeProjectFile(project) 
                   select project.Guid;
        }

        protected internal bool ShouldIncludeProjectFile(Project project)
        {
            if (ShouldIncludeForFileExtension(Path.GetExtension(project.Path)))
            {
                return true;
            }
            using (var reader = File.OpenText(project.Path))
            {
                var xDocument = XDocument.Load(reader);
                return ShouldIncludeProjectXml(xDocument, project.Path);
            }
        }

        bool ShouldIncludeForFileExtension(string extension)
        {
            return extension == ".ccproj";
        }

        protected internal IEnumerable<Project> GetAllProjectFiles(string solutionFile)
        {
            var solutionDirectory = Path.GetDirectoryName(solutionFile);
            foreach (var line in File.ReadAllLines(solutionFile))
            {
                if (!line.StartsWith("Project("))
                {
                    continue;
                }
                var strings = line.Split(new[] { "\", \"" },StringSplitOptions.RemoveEmptyEntries);
                var projectPath = Path.Combine(solutionDirectory,strings[1]);
                var guid = strings[2].Trim('{', '}','"');
                yield return new Project
                {
                    Path = projectPath,
                    Guid = guid
                };
            }
        }

        public class Project
        {
            public string Guid;
            public string Path;
        }

        protected internal bool ShouldIncludeProjectXml(XDocument xDocument, string projectFile)
        {
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
            var outputType = propertyGroup.Element("OutputType").Value;
            if (
                outputType == "Exe" ||
                outputType == "WinExe" 
                )
            {
                return true;
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