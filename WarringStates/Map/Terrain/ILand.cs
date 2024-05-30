using WarringStates.Graph;

namespace WarringStates.Map.Terrain;

public interface ILand
{
    /// <summary>
    /// comparing should use <see cref="Enum.Equals(object?)"/>,
    /// because <see href="=="/> will compare <see cref="Enum"/> as <see cref="object"/>,
    /// which may cause problem
    /// </summary>
    public Enum Type { get; }

    public Color Color { get; }

    public int DrawCell(Graphics? g, LatticeGrid.Cell cell, Rectangle drawRect, Color backColor, ILand? lastLand);
}
