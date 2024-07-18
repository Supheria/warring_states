using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System;
using WarringStates.Map;
using WarringStates.Map.Terrain;

namespace WarringStates.Server.User;

internal class Archive
{
    public AltitudeMap AltitudeMap { get; set; }

    public RandomTable RandomTable { get; set; }

    public Dictionary<string, List<SourceLand>> SourceLands { get; set; } = [];

    public Players Players { get; set; } = [];

    public int CurrentSpan { get; private set; } = 0;

    public Archive(/*ArchiveInfo info, */AltitudeMap altitudeMap, RandomTable randomTable)
    {
        //Info = info;
        AltitudeMap = altitudeMap;
        RandomTable = randomTable;
    }

    public Archive() : this(/*new(), */new(), new())
    {

    }
}
