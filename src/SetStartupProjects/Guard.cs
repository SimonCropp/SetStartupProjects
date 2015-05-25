using System;
using System.Collections;
using System.IO;

// ReSharper disable UnusedParameter.Global

namespace SetStartupProjects
{
    static class Guard
    {

        public static void AgainstNullAndEmpty(string value, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void AgainstNonExistingDirectory(string directory, string argumentName)
        {
            if (!Directory.Exists(directory))
            {
                var message = string.Format("Directory does not exist: {0}", directory);
                throw new ArgumentException(message, argumentName);
            }
        }
        public static void AgainstNonExistingFile(string file, string argumentName)
        {
            if (!File.Exists(file))
            {
                var message = string.Format("File does not exist: {0}", file);
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void AgainstNullAndEmpty(ICollection value, string argumentName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
            if (value.Count == 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }
    }
}