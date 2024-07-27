namespace WarringStates.Map;

public class SourceLandColors : ColorSelector
{
    protected override Dictionary<Enum, Color> Colors { get; set; } = new()
    {
        [LandTypes.None] = Color.White,
        [LandTypes.HorseLand] = Color.LightSalmon,
        [LandTypes.MineLand] = Color.DarkSlateGray,
        [LandTypes.FarmLand] = Color.Gold,
        [LandTypes.MulberryLand] = Color.MediumPurple,
        [LandTypes.WoodLand] = Color.ForestGreen,
        [LandTypes.FishLand] = Color.RoyalBlue,
        [LandTypes.TerraceLand] = Color.YellowGreen,
    };
}