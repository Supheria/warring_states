using AtlasGenerator;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates;

internal class AtlasManager
{
    public static Atlas Atlas
    {
        get => _atlas;
        set
        {
            _atlas = value;
            foreach(var points in _atlas.PixelsMap.Values)
            {
                foreach(var point in points)
                {
                    Terrain.GetTerrain(point.Altitude / _atlas.AltitudeMax);
                }
            }
            
        }
    }
    private static Atlas _atlas;

    static Dictionary<(int X, int Y), Terrain.Type> TerrainMap { get; } = [];

}
