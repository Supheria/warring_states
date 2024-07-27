namespace WarringStates.Map;

public class SingleLandColors : ColorSelector
{
    protected override Dictionary<Enum, Color> Colors { get; set; } = new()
    {
        [LandTypes.None] = Color.White,
        [LandTypes.Plain] = Color.LightYellow,
        [LandTypes.Wood] = Color.LimeGreen,
        [LandTypes.Stream] = Color.SkyBlue,
        [LandTypes.Hill] = Color.DarkSlateGray,
    };
}
