using System.IO;
using System.Reflection;

static class Resource
{
    static Assembly assembly = typeof(Resource).Assembly;

    public static Stream AsStream(string name)
    {
        return assembly.GetManifestResourceStream($"SetStartupProjects.{name}")!;
    }
}