using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenMcdf;
using Resourcer;

namespace SetStartupProjects
{
    /// <summary>
    /// Creates suo files that contain startup projects.
    /// </summary>
    public class StartProjectSuoCreator
    {
        /// <summary>
        /// Create suo startup files for a specific solution file.
        /// </summary>
        /// <remarks>
        /// All existing suo files will be overwritten.
        /// </remarks>
        public void CreateForSolutionFile(string solutionFilePath, List<string> startupProjectGuids, VisualStudioVersions visualStudioVersions = VisualStudioVersions.All)
        {
            Guard.AgainstNullAndEmpty(solutionFilePath, "solutionFilePath");
            Guard.AgainstNonExistingFile(solutionFilePath, "solutionFilePath");
            Guard.AgainstNullAndEmpty(startupProjectGuids, "startupProjectGuids");
            var solutionDirectory = Path.GetDirectoryName(solutionFilePath);
            if ((visualStudioVersions & VisualStudioVersions.Vs2015) == VisualStudioVersions.Vs2015)
            {
                var solutionName = Path.GetFileNameWithoutExtension(solutionFilePath);
                var suoDirectoryPath = Path.Combine(solutionDirectory, ".vs", solutionName, "v14");
                Directory.CreateDirectory(suoDirectoryPath);
                var suoFilePath = Path.Combine(suoDirectoryPath, ".suo");
                File.Delete(suoFilePath);
                using (var stream = Resource.AsStream("Solution2015.suotemplate"))
                {
                    WriteToStream(suoFilePath, startupProjectGuids, stream);
                }
            }
            if ((visualStudioVersions & VisualStudioVersions.Vs2013) == VisualStudioVersions.Vs2013)
            {
                var suoFilePath = Path.ChangeExtension(solutionFilePath, ".v12.suo");
                File.Delete(suoFilePath);
                using (var stream = Resource.AsStream("Solution2013.suotemplate"))
                {
                    WriteToStream(suoFilePath, startupProjectGuids, stream);
                }
            }
            if ((visualStudioVersions & VisualStudioVersions.Vs2012) == VisualStudioVersions.Vs2012)
            {
                var suoFilePath = Path.ChangeExtension(solutionFilePath, ".v11.suo");
                File.Delete(suoFilePath);
                using (var stream = Resource.AsStream("Solution2012.suotemplate"))
                {
                    WriteToStream(suoFilePath, startupProjectGuids, stream);
                }
            }
        }

        static void WriteToStream(string suoFilePath, List<string> startupProjectGuids, Stream stream)
        {
            try
            {
                using (var compoundFile = new CompoundFile(stream, UpdateMode.ReadOnly, true, true, false))
                {
                    compoundFile.RootStorage.Delete("SolutionConfiguration");
                    var solutionConfiguration = compoundFile.RootStorage.AddStream("SolutionConfiguration");

                    SetSolutionConfigValue(solutionConfiguration, startupProjectGuids);
                    compoundFile.Save(suoFilePath);
                }
            }
            catch (Exception exception)
            {
                var joinedGuids = string.Join(" ", startupProjectGuids);
                var message = string.Format("Could not create .suo file for '{0}'. Guids: {1}", suoFilePath, joinedGuids);
                throw new Exception(message, exception);
            }
        }

        static void SetSolutionConfigValue(CFStream cfStream, IEnumerable<string> startupProjectGuids)
        {
            var single = Encoding.GetEncodings().Single(x => x.Name == "utf-16");
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
}