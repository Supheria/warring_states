using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates;

public class GridToUpdateEventArgument(Image source, Rectangle drawRect, Color backColor, Coordinate originOffset) : IEventArgument
{
    public Image Source { get; } = source;

    public Rectangle DrawRect { get; } = drawRect;

    public Color BackColor { get; } = backColor;

    public Coordinate OriginOffset { get; } = originOffset;

    public GridToUpdateEventArgument(Image source, Rectangle drawRect, Color backColor) : this(source, drawRect, backColor, new())
    {

    }
}
