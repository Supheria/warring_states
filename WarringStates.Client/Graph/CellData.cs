namespace WarringStates.Client.Graph;

public class CellData
{
    public int EdgeLengthMin { get; set; } = 25;

    public int EdgeLengthMax { get; set; } = 125;

    public int EdgeLength
    {
        get => _edgeLength;
        set => _edgeLength = value < EdgeLengthMin || value > EdgeLengthMax ? _edgeLength : value;
    }
    int _edgeLength = 30;

    public double CenterPaddingFactorMin { get; set; } = 0.01;

    public double CenterPaddingFactorMax { get; set; } = 0.4;

    public double CenterPaddingFactor
    {
        get => _centerPaddingFactor;
        set => _centerPaddingFactor = value < CenterPaddingFactorMin || value > CenterPaddingFactorMax ? _centerPaddingFactor : value;
    }
    double _centerPaddingFactor = 0.2;
}
