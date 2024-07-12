using LocalUtilities.IocpNet.Common;

namespace WarringStates.Net.Common;

public class AutoDisposeFileStream : AutoDisposeItem
{
    FileStream FileStream { get; set; }

    public long Length => FileStream?.Length ?? 0;

    public long Position
    {
        get => FileStream?.Position ?? 0;
        set
        {
            if (FileStream is not null)
                FileStream.Position = value;
        }
    }

    public AutoDisposeFileStream(FileStream fileStream, DateTime timeStamp) : base(ConstTabel.FileStreamExpireMilliseconds)
    {
        FileStream = fileStream;
        TimeStamp = timeStamp;
        OnDisposed += FileStream.Dispose;
        DaemonThread.Start();
    }

    public void Read(byte[] buffer, out int readCount)
    {
        readCount = 0;
        DaemonThread.Stop();
        try
        {
            readCount = FileStream.Read(buffer);
        }
        finally
        {
            DaemonThread.Start();
        }
    }

    public void Write(byte[] buffer)
    {
        DaemonThread.Stop();
        try
        {
            FileStream.Write(buffer);
        }
        finally
        {
            DaemonThread.Start();
        }
        DaemonThread.Start();
    }
}
