// See https://aka.ms/new-console-template for more information
using LocalUtilities.SimpleScript;
using LocalUtilities.TypeGeneral;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RefelctionSerialize;

public class A
{
    public int Id { get; set; } = 0;

    public string Name { get; set; } = "test";

    public List<int> Ints { get; set; } = [];

    public Dictionary<string, B> Map { get; set; } = [];

    [SsItem(Name = "waht fuck")]
    public List<B> Bs { get; set; } = [];
}

public class B(int fuck)
{
    public int Fuck { get; private set; } = fuck;

    public int GetFuck => Fuck;

    public B() : this(0)
    {

    }
}

public class Program
{
    public static void Main()
    {
        var a = new A()
        {
            Id = 1020324,
            Name = "shitss",
            Ints = [0, 1, 2, 3],
            Map = new() { ["1"] = new(1), ["shit"] = new(2) },
            Bs = [new(), new()]
        };
        var buff = a.Serialize(null);
        var str = Encoding.UTF8.GetString(buff);
        var st2 = 2.Serialize(null);
        var buff3 = new Dictionary<string, List<B>>() { ["raht"] = [new(1024), new(20022)], ["SSS"] = [new()] }.Serialize("shit");
        var st3 = Encoding.UTF8.GetString(buff3);
        //SerializeTool.Deserialize(a.GetType(), str, null);
        var buff4 = new List<Coordinate>() { new(1, 1), new(2, 1), new(2, 2) }.Serialize("shit");
        var st4 = Encoding.UTF8.GetString(buff4);
        var strt = SerializeTool.Deserialize<A>(buff, 0, buff.Length, null);
        var dic = SerializeTool.Deserialize<Dictionary<string, List<B>>>(buff3, 0, buff3.Length, "shit");
        var _4 = SerializeTool.Deserialize<List<Coordinate>>(buff4, 0, buff4.Length, "shit");
        var cc = Color.Aquamarine.Serialize(null);
        var ccStr = Encoding.UTF8.GetString(cc);
        var color = SerializeTool.Deserialize<Color>(cc, 0, cc.Length, null);
        Application.Run(new TestForm());
    }
}

public class TestForm : ResizeableForm
{
    public override string InitializeName => nameof(TestForm);
}