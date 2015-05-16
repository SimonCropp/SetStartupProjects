using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SetStartupProjects
{
    public class StartProjectFinder
    {
        public IEnumerable<string> GetStartProjects(string solutionDirectory)
        {
            foreach (var projectFile in Directory.EnumerateFiles(solutionDirectory, "*.csproj", SearchOption.AllDirectories))
            {
                using (var reader = File.OpenText(projectFile))
                {
                    string guid;
                    if (ShouldInclude(reader, out guid))
                    {
                        yield return guid;
                    }
                }
            }
        }

        internal bool ShouldInclude(StreamReader reader, out string guid)
        {
            var xDocument = XDocument.Load(reader);
            xDocument.StripNamespace();
            var xElement = xDocument.Root;
            var propertyGroup = xElement
                .Element("PropertyGroup");
            guid = propertyGroup
                .Element("ProjectGuid")
                .Value
                .Trim('{', '}');

            if (ShouldIncludeForNsbHost(xDocument))
            {
                return true;
            }
            if (ShouldIncludeForExe(propertyGroup))
            {
                return true;
            }
            if (ShouldIncludeFromProjectTypeGuids(propertyGroup))
            {
                return true;
            }
            guid = null;
            return false;
        }

        static bool ShouldIncludeForNsbHost(XDocument xDocument)
        {
            return xDocument.Descendants("StartProgram")
                .Any(x => x.Value.Contains("NServiceBus.Host.exe"));
        }

        static bool ShouldIncludeForExe(XElement propertyGroup)
        {
            if (propertyGroup.Element("OutputType").Value == "Exe")
            {
                return true;
            }
            return false;
        }

        static bool ShouldIncludeFromProjectTypeGuids(XElement propertyGroup)
        {
            var projectTypes = propertyGroup.Element("ProjectTypeGuids");
            if (projectTypes != null)
            {
                return projectTypes.Value.Split(';')
                    .Select(x => x.Trim('{', '}'))
                    .Any(typeGuid => IncludeGuids.Any(x => x == typeGuid));
            }
            return false;
        }

        static List<string> IncludeGuids = new List<string>
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