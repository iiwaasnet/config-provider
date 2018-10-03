using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TypedConfigProvider
{
    public class CoreEnvironmentConfigTargetProvider : IConfigTargetProvider
    {
        private const string ConfigurationKey = "JsonConfig.EnvironmentSequence";
        private readonly IEnumerable<string> targetsSequence;

        public CoreEnvironmentConfigTargetProvider(IConfiguration config)
            => targetsSequence = ReadTargetsSequence(config);

        private static IEnumerable<string> ReadTargetsSequence(IConfiguration config)
        {
            var sequence = config[ConfigurationKey] ?? config["ASPNETCORE_ENVIRONMENT"];
            if (string.IsNullOrWhiteSpace(sequence))
            {
                throw new ArgumentException($"{ConfigurationKey} not configured!");
            }

            return sequence.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                           .Select(t => t.Trim())
                           .ToList();
        }

        public IEnumerable<string> GetTargetsSequence()
            => targetsSequence;
    }
}