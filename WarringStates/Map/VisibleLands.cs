namespace WarringStates.Map;

public class VisibleLands
{
    public List<SingleLand> SingleLands { get; private set; } = [];

    public List<SourceLand> SourceLands { get; private set; } = [];

    public void Add(Land land)
    {
        if (land is SingleLand singleLand)
            SingleLands.Add(singleLand);
        else if (land is SourceLand sourceLand)
            SourceLands.Add(sourceLand);
    }
}
