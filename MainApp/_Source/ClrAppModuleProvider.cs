namespace MainApp;

public class ClrAppModuleFactory
{
    public void AddModule()
    {
    }
}

public class ClrAppModuleProvider
{
}

public static class ServiceExtensions
{
    public static IServiceCollection AddClrApplicationModules(this IServiceCollection services, Action<ClrAppModuleFactory> action)
    {
        var factory = new ClrAppModuleFactory();
        action.Invoke(factory);
        services.AddSingleton<ClrAppModuleProvider>(s =>
            new ClrAppModuleProvider()
        );
        return services;
    }
}
