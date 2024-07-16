namespace WarringStates.Client.Graph;

public class GridData
{
    public double GuideLineWidth { get; set; } = 1.75;

    public Color GuideLineColor { get; set; } = Color.Red;

    public SolidBrush GuideLineBrush { get; } = new(Color.Transparent);
}
