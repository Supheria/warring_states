namespace WarringStates.Map;

public abstract class ColorSelector
{
    protected abstract Dictionary<Enum, Color> Colors { get; set; }

    public Color this[Enum type] => Colors[type];

    public void SetColor(Enum type, string colorName)
    {
        Colors[type] = Color.FromName(colorName);
    }
}
