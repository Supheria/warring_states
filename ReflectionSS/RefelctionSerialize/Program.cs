// See https://aka.ms/new-console-template for more information
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SimpleScript.Parser;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;
using RefelctionSerialize;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Formats.Asn1;
using System.Reflection;
using System.Xml.Linq;

public class A
{
    public int Id { get; set; } = 0;

    public string Name { get; set; } = "test";

    public List<int> Ints { get; set; } = [];

    public Dictionary<string, B> Map { get; set; } = [];

    [SsItem(Name = "waht fuck")]
    public List<B> Bs { get; set; } = [];
}

public class B
{
    public int shit { get; set; } = 0;
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
            Map = new() { ["1"] = new() { shit = 1 }, ["shit"] = new() { shit = 2 } },
            Bs = [new(), new()]
        };
        var str = a.Serialize(true, null);
        var st2 = 2.Serialize(true, null);
        var st3 = new Dictionary<string, List<B>>() { ["raht"] = [new()] }.Serialize(true, "shit");
        var tokenizer = new Tokenizer(str);
    }
}
