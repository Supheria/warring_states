using AltitudeMapGenerator.Layout;
using AltitudeMapGenerator.VoronoiDiagram;
using AltitudeMapGenerator.VoronoiDiagram.Data;
using System.Drawing.Drawing2D;

namespace AltitudeMapGenerator.Test;

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
    Size SegmentNumber = new(5, 5);

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
        //var e = new Edge(new(100, 30), new(10, 150));
        //var points = EdgeTool.GetInnerPoints(e);
        //foreach (var p in points)
        //    g.FillEllipse(Brushes.Red, p.X - 1, p.Y - 1, 2, 2);
        var size = new Size(1000, 1000);
        if (Cells.Count is 0)
        {
            var plane = new VoronoiPlane(size);
            var sites = plane.GenerateSites(SegmentNumber);
            Cells = plane.Generate(sites);
        }
        var river = new RiverGenerator(2.5, size, new(5, 5), RiverLayout.Types.BackwardSlash, Cells.Select(c => c.Site).ToList());
        g.Clear(Color.White);
        foreach (var c in Cells)
            g.DrawPolygon(Pens.LightGray, c.Vertexes.Select(p => (PointF)p).ToArray());
        //DrawVoronoi();

        //var data = new AtlasData("testMap", new(200, 200), new(4, 4), new(4, 6), RiverLayout.Type.Vertical, 2.15, 17000, 0.66f);
        //var atlas = new Atlas(data);
        foreach (var p in river.River)
        {
            g.FillEllipse(Brushes.Red, p.X, p.Y, 1, 1);
        }
        //DrawVoronoi();
        pb.Image = bitmap;
    }

    private void DrawVoronoi()
    {
        var cell = Cells[shit % Cells.Count];
        g.DrawPolygon(Pens.Gray, cell.Vertexes.Select(p => (PointF)p).ToArray());
        g.FillEllipse(Brushes.Red, (float)cell.Site.X - 3f, (float)cell.Site.Y - 3f, 6, 6);
        var vertex = cell.Vertexes[edgeLenth % cell.Vertexes.Count];
        g.FillEllipse(Brushes.Blue, (float)vertex.X - 3f, (float)vertex.Y - 3f, 6, 6);
        var nextVertex = cell.VertexCounterClockwiseNext(vertex);
        g.DrawLine(new Pen(Color.Red, 2f), vertex, nextVertex);

        //g.DrawPolygon(Pens.Black, cell.Vertexes.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray());
        //label1.Text = cell.GetArea().ToString() + "\n";
        //label1.Text += (cell.GetArea() / (1000 * 1000) * 100).ToString() + "%";
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
        SegmentNumber = new((int)numericUpDown3.Value, (int)numericUpDown4.Value);
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
