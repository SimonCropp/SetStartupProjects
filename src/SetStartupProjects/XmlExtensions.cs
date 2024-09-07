static class XmlExtensions
{
    public static void StripNamespace(this XDocument document)
    {
        if (document.Root == null)
        {
            return;
        }

        foreach (var element in document.Root.DescendantsAndSelf())
        {
            element.Name = element.Name.LocalName;
            element.ReplaceAttributes(GetAttributes(element));
        }
    }

    static IEnumerable GetAttributes(XElement element) =>
        element.Attributes()
            .Where(_ => !_.IsNamespaceDeclaration)
            .Select(_ => new XAttribute(_.Name.LocalName, _.Value));
}