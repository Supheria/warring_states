using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;
using WarringStates.Map.Terrain;
using WarringStates.User;

namespace WarringStates.Server.User;

internal class Archive
{
    public string LocalName => nameof(Archive);

    public ArchiveInfo Info { get; private set; } = new();

    public AltitudeMap AltitudeMap { get; private set; } = new();

    public SourceLandsOwnerMap SourceLands { get; private set; } = [];

    public List<Player> Players { get; private set; } = [];

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
        Info.SaveToSimpleScript(false, Info.GetArchiveInfoPath());
        AltitudeMap.SaveToSimpleScript(false, Info.GetAltitudeMapPath());
        SourceLands.SaveToSimpleScript(false, Info.GetSourceLandsPath());
        Players.SaveToSimpleScript(nameof(Players), false, Info.GetPlayersPath());
    }

    public static Archive Load(ArchiveInfo info)
    {
        return new Archive
        {
            Info = info,
            AltitudeMap = new AltitudeMap().LoadFromSimpleScript(info.GetAltitudeMapPath()),
            SourceLands = new SourceLandsOwnerMap().LoadFromSimpleScript(info.GetSourceLandsPath()),
            Players = SerializeTool.LoadFromSimpleScript<Player>(nameof(Players), info.GetPlayersPath())
        };
    }
}
