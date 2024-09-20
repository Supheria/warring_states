using AltitudeMapGenerator;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Flow;
using WarringStates.Map;
using WarringStates.Server.UI.Component;
using WarringStates.User;

namespace WarringStates.Server.Map;

internal partial class Atlas
{
    static LandMapEx LandMap { get; set; } = new();

    public static Size Size => LandMap.WorldSize;

    public static int Width => LandMap.WorldWidth;

    public static int Height => LandMap.WorldHeight;

    public static OwnerSite SetRandomSite(string playerName)
    {
        var owner = LandMap.SetRandomSite(playerName);
        SetOwnerSites(owner.Site, owner.LandType, playerName);
        return owner;
    }

    public static void GetVision(Coordinate site, VisibleLands vision)
    {
        LandMap.GetVision(site, vision);
    }

    public static VisibleLands GetVision(string playerName)
    {
        var visibleLands = new VisibleLands();
        var ownerSites = GetOwnerSites(playerName);
        foreach (var ownerSite in ownerSites)
        {
            LandMap.GetVision(ownerSite.Site, visibleLands);
        }
        return visibleLands;
    }

    public static bool AddSouceLand(Coordinate site, SourceLandTypes type)
    {
        return LandMap.AddSouceLand(site, type);
    }

    public static SourceLandTypes[] GetCanBuildTypes(Coordinate site)
    {
        return LandMap.GetCanBuildTypes(site);
    }
}
