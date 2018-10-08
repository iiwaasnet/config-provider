using System;
using System.Collections.Generic;
using System.Linq;

namespace TypedConfigProvider
{
    public class WindowsEnvironmentConfigTargetProvider : IConfigTargetProvider
    {
        private const string VariableKey = "JsonConfig.EnvironmentSequence";
        private readonly IEnumerable<string> targetsSequence;

        public WindowsEnvironmentConfigTargetProvider()
            => targetsSequence = ReadTargetsSequence();

        private static IEnumerable<string> ReadTargetsSequence()
        {
            var sequence = Environment.GetEnvironmentVariable(VariableKey);
            if (string.IsNullOrWhiteSpace(sequence))
            {
                throw new ArgumentException($"Environment variable {VariableKey} is not configured!");
            }

            return sequence.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                           .Select(t => t.Trim())
                           .ToList();
        }

        public IEnumerable<string> GetTargetsSequence()
            => targetsSequence;
    }
}