using System;

namespace SetStartupProjects
{
    /// <summary>
    /// What versions of Visual Studio to target for .suo creation.
    /// </summary>
    [Flags]
    public enum VisualStudioVersions
    {
        /// <summary>
        /// Target suo creation for Visual Studio 2012.
        /// </summary>
        Vs2012 = 1,
        /// <summary>
        /// Target suo creation for Visual Studio 2013.
        /// </summary>
        Vs2013 = 2,
        /// <summary>
        /// Target suo creation for Visual Studio 2015.
        /// </summary>
        Vs2015 = 4,
        /// <summary>
        /// Target suo creation for Visual Studio 2017.
        /// </summary>
        Vs2017 = 8,
        /// <summary>
        /// Target suo creation for Visual Studio versions 2012, 2013, 2015 and 2017.
        /// </summary>
        All = Vs2012 | Vs2013 | Vs2015 | Vs2017,
    }
}