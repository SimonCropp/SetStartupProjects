using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenMcdf;
using Resourcer;

namespace SetStartupProjects
{

    public class SuoHacker
    {
        public void CreateStartProjectSuoFiles(string solutionDirectory, List<string> startupProjectGuids)
        {
            DeleteExistingSuo(solutionDirectory);

            var solutonPath = Directory.EnumerateFiles(solutionDirectory, "*.sln").Single();

            using (var stream = Resource.AsStream("Solution2013.suotemplate"))
            {
                var suoFilePath = Path.ChangeExtension(solutonPath, ".v12.suo");
                WriteToStream(suoFilePath, startupProjectGuids, stream);
            }
            using (var stream = Resource.AsStream("Solution2012.suotemplate"))
            {
                var suoFilePath = Path.ChangeExtension(solutonPath, ".v11.suo");
                WriteToStream(suoFilePath, startupProjectGuids, stream);
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
            var NUL = '\u0000';
            var DC1 = '\u0011';
            var ETX = '\u0003';
            var SOH = '\u0001';

            var builder = new StringBuilder();
            builder.Append(DC1);
            builder.Append(NUL);
            builder.Append("MultiStartupProj");
            builder.Append(NUL);
            builder.Append('=');
            builder.Append(ETX);
            builder.Append(SOH);
            builder.Append(NUL);
            builder.Append(';');
            foreach (var startupProjectGuid in startupProjectGuids)
            {
                builder.Append('4');
                builder.Append(NUL);
                builder.AppendFormat("{{{0}}}.dwStartupOpt", startupProjectGuid);
                builder.Append(NUL);
                builder.Append('=');
                builder.Append(ETX);
                builder.Append(DC1);
                builder.Append(NUL);
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