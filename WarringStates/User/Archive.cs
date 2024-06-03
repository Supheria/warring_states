using AltitudeMapGenerator;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using WarringStates.Map;
using WarringStates.Terrain;

namespace WarringStates.User;

public class Archive : ISsSerializable
{
    public string LocalName => nameof(Archive);

    public ArchiveInfo Info { get; private set; } = new();

    public AltitudeMap AltitudeMap { get; private set; } = new();

    public List<SourceLand> SourceLands { get; private set; } = [];

    private Archive(ArchiveInfo info, AltitudeMap altitudeMap)
    {
        Info = info;
        AltitudeMap = altitudeMap;
    }

    public Archive()
    {

    }

    public static Archive Create(ArchiveInfo info, AltitudeMapData mapData)
    {
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
        thumbnail.Save(info.GetOverviewPath());
        return new(info, altitudeMap);
    }

    public bool Useable()
    {
        return Info.Useable() && AltitudeMap.OriginPoints.Count > 0;
    }

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteObject(Info);
        serializer.WriteObject(AltitudeMap);
        serializer.WriteObjects(nameof(SourceLands), SourceLands);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        deserializer.ReadObject(Info);
        deserializer.ReadObject(AltitudeMap);
        SourceLands = deserializer.ReadObjects<SourceLand>(nameof(SourceLands));
    }
}
