using System;
using Microsoft.Extensions.DependencyInjection;

namespace MainApp;

public class ClrAppModuleFactory
{
    public void AddModule()
    {
    }
}

public class ClrAppModuleProvider
{

    public void test()
    {
        var ag = new MainApp.Agent(null);
    }
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
