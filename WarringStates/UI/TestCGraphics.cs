using LocalUtilities.TypeGeneral;

namespace WarringStates.UI;

public partial class TestCGraphics : ResizeableForm
{
    public override string LocalName => nameof(TestCGraphics);

    PictureBox Displayer { get; } = new()
    {
        Dock = DockStyle.Fill,
    };

    //CBitmap? CBitmap { get; set; }

    private void DrawClient()
    {
        //Displayer.Image = new Bitmap(Displayer.Width, Displayer.Height);
        //var g = Graphics.FromImage(Displayer.Image);
        //g.Render(Displayer.Width, Displayer.Height);
        //g.DrawLine(Pens.Red, new Point(0, 50), new Point(100, 50));
        //CBitmap?.Dispose();
        //((Bitmap)Displayer.Image)?.ReleaseImage();
        //var a = Displayer.Image as Bitmap;
        //a?.ReleaseImage();
        //if (Displayer.Image is null)
        //    Displayer.Image = CGraphicsInterface.GetImage();
        ////Displayer.Image?.Dispose();
        //a = Displayer.Image as Bitmap;
        //Displayer.Image = a.ResizeImage(Displayer.Size);
        ////Displayer.Image = CBitmap.Source;
        //Displayer.Image = new Bitmap(Displayer.Width, Displayer.Height);
    }

    protected override void InitializeComponent()
    {
        Controls.AddRange([
            Displayer,
            ]);
        OnDrawingClient += DrawClient;
    }
}
