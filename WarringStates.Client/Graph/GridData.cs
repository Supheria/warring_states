namespace WarringStates.Client.Graph;

public class GridData
{
    public float GuideLineWidth { get; set; } = 1.75f;

    public Color GuideLineColor { get; set; } = Color.Red;

    public SolidBrush GuideLineBrush { get; } = new(Color.Transparent);
}
