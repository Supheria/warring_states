namespace WarringStates.Net.Common;

public class FileTransferArgs(string dirName, string fileName)
{
    public DateTime StartTime { get; private set; } = DateTime.Now;

    public string DirName { get; private set; } = dirName;

    public string FileName { get; private set; } = fileName;

    public string Md5Value { get; set; } = "";

    public long FileLength { get; set; } = 0;

    public long PacketLength { get; set; } = 0;

    public long FilePosition { get; set; } = 0;

    public FileTransferArgs() : this("", "")
    {

    }
}
