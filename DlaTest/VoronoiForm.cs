using LocalUtilities;
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
        if (Cells.Count is 0)
            Cells = new Atlas(1000, 1000, WidthSegmentNumber, HeightSegmentNumber, 0).GenerateVoronoi(new RandomPointsGenerationGaussian());
        g.Clear(Color.White);
        DrawVoronoi();
        var cellMap = new Dictionary<(int, int), List<VoronoiCell>>();
        foreach (var c in Cells)
        {
            if (cellMap.TryGetValue(c.Location, out var value))
                value.Add(c);
            else
                cellMap[c.Location] = [c];
        }
        HashSet<Node> nodes = [];
        HashSet<Edge> edges = [];
        foreach(var c in Cells)
        {
            foreach(var v in c.Vertices)
            {
                var n = new Node(v.X, v.Y);
                if (!nodes.Contains(n))
                {
                    nodes.Add(new(v.X, v.Y));
                }
                var nV = c.VerticeNeighbor(v, true);
                var nextNode = new Node(nV.X, nV.Y);
                var edge = new Edge(n, nextNode);
                if (!edges.Contains(edge))
                {
                    edges.Add(edge);
                }
            }
        }
        DijkstraRouter.Initialize(edges, nodes);
        var random = new Random();
        var location = (0, random.Next(0, HeightSegmentNumber));
        var cell = cellMap[location].First();
        VoronoiVertex vertice = cell.Vertices.First();
        foreach(var v in cell.Vertices)
        {
            if(v.DirectionOnBorder is Direction.Left)
            {
                if (vertice.DirectionOnBorder is not Direction.Left || v.Y < vertice.Y)
                    vertice = v;
            }
        }
        var startNode = new Node(vertice.X, vertice.Y);
        location = new(WidthSegmentNumber - 1, random.Next(0, HeightSegmentNumber));
        cell = cellMap[location].First();
        vertice = cell.Vertices.First();
        foreach (var v in cell.Vertices)
        {
            if (v.DirectionOnBorder is Direction.Right)
            {
                if (vertice.DirectionOnBorder is not Direction.Right || v.Y < vertice.Y)
                    vertice = v;
            }
        }
        var endNode = new Node(vertice.X, vertice.Y);
        var route = DijkstraRouter.GetRoute(startNode.Id, endNode.Id);

        foreach (var r in route.Edges)
            g.DrawLine(new Pen(Color.Green, 2f), new PointF((float)r.StartNode.X, (float)r.StartNode.Y), new((float)r.EndNode.X, (float)r.EndNode.Y));
        pb.Image = bitmap;
    }

    private void DrawVoronoi()
    {
        var cell = Cells[shit];
        g.FillEllipse(Brushes.Blue, (float)cell.Site.X - 1.5f, (float)cell.Site.Y - 1.5f, 3, 3);
        var centroid = cell.Centroid;
        g.FillEllipse(Brushes.Red, (float)centroid.X - 3f, (float)centroid.Y - 3f, 6, 6);
        g.FillEllipse(Brushes.Blue, (float)cell.Vertices[edgeLenth].X - 3f, (float)cell.Vertices[edgeLenth].Y - 3f, 6, 6);
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
            g.DrawPolygon(Pens.LightGray, c.Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray());
        g.DrawPolygon(Pens.Black, cell.Vertices.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray());
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
        if (cell.ContainPoint(e.X, e.Y))
            label1.Text = "true" + e.X + " " + e.Y;
        else
            label1.Text = "false" + e.X + " " + e.Y;
        label1.Text += $"\n{cell.Vertices[edgeLenth]}\n{cell}";
        label1.AutoSize = true;
    }
}
