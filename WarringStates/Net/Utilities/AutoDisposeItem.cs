using LocalUtilities.IocpNet;

namespace WarringStates.Net.Utilities;

public abstract class AutoDisposeItem
{
    public event NetEventHandler? OnDisposed;

    protected DaemonThread DaemonThread { get; }

    public DateTime TimeStamp { get; init; }

    public AutoDisposeItem(int disposeMilliseconds)
    {
        DaemonThread = new(disposeMilliseconds, AutoDispose);
    }

    protected virtual void AutoDispose()
    {
        Dispose();
    }

    public void Dispose()
    {
        DaemonThread.Dispose();
        OnDisposed?.Invoke();
    }
}
