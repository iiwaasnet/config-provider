using System;

namespace Tests.config
{
    public class SimpleFlatConfiguration
    {
        public string StringProp { get; set; }
        public bool BooleanProp { get; set; }
        public int IntegerProp { get; set; }
        public DateTime DateTimeProp { get; set; }
        public TimeSpan TimeSpanProp { get; set; }
        public decimal DecimalProp { get; set; }
    }
}