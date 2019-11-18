namespace TypedConfigProvider
{
    public interface IConfigProvider
    {
        T GetConfiguration<T>()
            where T : class, new();
        
        T GetConfigurationForTargets<T>(params string[] targets)
            where T : class, new();
    }
}