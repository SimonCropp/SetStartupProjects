// ReSharper disable UnusedParameter.Global

static class Guard
{
    public static void AgainstNullAndEmpty(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNonExistingFile(string file, string argumentName)
    {
        if (!File.Exists(file))
        {
            throw new ArgumentException($"File does not exist: {file}", argumentName);
        }
    }
}