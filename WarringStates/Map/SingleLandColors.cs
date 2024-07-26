using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Map;

public class SingleLandColors : ColorSelector
{
    protected override Dictionary<Enum, Color> Colors { get; set; } = new()
    {
        [LandTypes.Plain] = Color.LightYellow,
        [LandTypes.Wood] = Color.LimeGreen,
        [LandTypes.Stream] = Color.SkyBlue,
        [LandTypes.Hill] = Color.DarkSlateGray,
    };
}
