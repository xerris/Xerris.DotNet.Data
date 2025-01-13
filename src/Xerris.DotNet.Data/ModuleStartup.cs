using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xerris.DotNet.Core.DI;

namespace Xerris.DotNet.Data;

public class ModuleStartup : IModule
{
    public void RegisterServices(IServiceCollection services)
        => services.TryAddScoped<IDbContextObserver, DefaultDbContextObserver>();

    public int Priority => 1;
}