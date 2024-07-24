namespace WarringStates.Net.Common;

public class FileTransferArgs(string dirName, string filePath)
{
    public DateTime StartTime { get; private set; } = DateTime.Now;

    public string DirName { get; private set; } = dirName;

    public string FilePath { get; private set; } = filePath;

    public string FileName => Path.GetFileName(FilePath);

    public string Md5Value { get; set; } = "";

    public long FileLength { get; set; } = 0;

    public long PacketLength { get; set; } = 0;

    public long FilePosition { get; set; } = 0;

    public FileTransferArgs() : this("", "")
    {

    }
}
