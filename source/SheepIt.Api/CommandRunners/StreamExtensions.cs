using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SheepIt.Api.CommandRunners
{
    public static class StreamReaderExtensions
    {
        public static string[] ReadLinesToEnd(this StreamReader reader)
        {
            return reader.ReadLines().ToArray();
        }

        public static IEnumerable<string> ReadLines(this StreamReader reader)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}