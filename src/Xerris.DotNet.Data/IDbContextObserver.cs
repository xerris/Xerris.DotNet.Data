using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;

namespace Xerris.DotNet.Data;

public interface IDbContextObserver : IDisposable
{
    void OnEntityTracked(object sender, EntityTrackedEventArgs e);
}

public sealed class DefaultDbContextObserver : IDbContextObserver
{
    public void Dispose()
        => Log.Debug("Disposing {@type}", GetType().Name);

    public void OnEntityTracked(object sender, EntityTrackedEventArgs e)
        => Log.Debug("OnEntityTracked {@type}", e.GetType().Name);
}