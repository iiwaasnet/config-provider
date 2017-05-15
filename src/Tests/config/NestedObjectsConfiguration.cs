using System.Collections.Generic;

namespace Tests.config
{
    public class NestedObjectsConfiguration
    {
        public Entity Entity { get; set; }

        public string Prop { get; set; }
    }

    public class ItemConfiguration
    {
        public string Name { get; set; }
    }

    public class Entity
    {
        public IEnumerable<ItemConfiguration> Items { get; set; }

        public string Prop { get; set; }
    }
}