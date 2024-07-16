using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.Server.User;

internal class ArchiveInfo
{
    public string Id { get; private set; } = "";

    public string RootPath { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public Size WorldSize { get; private set; } = new();

    public long CreateTime { get; private set; } = 0;

    public long LastSaveTime { get; private set; } = 0;

    public int CurrentSpan { get; private set; } = 0;

    public ArchiveInfo(string worldName, Size worldSize)
    {
        WorldName = worldName;
        WorldSize = worldSize;
        LastSaveTime = CreateTime = DateTime.Now.ToBinary();
        Id = (WorldName + CreateTime).ToMd5HashString();
        RootPath = Path.Combine(LocalArchives.RootPath, Id);
    }

    public ArchiveInfo()
    {

    }

    public void UpdateLastSaveTime()
    {
        LastSaveTime = DateTime.Now.ToBinary();
    }

    public string GetArchiveInfoPath()
    {
        return Path.Combine(RootPath, "info");
    }

    public string GetThumbnailPath()
    {
        return Path.Combine(RootPath, "thumbnail");
    }

    public string GetAltitudeMapPath()
    {
        return Path.Combine(RootPath, "altitude map");
    }

    public string GetSourceLandsPath()
    {
        return Path.Combine(RootPath, "source lands");
    }

    public string GetPlayersPath()
    {
        return Path.Combine(RootPath, "players");
    }
}
