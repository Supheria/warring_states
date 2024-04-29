using LocalUtilities.VoronoiDiagram;
using System.Drawing.Drawing2D;

namespace DlaTest;

/// <summary>
/// Description of MainForm.
/// </summary>
public partial class VoronoiForm : Form
{
    Bitmap bitmap;
    Graphics g;
    static int edgeLenth = 0;
    VoronoiPlane? Plane;
    int shit = 0;

    public VoronoiForm()
    {
        //
        // The InitializeComponent() call is required for Windows Forms designer support.
        //
        InitializeComponent();

        pb.AutoSize = true;
        bitmap = new Bitmap(1050, 1050);

        g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.Clear(Color.White);
        pb.Image = bitmap;
        this.AutoSize = true;
    }

    void spreadPoints()
    {
        g.Clear(Color.White);
        var pts = new List<(double, double)>()
        {
            new(132, 22),
new(187, 383),
new(104, 470),
new(66, 699),
new(143, 802),
new(233, 65),
new(255, 299),
new(224, 436),
new(386, 785),
new(327, 998),
new(491, 77),
new(409, 246),
new(474, 426),
new(460, 686),
new(531, 972),
new(623, 91),
new(650, 397),
new(624, 494),
new(730, 765),
new(615, 940),
new(967, 192),
new(908, 339),
new(950, 562),
new(922, 616),
new(818, 944),
new(616, 940),

        };
        Plane = new VoronoiPlane(0, 0, 1000, 1000);
        Plane.Generate(pts);
        var cell = Plane.Cells[shit]; 
        g.FillEllipse(Brushes.Blue, (float)cell.Site.X - 1.5f, (float)cell.Site.Y - 1.5f, 3, 3);
        var centroid = cell.Centroid;
        g.FillEllipse(Brushes.Red, (float)centroid.X - 3f, (float)centroid.Y - 3f, 6, 6);
        g.FillEllipse(Brushes.Green, (float)cell.CellVertices[edgeLenth].X - 3f, (float)cell.CellVertices[edgeLenth].Y - 3f, 6, 6);
        g.DrawPolygon(Pens.Gray, cell.CellVertices.Select(p=>new PointF((float)p.X, (float)p.Y)).ToArray());var a = 100;
        var ps = new Pen[]
        { 
            new(Color.FromArgb(a, Color.Cyan)),
            new(Color.FromArgb(a, Color.Gray)), 
            new(Color.FromArgb(a, Color.Pink)),
            new(Color.FromArgb(a, Color.Green))
        };
        int i = 0;
        foreach(var c in Plane.Cells)
        {
            g.DrawPolygon(ps[i++ % 3], c.CellVertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray());
        }
        var pens = new Pen[]
        {
            Pens.LightPink,
            Pens.LightBlue,
            Pens.LightGreen,
        };
        pb.Image = bitmap;
    }

    void Button1Click(object sender, EventArgs e)
    {
        //this.richTextBox1.Text += "\n******* NEW TEST *******";
        spreadPoints();
        //background = Clone32BPPBitmap ( bitmap );
    }

    void NumericUpDown1ValueChanged(object sender, EventArgs e)
    {
        edgeLenth = (int)(numericUpDown1.Value);
        spreadPoints();
        //background = Clone32BPPBitmap ( bitmap );
    }

    private void NumericUpDown2ValueChanged(object sender, EventArgs e)
    {
        shit = (int)(numericUpDown2.Value);
        spreadPoints();
    }

    void PbMouseMove(object sender, MouseEventArgs e)
    {
        spreadPoints();
        var cell = Plane.Cells[shit];
        if (cell.Contains(e.X, e.Y))
            label1.Text = "true" + e.X + " " + e.Y;
        else
            label1.Text = "false" + e.X + " " + e.Y;
        label1.Text += $"\n{cell.CellVertices[edgeLenth]}";
        label1.AutoSize = true;
    }
}
