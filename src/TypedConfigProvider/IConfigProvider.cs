namespace TypedConfigProvider
{
    public interface IConfigProvider
    {
        T GetConfiguration<T>()
            where T : class, new();
    }
}