using System.Reflection;

namespace Commerce.SharedKernel.Infrastructure;

public interface IRegisterModule
{
    void AddModule(IHostApplicationBuilder builder);
    void UseModule(WebApplication app);
}
public class ModuleRegistry
{
    private readonly IReadOnlyCollection<IRegisterModule> _modules = DiscoverModules();

    public IHostApplicationBuilder AddModuleServices(IHostApplicationBuilder builder)
    {
        foreach (var module in _modules)
        {
            module.AddModule(builder);
        }

        return builder;
    }

    public WebApplication UseModuleServices(WebApplication app)
    {
        foreach (var module in _modules)
        {
            module.UseModule(app);
        }

        return app;
    }

    private static IReadOnlyCollection<IRegisterModule> DiscoverModules()
    {
        return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(filePath => Path.GetFileName(filePath).StartsWith("Commerce.Features."))
            .Select(Assembly.LoadFrom)
            .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(IRegisterModule).IsAssignableFrom(type) &&
                               type is { IsInterface: false, IsAbstract: false }))
            .Select(type => (IRegisterModule)Activator.CreateInstance(type)!)
            .ToList()
            .AsReadOnly();
    }
}