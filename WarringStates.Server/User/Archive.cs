using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;
using WarringStates.Map.Terrain;

namespace WarringStates.Server.User;

internal class Archive
{
    public string LocalName => nameof(Archive);

    public ArchiveInfo Info { get; private set; } = new();

    public AltitudeMap AltitudeMap { get; private set; } = new();

    public Dictionary<string, List<SourceLand>> SourceLands { get; private set; } = [];

    public PlayerRoster Players { get; private set; } = [];

    private Archive(ArchiveInfo info, AltitudeMap altitudeMap)
    {
        Info = info;
        AltitudeMap = altitudeMap;
    }

    private Archive()
    {

    }

    public static Archive Create(ArchiveInfo info, AltitudeMapData mapData)
    {
        Directory.CreateDirectory(Path.Combine(LocalArchives.RootPath, info.Id));
        var altitudeMap = new AltitudeMap(mapData);
        var landMap = new LandMap();
        landMap.Relocate(altitudeMap);
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
        return new(info, altitudeMap);
    }

    public void Save()
    {
        Directory.CreateDirectory(Info.RootPath);
        Info.UpdateLastSaveTime();
        Info.SerializeFile(false, Info.GetArchiveInfoPath(), nameof(Info));
        AltitudeMap.SerializeFile(false, Info.GetAltitudeMapPath(), nameof(AltitudeMap));
        SourceLands.SerializeFile(false, Info.GetSourceLandsPath(), nameof(SourceLands));
        Players.SerializeFile(false, Info.GetPlayersPath(), nameof(Players));
    }

    public static Archive Load(ArchiveInfo info)
    {
        return new Archive
        {
            Info = info,
            AltitudeMap = LoadAltitudeMap(info),
            SourceLands = LoadSourceLands(info),
            Players = LoadPlayers(info),
        };
    }

    public static AltitudeMap LoadAltitudeMap(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<AltitudeMap>(info.GetAltitudeMapPath(), nameof(AltitudeMap)) ?? new();
    }

    public static Dictionary<string, List<SourceLand>> LoadSourceLands(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<Dictionary<string, List<SourceLand>>>(info.GetSourceLandsPath(), nameof(SourceLands)) ?? [];
    }

    public static PlayerRoster LoadPlayers(ArchiveInfo info)
    {
        return SerializeTool.DeserializeFile<PlayerRoster>(info.GetPlayersPath(), nameof(Players)) ?? [];
    }
}
