namespace SetStartupProjects;

public class Project
{
    public Project(string fullPath, string relativePath, string guid)
    {
        FullPath = fullPath;
        RelativePath = relativePath;
        Guid = guid;
    }

    public readonly string Guid;
    public readonly string RelativePath;
    public readonly string FullPath;
}