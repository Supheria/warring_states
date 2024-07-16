// See https://aka.ms/new-console-template for more information
using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SimpleScript.Data;
using LocalUtilities.SimpleScript.Parser;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;
using System.Collections;
using System.Drawing;
using System.Reflection;
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
    int Fuck { get; set; } = fuck;

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
            Map = new() { ["1"] = new(1) , ["shit"] = new(2)  },
            Bs = [new(), new()]
        };
        var str = a.Serialize(true, null);
        var st2 = 2.Serialize(true, null);
        var st3 = new Dictionary<string, List<B>>() { ["raht"] = [new(1024) , new(20022)], ["SSS"] = [new()] }.Serialize(true, "shit");
        //SerializeTool.Deserialize(a.GetType(), str, null);
        var st4 = new List<Coordinate>() { new(1, 1), new(2, 1), new(2, 2) }.Serialize(true, "shit");
        var strt = str.Deserialize<A>(null);
        var dic = st3.Deserialize<Dictionary<string, List<B>>>("shit");
        var _4 = st4.Deserialize<List<Coordinate>>("shit");
    }
}

partial class SerializeTool
{
    public static T? Deserialize<T>(this string str, string? name)
    {
        var tokenizer = new Tokenizer(str);
        var type = typeof(T);
        name ??= type.Name;
        if (!tokenizer.Elements.Property.TryGetValue(name, out var roots))
            return default;
        return (T?)Deserialize(type, roots.LastOrDefault());
    }

    public static object? Deserialize(Type type, Element? root)
    {
        if (root is null)
            return null;
        if (GetSimpleTypeConvert(type, out var convert))
            return convert(root.Value.Text);
        if (type == TPoint)
            return root.Value.Text.ToPoint();
        if (type == TRectangle)
            return root.Value.Text.ToRectangle();
        if (typeof(IArrayStringConvertable).IsAssignableFrom(type))
        {
            var obj = Activator.CreateInstance(type);
            if (obj is not IArrayStringConvertable iarr)
                return null;
            iarr.ParseArrayString(root.Value.Text);
            return iarr;
        }
        if (root is not ElementScope scope)
            return null;
        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            var openType = typeof(Dictionary<,>);
            var pairType = type.GetGenericArguments();
            var closeType = openType.MakeGenericType(pairType[0], pairType[1]);
            var obj = Activator.CreateInstance(closeType);
            if (!GetSimpleTypeConvert(pairType[0], out convert))
                return obj;
            foreach (var pair in scope.Property)
            {
                var key = convert(pair.Key);
                var value = Deserialize(pairType[1], pair.Value.FirstOrDefault());
                var add = type.GetMethod("Add");
                if (value is not null)
                    add?.Invoke(obj, [key, value]);
            }
            return obj;
        }
        if (typeof(ICollection).IsAssignableFrom(type))
        {
            var openType = typeof(List<>);
            var itemType = type.GetGenericArguments()[0];
            var closeType = openType.MakeGenericType(itemType);
            var obj = Activator.CreateInstance(closeType);
            var add = type.GetMethod("Add");
            if (GetSimpleTypeConvert(itemType, out convert))
            {
                var array = scope.Value.Text.ToArray();
                foreach (var item in array)
                    add?.Invoke(obj, [convert(item)]);
            }
            else if (scope.Property.TryGetValue("", out var items))
            {
                foreach (var item in items)
                {
                    var itemObj = Deserialize(itemType, item);
                    if (itemObj is not null)
                        add?.Invoke(obj, [itemObj]);
                }
            }
            return obj;
        }
        else
        {
            var obj = Activator.CreateInstance(type);
            foreach (var property in type.GetProperties(Authority))
            {
                if (property.GetCustomAttribute<SsIgnore>() is not null)
                    continue;
                var propertyName = property.GetCustomAttribute<SsItem>()?.Name ?? property.Name;
                if (!scope.Property.TryGetValue(propertyName, out var roots))
                    continue;
                var subObj = Deserialize(property.PropertyType, roots.LastOrDefault());
                if (subObj is not null)
                    property.SetValue(obj, subObj, Authority, null, null, null);
            }
            return obj;
        }
    }
}