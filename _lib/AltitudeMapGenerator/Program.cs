using AltitudeMapGenerator.Layout;
using AltitudeMapGenerator.Test;
using LocalUtilities.SimpleScript;
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.TypeGeneral;
using System.Diagnostics;

namespace AltitudeMapGenerator;

public class Program
{
    [STAThread]
    public static void Main()
    {
        //new AltiteudeForm().ShowDialog();
        //new VoronoiForm().ShowDialog();
        var data = new AltitudeMapData(new(1000, 1000), new(5, 5), new(7, 6), RiverLayout.Types.Vertical, 3, 550000, 0.66f);
        //var data = new AtlasData("testMap", new(500, 500), new(4, 4), new(5, 6), RiverLayout.Type.Horizontal, 120000, 0.66f, new RandomPointsGenerationGaussian());

        //var data = new AltitudeMapData(new(200, 300), new(2, 3), new(6, 3), RiverLayout.Types.BackwardSlash, 1, 30000, 0.5f);
        var atl = new AltitudeMap(data, null);
        var signTable = new SsSignTable();
        SerializeTool.SerializeFile(atl, new(), "test.ss", false, signTable);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var atlas = SerializeTool.DeserializeFile<AltitudeMap>(new(), "test.ss", signTable);
        stopwatch.Stop();
        var time = stopwatch.ElapsedMilliseconds;
        SerializeTool.SerializeFile(atlas, new(), "test1.ss", false, signTable);
        MessageBox.Show(time.ToString());
        Bitmap image;
        try
        {
            image = new Bitmap(atlas.Width, atlas.Height + 200);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return;
        }
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.FillRectangle(new SolidBrush(Color.LightYellow), atlas.Bounds);
        foreach (var r in atlas.RiverPoints)
        {
            g.FillEllipse(new SolidBrush(Color.Red), r.X, r.Y, 1.5f, 1.5f);
        }
        //g.DrawPolygon(Pens.Black, bigMap.PolygonRegion.Select(p => new PointF(p.X, p.Y)).ToArray());
        //image.Save("test.bmp");
        var pImage = new PointBitmap(image);
        pImage.LockBits();
        var forestRatio = 0.0835f; // 1/12
        var mountainRatio = 0.2505f; // 3/12
        // waterRatio                // 8/12
        double mountain = 0, water = 0, forest = 0;
        foreach (var point in atlas.AltitudePoints)
        {
            float heightRatio = (float)point.Altitude / (float)atlas.AltitudeMax;
            if (heightRatio <= forestRatio)
            {
                pImage.SetPixel(point.X, point.Y, Color.ForestGreen);
                forest++;
            }
            else if (heightRatio > forestRatio && heightRatio <= forestRatio + mountainRatio)
            {
                pImage.SetPixel(point.X, point.Y, Color.Black);
                mountain++;
            }
            else
            {
                pImage.SetPixel(point.X, point.Y, Color.SkyBlue);
                water++;
            }
        }
        pImage.UnlockBits();
        var total = atlas.Width * atlas.Height;
        mountain = Math.Round(mountain / total * 100, 2);
        water = Math.Round(water / total * 100, 2);
        forest = Math.Round(forest / total * 100, 2);
        var plain = Math.Round(100 - (mountain + water + forest), 2);
        var totalCount = atlas.AltitudePoints.Count;
        g.DrawString($"\n\n\n生成数 {totalCount}\n\n范围 {atlas.Bounds}\n\n山地{mountain}% 平原{plain}%\n河水{water}% 树林{forest}%",
            new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), new RectangleF(0, image.Height - 200, image.Width, 200));
        //g.DrawPolygon(Pens.Black, bigMap.Region.CellVertices.Select(p=>new PointF((float)p.X, (float)p.Y)).ToArray());
        g.Flush(); g.Dispose();

        image.Save("_scale_gen.bmp");

        Console.WriteLine("OK");
    }
}