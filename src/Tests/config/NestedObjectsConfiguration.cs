using System.Collections.Generic;

namespace Tests.config
{
    public class NestedObjectsConfiguration
    {
        public IEnumerable<ItemConfiguration> Items { get; set; }
    }

    public class ItemConfiguration
    {
        public string Name { get; set; }
    }
}