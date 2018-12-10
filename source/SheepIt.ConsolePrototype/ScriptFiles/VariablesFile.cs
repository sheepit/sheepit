using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SheepIt.ConsolePrototype.ScriptFiles
{
    public class VariablesFile
    {
        public static VariablesFile Open(string path)
        {
            using (var fileStream = File.OpenText(path))
            {
                return new DeserializerBuilder()
                    .WithNamingConvention(new UnderscoredNamingConvention())
                    .Build()
                    .Deserialize<VariablesFile>(fileStream);
            }
        }

        public Dictionary<string, VariableValues> Variables { get; set; }

        public Variable[] GetForEnvironment(string environment)
        {
            return Variables
                .Select(pair => new Variable(pair.Key, pair.Value.ValueForEnvironment(environment)))
                .ToArray();
        }
    }

    public class Variable
    {
        public string Name { get; }
        public string Value { get; }

        public Variable(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public class VariableValues
    {
        public string Default { get; set; }

        public Dictionary<string, string> Environments { get; set; }

        public string ValueForEnvironment(string environment)
        {
            if (Environments != null && Environments.TryGetValue(environment, out var environmentSpecificValue))
            {
                return environmentSpecificValue;
            }

            return Default;
        }
    }
}