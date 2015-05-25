using NUnit.Framework;
using Resourcer;
using SetStartupProjects;

[TestFixture]
public class StartProjectFinderTests
{
    [Test]
    public void Exe_from_OutputType()
    {
        using (var reader = Resource.AsStreamReader("OutputType_Exe.txt"))
        {
            string guid;
            Assert.IsTrue(new StartProjectFinder().ShouldInclude(reader, out guid));
            Assert.AreEqual("D04CF1FC-C4C0-4959-A817-2BC68770CA9B", guid);
        }
    }
    [Test]
    public void StartActionIsProgram()
    {
        using (var reader = Resource.AsStreamReader("StartActionIsProgram.txt"))
        {
            string guid;
            Assert.IsTrue(new StartProjectFinder().ShouldInclude(reader, out guid));
            Assert.AreEqual("D04CF1FC-C4C0-4959-A817-2BC68770CA9B", guid);
        }
    }

    [Test]
    public void Lib_from_OutputType()
    {
        using (var reader = Resource.AsStreamReader("OutputType_Lib.txt"))
        {
            string guid;
            Assert.IsFalse(new StartProjectFinder().ShouldInclude(reader, out guid));
            Assert.IsNull(guid);
        }
    }

    [Test]
    public void Multiple_excluded_project_types()
    {
        using (var reader = Resource.AsStreamReader("Multiple_Exclude.txt"))
        {
            string guid;
            Assert.IsFalse(new StartProjectFinder().ShouldInclude(reader, out guid));
            Assert.IsNull(guid);
        }
    }

    [Test]
    public void Multiple_include_project_types()
    {
        using (var reader = Resource.AsStreamReader("Multiple_Include.txt"))
        {
            string guid;
            Assert.IsTrue(new StartProjectFinder().ShouldInclude(reader, out guid));
            Assert.AreEqual("D04CF1FC-C4C0-4959-A817-2BC68770CA9B", guid);
        }
    }
    [Test]
    public void Lower_project_types()
    {
        using (var reader = Resource.AsStreamReader("Lower_Include.txt"))
        {
            string guid;
            Assert.IsTrue(new StartProjectFinder().ShouldInclude(reader, out guid));
            Assert.AreEqual("D04CF1FC-C4C0-4959-A817-2BC68770CA9B", guid);
        }
    }
  
}