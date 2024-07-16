// See https://aka.ms/new-console-template for more information
using LocalUtilities.SimpleScript;
using LocalUtilities.TypeGeneral;
using System.Drawing;
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
        var str = a.Serialize(true, null);
        var st2 = 2.Serialize(true, null);
        var st3 = new Dictionary<string, List<B>>() { ["raht"] = [new(1024), new(20022)], ["SSS"] = [new()] }.Serialize(true, "shit");
        //SerializeTool.Deserialize(a.GetType(), str, null);
        var st4 = new List<Coordinate>() { new(1, 1), new(2, 1), new(2, 2) }.Serialize(true, "shit");
        var strt = str.Deserialize<A>(null);
        var dic = st3.Deserialize<Dictionary<string, List<B>>>("shit");
        var _4 = st4.Deserialize<List<Coordinate>>("shit");
        var cc = Color.Aquamarine.Serialize(true, null);
        var color = cc.Deserialize<Color>(null);
        Application.Run(new TestForm());
    }
}

public class TestForm : ResizeableForm
{
    public override string InitializeName => nameof(TestForm);
}