using SetStartupProjects;

class Samples
{
    void GuessStartupProjects(string solutionFilePath)
    {
        var startupProjectGuids = new StartProjectFinder()
            .GetStartProjects(solutionFilePath)
            .ToList();
        var startProjectSuoCreator = new StartProjectSuoCreator();
        startProjectSuoCreator.CreateForSolutionFile(solutionFilePath, startupProjectGuids);
    }

    void PassGuids(string solutionFilePath)
    {
        List<string> startupProjectGuids = new()
        {
            "11111111-1111-1111-1111-111111111111",
            "22222222-2222-2222-2222-222222222222"
        };
        StartProjectSuoCreator startProjectSuoCreator = new();
        startProjectSuoCreator.CreateForSolutionFile(solutionFilePath, startupProjectGuids);
    }
}