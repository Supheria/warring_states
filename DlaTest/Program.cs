using LocalUtilities.FileUtilities;
using LocalUtilities.GdiUtilities;
using LocalUtilities.Serializations;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using LocalUtilities.VoronoiDiagram;
using LocalUtilities.VoronoiDiagram.Model;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace DlaTest;

public class Program
{
    public static void TestBelin()
    {
        var image = new Bitmap(500, 500);
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.Flush(); g.Dispose();

        var pImage = new PointBitmap(image);
        pImage.LockBits();
        for (double i = 0; i < image.Width; i++)
        {
            for (double j = 0; j < image.Height; j++)
            {
                var height = //new Perlin(255).OctavePerlin(i / 50, j / 50, 2, 5) * 0.5 +

                    new Perlin(55).OctavePerlin(i / 50, j / 50, 8, 2) * 0.5;
                pImage.SetPixel((int)i, (int)j, Color.FromArgb((int)(height * 255), Color.White));
            }
        }
        pImage.UnlockBits();

        image.Save("Perlin.bmp");
    }

    //public static DlaMap GetScaledBitmap(Bitmap source, int addWidth, int addHeight, int maxWalkerNumber)
    //{
    //    var scale = new Bitmap((int)(source.Width + addWidth), (int)(source.Height + addHeight));
    //    var g = Graphics.FromImage(scale);
    //    g.Clear(Color.Black);
    //    g.InterpolationMode = InterpolationMode.NearestNeighbor;
    //    g.DrawImage(source, 0, 0, scale.Width, scale.Height);
    //    g.Flush(); g.Dispose();
    //    scale.Save("_scale.bmp");

    //    var list = new List<(int X, int Y)>();
    //    var pScale = new PointBitmap(scale);
    //    pScale.LockBits();
    //    for (var i = 0; i < scale.Width; i++)
    //    {
    //        for (var j = 0; j < scale.Height; j++)
    //        {
    //            var color = pScale.GetPixel(i, j);
    //            if (color.ToArgb() == Color.White.ToArgb())
    //                list.Add((i, j));
    //        }
    //    }
    //    pScale.UnlockBits();
    //    var tree = new DlaMap(new(0, 0, scale.Width, scale.Height), [], maxWalkerNumber);
    //    //bigMap.Generate();
    //    return tree;
    //}

    //public static Bitmap GetDlaImage(DlaMap tree)
    //{
    //    //bigMap.ComputeHeight();

    //    var image = new Bitmap(tree.Bounds.Width, tree.Bounds.Height);
    //    var g = Graphics.FromImage(image);
    //    g.Clear(Color.Black);
    //    g.Flush(); g.Dispose();

    //    var pImage = new PointBitmap(image);
    //    pImage.LockBits();
    //    foreach (var point in tree.RosterList)
    //    {
    //        //var bright = pair.Value.Height() * 30;
    //        //var a = (2 + 1) - Math.Abs(1 - 2);
    //        //pImage.SetPixel(pair.Key.X, pair.Key.Y, (Color.FromArgb(((int)bright) > 255 ? 255 : (int)bright, Color.White)));
    //        pImage.SetPixel(point.X, point.Y, Color.White);
    //    }
    //    pImage.UnlockBits();
    //    image.Save("_dlg.bmp");
    //    return image;
    //}

    public static int GetHeightMax(DlaPixel[] pixels)
    {
        var heightMax = 0;
        foreach (var pixel in pixels) 
        {
            heightMax = Math.Max(heightMax, pixel.Height);
        }
        return heightMax;
    }

    public static void Main()
    {
        var atlas = new Atlas(1000, 1000, 5, 5, 350000);
        atlas.Generate();
        Bitmap image;
        try
        {
            image = new Bitmap(atlas.Width, atlas.Height + 200);
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
            return;
        }
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.FillRectangle(new SolidBrush(Color.LightYellow), atlas.Bounds);
        //g.DrawPolygon(Pens.Black, bigMap.PolygonRegion.Select(p => new PointF(p.X, p.Y)).ToArray());
        //image.Save("test.bmp");
        var pImage = new PointBitmap(image);
        pImage.LockBits();
        var forestRatio = 0.0835f; // 1/12
        var mountainRatio = 0.2505f; // 3/12
        // waterRatio                // 8/12
        double mountain = 0, water = 0, forest = 0;
        foreach (var pixels in atlas.BlockMap.Values)
        {
            var heightMax = GetHeightMax(pixels);
            foreach (var pixel in pixels)
            {
                float heightRatio = (float)pixel.Height / (float)heightMax;
                if (heightRatio <= forestRatio)
                {
                    pImage.SetPixel(pixel.X, pixel.Y, Color.ForestGreen);
                    forest++;
                }
                else if (heightRatio > forestRatio && heightRatio <= forestRatio + mountainRatio)
                {
                    pImage.SetPixel(pixel.X, pixel.Y, Color.Black);
                    mountain++;
                }
                else
                {
                    pImage.SetPixel(pixel.X, pixel.Y, Color.SkyBlue);
                    water++;
                }
            }
        }
        
        pImage.UnlockBits();
        var total = atlas.Width * atlas.Height;
        mountain = Math.Round(mountain / total * 100, 2);
        water = Math.Round(water / total * 100, 2);
        forest = Math.Round(forest / total * 100, 2);
        var plain = Math.Round(100 - (mountain + water + forest), 2);
        var totalCount = 0;
        foreach (var pixels in atlas.BlockMap.Values)
            totalCount += pixels.Length;
        g.DrawString($"\n\n\n生成数 {totalCount}\n\n范围 {atlas.Bounds}\n\n山地{mountain}% 平原{plain}%\n河水{water}% 树林{forest}%",
            new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), new RectangleF(0, image.Height - 200, image.Width, 200));
        //g.DrawPolygon(Pens.Black, bigMap.Region.CellVertices.Select(p=>new PointF((float)p.X, (float)p.Y)).ToArray());
        g.Flush(); g.Dispose();

