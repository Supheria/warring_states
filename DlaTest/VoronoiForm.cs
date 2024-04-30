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
        foreach (var cell in Cells)
        {
            if (cellMap.TryGetValue(cell.Location, out var value))
                value.Add(cell);
            else
                cellMap[cell.Location] = [cell];
        }
        //var path = new List<VoronoiPoint>();
        var visited = new HashSet<VoronoiVertice>();
        var cellPath = new List<VoronoiCell>();
        var random = new Random();
        //var currentCell = cellMap[Direction.Left][random.Next(0, cellMap[Direction.Left].Count)];
        //visited.Add(currentCell.Centroid);
        //cellPath.Add(currentCell);
        //var nextPoint = currentCell.Vertices.First();
        ////foreach (var v in currentCell.Vertices)
        ////{
        ////    if (v.BorderLocation is Direction.Left)
        ////    {
        ////        if (nextPoint.BorderLocation is not Direction.Left || v.Y < nextPoint.Y)
        ////            nextPoint = v;
        ////    }
        ////}
        ////g.FillEllipse(Brushes.Green, (float)nextPoint.X - 3f, (float)nextPoint.Y - 3f, 6, 6);
        //g.FillEllipse(Brushes.DarkGreen, (float)currentCell.Centroid.X - 5f, (float)currentCell.Centroid.Y - 5f, 10, 10);
        ////path.Add(nextPoint);
        ////var closewise = true;
        ////var prevCell = currentCell;
        ////var leftVisited = new HashSet<VoronoiPoint>() { currentCell.Centroid };
        //do
        //{
        //    //nextPoint = currentCell.VerticeNeighbor(nextPoint, closewise);
        //    //path.Add(nextPoint);
        //    //if (nextPoint.BorderLocation == Direction.Top || nextPoint.BorderLocation == Direction.Bottom)
        //    //    currentCell = prevCell;
        //    //else
        //    //    path.Add(nextPoint);
        //    var tempCell = currentCell;
        //    var ved = new HashSet<VoronoiPoint>();
        //    bool fallBack = false;
        //    //bool finish = false;
        //    do
        //    {
        //        tempCell = currentCell.Neighbours[random.Next(0, currentCell.Neighbours.Count)];
        //        ved.Add(tempCell.Centroid);
        //        if (ved.Count == currentCell.Neighbours.Count)
        //        {
        //            if (cellPath.Count is 0)
        //            {
        //                //currentCell = cellMap[Direction.Left][random.Next(0, cellMap[Direction.Left].Count)];
        //                visited.Clear();
        //                cellPath.Add(currentCell);
        //                //leftVisited.Add(currentCell.Centroid);
        //                //if (leftVisited.Count == cellMap[Direction.Left].Count)
        //                //{
        //                //    finish = true;
        //                //    break;
        //                //}
        //            }
        //            else
        //            {
        //                currentCell = cellPath[cellPath.Count - 2 < 0 ? 0 : cellPath.Count - 2];
        //                cellPath.RemoveAt(cellPath.Count - 1);
        //            }
        //            fallBack = true;
        //            break;
        //        }
        //        //if (tempCell.Location is Direction.Top || tempCell.Location is Direction.Bottom)
        //        //{
        //        //    currentCell = cellPath[cellPath.Count - 2 < 0 ? 0 : cellPath.Count - 2];
        //        //    cellPath.RemoveAt(cellPath.Count - 1);
        //        //    continue;
        //        //}
        //        //ved.Add(tempCell.Centroid);
        //        //if (ved.Count == currentCell.Neighbours.Count)
        //        //    break;
        //    } while (visited.Contains(tempCell.Centroid) || tempCell.Location is Direction.Top || tempCell.Location is Direction.Bottom || tempCell.Location.HasFlag(Direction.Left));
        //    if (fallBack)
        //        continue;
        //    //if (finish)
        //    //    break;
        //    currentCell = tempCell;
        //    visited.Add(currentCell.Centroid);
        //    cellPath.Add(currentCell);
        //    //closewise = !closewise;
        //} while (!currentCell.Location.HasFlag(Direction.Right));
        //foreach(var v in path)
        //    g.FillEllipse(Brushes.DarkGreen, (float)v.X - 5f, (float)v.Y - 5f, 10, 10);
        foreach (var v in cellPath)
            g.DrawEllipse(Pens.DarkGreen, (float)v.Centroid.X - 5f, (float)v.Centroid.Y - 5f, 10, 10);
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
