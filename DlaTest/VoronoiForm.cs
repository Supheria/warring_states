using LocalUtilities;
using LocalUtilities.DijkstraShortestPath;
using LocalUtilities.GdiUtilities;
using LocalUtilities.VoronoiDiagram;
using LocalUtilities.VoronoiDiagram.Model;
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
    List<VoronoiCell> Cells = new();
    int shit = 0;
    int WidthSegmentNumber = 5;
    int HeightSegmentNumber = 5;

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

    void SpreadPoints()
    {
        var atlas = new Atlas(new(1000, 1000), new(WidthSegmentNumber, HeightSegmentNumber), new(5, 5), 0, new RandomPointsGenerationGaussian());
        if (Cells.Count is 0)
            Cells = atlas.GenerateVoronoi();
        var river = atlas.GenerateRiver();
        g.Clear(Color.White);
        DrawVoronoi();
        foreach (var p in river)
            g.DrawLine(new Pen(Color.Green, 2f), p.Starter, p.Ender);
        pb.Image = bitmap;
    }

    private void DrawVoronoi()
    {
        var cell = Cells[shit];
        g.FillEllipse(Brushes.Blue, (float)cell.Site.X - 1.5f, (float)cell.Site.Y - 1.5f, 3, 3);
        var centroid = cell.Centroid;
        g.FillEllipse(Brushes.Red, (float)centroid.X - 3f, (float)centroid.Y - 3f, 6, 6);
        g.FillEllipse(Brushes.Blue, (float)cell.Vertexes[edgeLenth].X - 3f, (float)cell.Vertexes[edgeLenth].Y - 3f, 6, 6);
        //var next = cell.VerticeNeighbor(cell.Vertices[edgeLenth], true);
        //g.FillEllipse(Brushes.Violet, (float)next.X - 3f, (float)next.Y - 3f, 6, 6);
        foreach (var n in cell.Neighbours)
        {
            centroid = n.Centroid;
            g.FillEllipse(Brushes.Red, (float)centroid.X - 3f, (float)centroid.Y - 3f, 6, 6);
        }
        //var a = 100;
        //var ps = new Pen[]
        //{
        //    new(Color.FromArgb(a, Color.Cyan)),
        //    new(Color.FromArgb(a, Color.Gray)),
        //    new(Color.FromArgb(a, Color.Pink)),
        //    new(Color.FromArgb(a, Color.Green))
        //};
        //int index = 0;
        foreach (var c in Cells)
            g.DrawPolygon(Pens.LightGray, c.Vertexes.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray());
        g.DrawPolygon(Pens.Black, cell.Vertexes.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray());
        label1.Text = cell.GetArea().ToString() + "\n";
        label1.Text += (cell.GetArea() / (1000 * 1000) * 100).ToString() + "%";
    }

    void Button1Click(object sender, EventArgs e)
    {
        //this.richTextBox1.Text += "\n******* NEW TEST *******";
        Cells.Clear();
        SpreadPoints();
        //background = Clone32BPPBitmap ( bitmap );
    }

    void NumericUpDown1ValueChanged(object sender, EventArgs e)
    {
        edgeLenth = (int)(numericUpDown1.Value);
        SpreadPoints();
        //background = Clone32BPPBitmap ( bitmap );
    }

    private void NumericUpDown2ValueChanged(object sender, EventArgs e)
    {
        shit = (int)(numericUpDown2.Value);
        SpreadPoints();
    }

    private void NumericUpDown3_ValueChanged(object sender, EventArgs e)
    {
        WidthSegmentNumber = (int)(numericUpDown3.Value);
        HeightSegmentNumber = (int)(numericUpDown4.Value);
        Cells.Clear();
        SpreadPoints();
    }

    void PbMouseMove(object sender, MouseEventArgs e)
    {
        if (Cells.Count is 0)
            SpreadPoints();
        var cell = Cells[shit];
        //if (cell.ContainPoint(e.X, e.Y))
        //    label1.Text = "true" + e.X + " " + e.Y;
        //else
        //    label1.Text = "false" + e.X + " " + e.Y;
        //label1.Text += $"\n{cell.Vertices[edgeLenth]}\n{cell}";
        //label1.AutoSize = true;
    }
}
