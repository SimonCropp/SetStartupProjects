static class Resource
{
    static Assembly assembly = typeof(Resource).Assembly;

    public static Stream AsStream(string name) =>
        assembly.GetManifestResourceStream($"SetStartupProjects.{name}")!;
}