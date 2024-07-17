using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System;
using WarringStates.Map;
using WarringStates.Map.Terrain;

namespace WarringStates.Server.User;

internal class Archive
{
    public ArchiveInfo Info { get; private set; }

    public AltitudeMap AltitudeMap { get; private set; }

    public RandomTable RandomTable { get; private set; }

    public Dictionary<string, List<SourceLand>> SourceLands { get; private set; } = [];

    public PlayerRoster Players { get; private set; } = [];

    private Archive(ArchiveInfo info, AltitudeMap altitudeMap, RandomTable randomTable)
    {
        Info = info;
        AltitudeMap = altitudeMap;
        RandomTable = randomTable;
    }

    private Archive() : this(new(), new(), new())
    {

    }

    public static Archive Create(ArchiveInfo info, AltitudeMapData mapData)
    {
        Directory.CreateDirectory(Path.Combine(LocalArchives.RootPath, info.Id));
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
        thumbnail.Save(info.GetThumbnailPath());
        return new(info, altitudeMap, randomTable);
    }

    public void Save()
    {
        Directory.CreateDirectory(Info.RootPath);
        Info.UpdateLastSaveTime();
        SerializeTool.SerializeFile(Info, true, Info.GetArchiveInfoPath(), nameof(Info), null);
        SerializeTool.SerializeFile(AltitudeMap, false, Info.GetAltitudeMapPath(), nameof(AltitudeMap), null);
        SerializeTool.SerializeFile(RandomTable, false, Info.GetRandomTablePath(), nameof(RandomTable), null);
        SerializeTool.SerializeFile(SourceLands, true, Info.GetSourceLandsPath(), nameof(SourceLands), null);
        SerializeTool.SerializeFile(Players, true, Info.GetPlayersPath(), nameof(Players), null);
    }

    public static Archive Load(ArchiveInfo info)
    {
        return new Archive
        {
            Info = info,
            AltitudeMap = LoadAltitudeMap(info),
            RandomTable = LoadRandomTable(info),
            SourceLands = LoadSourceLands(info),
            Players = LoadPlayers(info),
        };
    }

    public static AltitudeMap LoadAltitudeMap(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<AltitudeMap>(info.GetAltitudeMapPath(), nameof(AltitudeMap), null) ?? new();
    }

    public static RandomTable LoadRandomTable(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<RandomTable>(info.GetRandomTablePath(), nameof(RandomTable), null) ?? new();
    }

    public static Dictionary<string, List<SourceLand>> LoadSourceLands(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<Dictionary<string, List<SourceLand>>>(info.GetSourceLandsPath(), nameof(SourceLands), null) ?? [];
    }

    public static PlayerRoster LoadPlayers(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<PlayerRoster>(info.GetPlayersPath(), nameof(Players), null) ?? [];
    }
}
