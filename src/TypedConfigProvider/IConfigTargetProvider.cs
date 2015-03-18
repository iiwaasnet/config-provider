using System.Collections.Generic;

namespace TypedConfigProvider
{
    public interface IConfigTargetProvider
    {
        IEnumerable<string> GetTargetsSequence();
    } 
}