using LocalUtilities.TypeGeneral;

namespace WarringStates.UI;

public partial class TestCGraphics : ResizeableForm
{
    public override string LocalName => nameof(TestCGraphics);

    PictureBox Displayer { get; } = new()
    {
        Dock = DockStyle.Fill,
    };

    private void DrawClient()
    {
        Displayer.Image = new Bitmap(Displayer.Width, Displayer.Height);
        var g = Graphics.FromImage(Displayer.Image);
        g.Render(Displayer.Width, Displayer.Height);
        g.DrawLine(Pens.Red, new Point(0, 50), new Point(100, 50));
    }

    protected override void InitializeComponent()
    {
        Controls.AddRange([
            Displayer,
            ]);
        OnDrawingClient += DrawClient;
    }
}
