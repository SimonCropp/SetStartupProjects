class Samples
{
    static void GuessStartupProjects(string solutionFilePath)
    {
        var startupProjectGuids = StartProjectFinder.GetStartProjects(solutionFilePath)
            .ToList();
        StartProjectSuoCreator.CreateForSolutionFile(solutionFilePath, startupProjectGuids);
    }

    static void PassGuids(string solutionFilePath)
    {
        var startupProjectGuids = new List<string>
        {
            "11111111-1111-1111-1111-111111111111",
            "22222222-2222-2222-2222-222222222222"
        };
        StartProjectSuoCreator.CreateForSolutionFile(solutionFilePath, startupProjectGuids);
    }
}