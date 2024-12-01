// See https://aka.ms/new-console-template for more information
using LocalUtilities.SimpleScript;

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
            Name = "你好世界",
            Ints = [0, 1, 2, 3],
            Map = new() { ["1"] = new(1), ["shit"] = new(2) },
            Bs = [new(), new()]
        };
        var signTable = new SsSignTable();
        //var str1 = SerializeTool.Serialize(a, new(), signTable, false);
        ////var buff1 = SerializeTool.Serialize(a, new(), signTable, null);
        ////var __1 = SerializeTool.Deserialize<A>(new(), buff1, 0, buff1.Length, signTable, null);
        //var str2 = SerializeTool.Serialize(2, new(), signTable, false);
        //var str3 = SerializeTool.Serialize(new Dictionary<string, List<B>>() { ["raht"] = [new(1024), new(20022)], ["SSS"] = [new()] }, new(), signTable, true);
        //var str4 = SerializeTool.Serialize(new List<Coordinate>() { new(1, 1), new(2, 1), new(2, 2) }, new("shit"), signTable, true);
        //var _1 = SerializeTool.Deserialize<A>(new(), str1, signTable);
        //var _2 = SerializeTool.Deserialize<int>(new(), str2, signTable);
        //var _3 = SerializeTool.Deserialize<Dictionary<string, List<B>>>(new(), str3, signTable);
        //var _4 = SerializeTool.Deserialize<List<Coordinate>>(new("shit"), str4, signTable);
        //var cc = SerializeTool.Serialize(Color.Aquamarine, new(), signTable, false);
        //var color = SerializeTool.Deserialize<Color>(new(), cc, signTable);
        var str5 = SerializeTool.Serialize(new Coordinate(101, 202), new(), signTable, false);
        var site = SerializeTool.Deserialize<Coordinate>(new(), str5, signTable);
        Application.Run(new TestForm());
    }
}

public class TestForm : ResizeableForm
{
    public override string InitializeName => nameof(TestForm);
}