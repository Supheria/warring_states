using LocalUtilities.TypeGeneral;
using WarringStates.Data;
using WarringStates.UI;

namespace WarringStates.Client.UI.Component;

internal partial class LandBuilder : Pannel
{
    public override Size Padding { get; set; } = new(30, 30);

    public static int ButtonHeight { get; set; } = 50;

    public static Color FrontColor { get; set; } = Color.DeepSkyBlue;

    public static new Color BackColor { get; set; } = Color.White;

    public static Color ButtonBackColor { get; set; } = Color.LightYellow;

    public static Color ButtonFrontColor { get; set; } = Color.DarkSlateGray;

    Rectangle Range { get; set; } = new();

    public new Rectangle Bounds
    {
        get => base.Bounds;
        set
        {
            Range = value;
            if (Visible)
            {
                base.Bounds = Range;
            }
            else
            {
                base.Bounds = new();
            }
        }
    }

    ImageButton BuildButton { get; } = new()
    {
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
    };

    ImageButton CloseButton { get; } = new()
    {
        Text = Localize.Table.X,
        FrontColor = ButtonFrontColor,
        BackColor = ButtonBackColor,
        CanSelect = true,
    };

    Selector BuildTypeSelector { get; } = new()
    {
        FrontColor = BackColor,
        BackColor = FrontColor,
    };

    public LandBuilder()
    {
        base.BackColor = BackColor;
        Controls.AddRange([
            CloseButton,
            BuildButton,
            BuildTypeSelector,
            ]);
        Visible = false;
    }

    protected override void SetSize()
    {
        if (!Visible)
            return;
        base.SetSize();
        //
        BuildTypeSelector.Bounds = new(
            ClientLeft,
            ClientTop,
            ClientWidth,
            ClientHeight - ButtonHeight - Padding.Height
            );
        //
        BuildButton.Bounds = new(
            ClientLeft,
            BuildTypeSelector.Bottom + Padding.Height / 2,
            ClientWidth,
            ButtonHeight
            );
        //
        CloseButton.Bounds = new(
            ClientRight - Padding.Width,
            ClientTop,
            Padding.Width,
            Padding.Height
            );
    }
}
