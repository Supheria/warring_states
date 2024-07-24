using WarringStates.Events;
using WarringStates.Map.Terrain;

namespace WarringStates.Client.User;

internal class ThumbnailRedrawArgs : ICallbackArgs
{
    public Size WorldSize { get; set; } = new();

    public List<SourceLand> OwnerShip { get; set; } = [];

    public int CurrentSpan { get; set; } = 0;

    public int PlayerCount { get; set; } = 0;

    public int Width => WorldSize.Width;

    public int Height => WorldSize.Height;
}