        image.Save("_scale_gen.bmp");

        Console.WriteLine("OK");
    }
}

//public class TreeXmlSerialization(string localName) : RosterXmlSerialization<DlaMap, (int, int), DlaPixel>(new(), new WalkerXmlSerialization())
//{
//    public override string LocalName => localName;

//    protected override string RosterName => "Items";

//    public TreeXmlSerialization() : this(nameof(DlaMap))
//    {
//        OnRead += TreeXmlSerialization_OnRead;
//        OnWrite += TreeXmlSerialization_OnWrite;
//    }

//    private void TreeXmlSerialization_OnRead(XmlReader reader)
//    {
//        //Source.HeightMax = reader.GetAttribute(nameof(Source.HeightMax)).ToInt() ?? Source.HeightMax;
//        while (reader.Read())
//        {
//            if (reader.Name == nameof(Source.Bounds))
//            {
//                //Source.RectangleBoundary = new RectangleXmlSerialization(nameof(Source.Bounds)).Deserialize(reader);
//                break;
//            }
//        }
//    }

//    private void TreeXmlSerialization_OnWrite(XmlWriter writer)
//    {
//        writer.WriteAttributeString(nameof(Source.HeightMax), Source.HeightMax.ToString());
//        new RectangleXmlSerialization(nameof(Source.Bounds)) { Source = Source.Bounds }.Serialize(writer);
//    }
//}

//public class WalkerXmlSerialization() : XmlSerialization<DlaPixel>(new())
//{
//    public override string LocalName => nameof(DlaPixel);

//    public override void ReadXml(XmlReader reader)
//    {
//        var x = reader.GetAttribute(nameof(Source.X)).ToInt() ?? Source.X;
//        var y = reader.GetAttribute(nameof(Source.Y)).ToInt() ?? Source.Y;
//        Source.SetSignature = (x, y);
//        //Source.Height = reader.GetAttribute(nameof(Source.Height)).ToInt() ?? Source.Height;
//        //while (reader.Read())
//        //{
//        //    if (reader.Name == LocalName && reader.NodeType is XmlNodeType.EndElement)
//        //        break;
//        //    if (reader.NodeType is not XmlNodeType.Element)
//        //        continue;
//        //    if (reader.Name == nameof(Source.Neighbor))
//        //        Source.Neighbor = new WalkerNeighborXmlSerialization().ReadXmlCollection(reader, nameof(Source.Neighbor));
//        //}
//    }

//    public override void WriteXml(XmlWriter writer)
//    {
//        writer.WriteAttributeString(nameof(Source.X), Source.X.ToString());
//        writer.WriteAttributeString(nameof(Source.Y), Source.Y.ToString());
//        //writer.WriteAttributeString(nameof(Source.Height), Source.Height.ToString());
//        //new WalkerNeighborXmlSerialization().WriteXmlCollection(Source.Neighbor, writer, nameof(Source.Neighbor));
//    }
//}

//public class WalkerNeighborXmlSerialization : KeyValuePairXmlSerialization<Direction, (int X, int Y)>
//{
//    public override string LocalName => "Item";

//    protected override string KeyName => nameof(Direction);

//    protected override string ValueName => "Location";

//    protected override Func<string?, Direction> ReadKey => key => key.ToEnum<Direction>();

//    protected override Func<string?, (int X, int Y)> ReadValue => value => value.ToPair(0, 0, str => str.ToInt() ?? 0, str => str.ToInt() ?? 0);

//    protected override Func<Direction, string> WriteKey => key => key.ToString();

//    protected override Func<(int X, int Y), string> WriteValue => value => value.ToArrayString();
//}