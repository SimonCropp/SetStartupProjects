namespace SetStartupProjects;

public class Project(string fullPath, string relativePath, string guid)
{
    public readonly string Guid = guid;
    public readonly string RelativePath = relativePath;
    public readonly string FullPath = fullPath;
}