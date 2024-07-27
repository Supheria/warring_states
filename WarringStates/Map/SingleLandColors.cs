namespace WarringStates.Map;

public class SingleLandColors : ColorSelector
{
    protected override Dictionary<Enum, Color> Colors { get; set; } = new()
    {
        [SingleLandTypes.None] = Color.Black,
        [SingleLandTypes.Plain] = Color.LightYellow,
        [SingleLandTypes.Wood] = Color.LimeGreen,
        [SingleLandTypes.Stream] = Color.SkyBlue,
        [SingleLandTypes.Hill] = Color.DarkSlateGray,
    };
}
