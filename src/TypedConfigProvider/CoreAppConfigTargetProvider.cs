using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TypedConfigProvider
{
    public class CoreAppConfigTargetProvider : IConfigTargetProvider
    {
        private const string ConfigurationKey = "JsonConfig.EnvironmentSequence";
        private readonly IEnumerable<string> targetsSequence;

        public CoreAppConfigTargetProvider(string appSettingFileName)
            => targetsSequence = ReadTargetsSequence(appSettingFileName);

        public CoreAppConfigTargetProvider()
            : this("appsettings.json")
        {
        }

        private static IEnumerable<string> ReadTargetsSequence(string appSettingFileName)
        {
            var builder = new ConfigurationBuilder().AddJsonFile(appSettingFileName, false, true);
            var config = builder.Build();

            var sequence = config[ConfigurationKey];
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