static class Resource
{
    static Assembly assembly = typeof(Resource).Assembly;

    public static MemoryStream AsStream(string name)
    {
        using var stream = assembly.GetManifestResourceStream($"SetStartupProjects.{name}")!;
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream;
    }
}