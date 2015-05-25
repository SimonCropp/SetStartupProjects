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
    public class SuoHacker
    {
        /// <summary>
        /// Create suo files that contain startup projects.
        /// </summary>
        /// <remarks>
        /// All existing suo files will be deleted.
        /// </remarks>
        public void CreateStartProjectSuoFiles(string solutionDirectory, List<string> startupProjectGuids, VisualStudioVersions visualStudioVersions = VisualStudioVersions.All)
        {
            Guard.AgainstNullAndEmpty(solutionDirectory, "solutionDirectory");
            Guard.AgainstNonExistingDirectory(solutionDirectory, "solutionDirectory");
            Guard.AgainstNullAndEmpty(startupProjectGuids, "startupProjectGuids");

            DeleteExistingSuo(solutionDirectory);

            var solutionPath = Directory.EnumerateFiles(solutionDirectory, "*.sln").Single();
            if ((visualStudioVersions & VisualStudioVersions.Vs2013) == VisualStudioVersions.Vs2013)
            {
                using (var stream = Resource.AsStream("Solution2013.suotemplate"))
                {
                    var suoFilePath = Path.ChangeExtension(solutionPath, ".v12.suo");
                    WriteToStream(suoFilePath, startupProjectGuids, stream);
                }
            }
            if ((visualStudioVersions & VisualStudioVersions.Vs2012) == VisualStudioVersions.Vs2012)
            {
                using (var stream = Resource.AsStream("Solution2012.suotemplate"))
                {
                    var suoFilePath = Path.ChangeExtension(solutionPath, ".v11.suo");
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

        static void DeleteExistingSuo(string solutionDirectory)
        {
            foreach (var suoFile in Directory.EnumerateFiles(solutionDirectory, "*.suo"))
            {
                File.Delete(suoFile);
            }
        }
    }
}