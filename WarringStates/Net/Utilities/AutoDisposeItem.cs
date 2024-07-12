using LocalUtilities.IocpNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
