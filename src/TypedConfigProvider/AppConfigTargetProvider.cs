using System;
using System.Collections.Generic;
using System.Configuration;

namespace TypedConfigProvider
{
    public class AppConfigTargetProvider : IConfigTargetProvider
    {
        private const string ConfigurationKey = "JsonConfig.EnvironmentSequence";
        private readonly IEnumerable<string> targetsSequence;

        public AppConfigTargetProvider()
        {
            targetsSequence = ReadTargetsSequence();
        }

        private IEnumerable<string> ReadTargetsSequence()
        {
            var sequence = ConfigurationManager.AppSettings[ConfigurationKey];
            if (string.IsNullOrWhiteSpace(sequence))
            {
                throw new ArgumentException($"{ConfigurationKey} not configured!");
            }

            return sequence.Split(',');
        }

        public IEnumerable<string> GetTargetsSequence()
            => targetsSequence;
    }
}