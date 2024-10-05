namespace SetStartupProjects;

/// <summary>
/// What versions of Visual Studio to target for .suo creation.
/// </summary>
[Flags]
public enum VisualStudioVersions
{
    /// <summary>
    /// Target suo creation for Visual Studio 2017.
    /// </summary>
    Vs2017 = 8,
    /// <summary>
    /// Target suo creation for Visual Studio 2019.
    /// </summary>
    Vs2019 = 16,
    /// <summary>
    /// Target suo creation for Visual Studio 2022.
    /// </summary>
    Vs2022 = 32,
    /// <summary>
    /// Target suo creation for Visual Studio versions 2012, 2013, 2015, 2017, 2019, and 2022.
    /// </summary>
    All = Vs2017 | Vs2019 | Vs2022,
}