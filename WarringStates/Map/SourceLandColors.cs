using LocalUtilities;

namespace WarringStates.Map;

public class SourceLandColors : ColorSelector
{
    protected override Dictionary<Enum, Color> Colors { get; set; } = new()
    {
        [SourceLandTypes.None] = Color.Black,
        [SourceLandTypes.HorseLand] = Color.LightSalmon,
        [SourceLandTypes.MineLand] = Color.DarkSlateGray,
        [SourceLandTypes.FarmLand] = Color.Gold,
        [SourceLandTypes.MulberryLand] = Color.MediumPurple,
        [SourceLandTypes.WoodLand] = Color.ForestGreen,
        [SourceLandTypes.FishLand] = Color.RoyalBlue,
        [SourceLandTypes.TerraceLand] = Color.YellowGreen,
    };
}