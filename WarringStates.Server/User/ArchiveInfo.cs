using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.SQLiteHelper.Data;
using LocalUtilities.TypeToolKit;
using LocalUtilities.TypeToolKit.Text;
using WarringStates.Map.Terrain;
using WarringStates.Map;
using WarringStates.User;
using LocalUtilities.TypeGeneral;

namespace WarringStates.Server.User;

[Table]
internal class ArchiveInfo
{
    public string RootPath => Path.Combine(LocalArchives.RootPath, Id);

    [TableField(IsPrimaryKey = true)]
    public string Id { get; private set; } = "";

    public string WorldName { get; private set; } = "";

    public Size WorldSize { get; private set; } = new();

    public long CreateTime { get; private set; } = 0;

    public long LastSaveTime { get; private set; } = 0;

    static SsSignTable SignTable { get; } = new();

    public ArchiveInfo(string worldName, Size worldSize)
    {
        WorldName = worldName;
        WorldSize = worldSize;
        LastSaveTime = CreateTime = DateTime.Now.ToBinary();
        Id = (WorldName + CreateTime).ToMd5HashString();
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

    public string GetRandomTablePath()
    {
        return Path.Combine(RootPath, "random table");
    }

    public string GetSourceLandsPath()
    {
        return Path.Combine(RootPath, "source lands");
    }

    public string GetPlayersPath()
    {
        return Path.Combine(RootPath, "players");
    }

    public string GetCurrentSpanPath()
    {
        return Path.Combine(RootPath, "current span");
    }

    public AltitudeMap LoadAltitudeMap()
    {
        return SerializeTool.DeserializeFile<AltitudeMap>(new(nameof(Archive.AltitudeMap)), GetAltitudeMapPath(), SignTable) ?? new();
    }

    public RandomTable LoadRandomTable()
    {
        return SerializeTool.DeserializeFile<RandomTable>(new(nameof(Archive.RandomTable)), GetRandomTablePath(), SignTable) ?? new();
    }

    public Dictionary<string, List<SourceLand>> LoadSourceLands()
    {
        return SerializeTool.DeserializeFile<Dictionary<string, List<SourceLand>>>(new(nameof(Archive.SourceLands)), GetSourceLandsPath(), SignTable) ?? [];
    }

    public Players LoadPlayers()
    {
        return SerializeTool.DeserializeFile<Players>(new(nameof(Archive.Players)), GetPlayersPath(), SignTable) ?? [];
    }

    public int LoadCurrentSpan()
    {
        return SerializeTool.DeserializeFile<int>(new(nameof(Archive.CurrentSpan)), GetCurrentSpanPath(), SignTable);
    }

    public Archive Load()
    {
        return new()
        {
            //Info = info,
            AltitudeMap = LoadAltitudeMap(),
            RandomTable = LoadRandomTable(),
            SourceLands = LoadSourceLands(),
            Players = LoadPlayers(),
        };
    }

    public void Save(Archive archive)
    {
        SerializeTool.SerializeFile(archive.AltitudeMap, new(nameof(Archive.AltitudeMap)), GetAltitudeMapPath(), false, SignTable);
        SerializeTool.SerializeFile(archive.RandomTable, new(nameof(Archive.RandomTable)), GetRandomTablePath(), false, SignTable);
        SerializeTool.SerializeFile(archive.SourceLands, new(nameof(Archive.SourceLands)), GetSourceLandsPath(), true, SignTable);
        SerializeTool.SerializeFile(archive.Players, new(nameof(Archive.Players)), GetPlayersPath(), true, SignTable);
        SerializeTool.SerializeFile(archive.CurrentSpan, new(nameof(Archive.CurrentSpan)), GetCurrentSpanPath(), true, SignTable);
    }

    public Archive Create(AltitudeMapData mapData)
    {
        Directory.CreateDirectory(Path.Combine(LocalArchives.RootPath, Id));
        var altitudeMap = new AltitudeMap(mapData);
        var landMap = new LandMap();
        var randomTable = new RandomTable(1000);
        landMap.Relocate(altitudeMap, randomTable);
        using var thumbnail = new Bitmap(landMap.Width, landMap.Height);
        var pThumbnail = new PointBitmap(thumbnail);
        pThumbnail.LockBits();
        for (int i = 0; i < landMap.Width; i++)
        {
            for (int j = 0; j < landMap.Height; j++)
            {
                var color = landMap[new(i, j)].Color;
                pThumbnail.SetPixel(i, j, color);
            }
        }
        pThumbnail.UnlockBits();
        thumbnail.Save(GetThumbnailPath());
        return new(/*info, */altitudeMap, randomTable);
    }
}
