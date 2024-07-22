using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map.Terrain;

namespace WarringStates.Client.User;

internal class ThumbnailInfo
{
    public Size WorldSize { get; set; } = new();

    public List<SourceLand> OwnerShip { get; set; } = [];

    public int CurrentSpan { get; set; } = 0;

    public int Width => WorldSize.Width;

    public int Height => WorldSize.Height;
}