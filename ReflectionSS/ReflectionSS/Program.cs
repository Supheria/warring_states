// See https://aka.ms/new-console-template for more information

using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SimpleScript.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

[SsRoot]
public class A
{
    public int Id { get; set; } = 0;

    public string Name { get; set; } = "test";

    [SsCollection]
    public List<int> Ints { get; set; } = [];

    [SsDictionary(typeof(string), typeof(string))]
    public Dictionary<string, B> Map { get; set; } = [];
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
            Map = new() { ["1"] = new() { shit = 1}, ["shit"] = new() { shit = 2 } }
        };
        var str = Serialize(a, true);
    }

    public static string Serialize(object obj, bool writeIntoMultiLines)
    {
        var writer = new SsWriter(writeIntoMultiLines);
        var type = obj.GetType();
        var root = type.GetCustomAttribute<SsRoot>();
        if (root is null)
            return "";
        var rootName = root.Name ?? type.Name;
        writer.AppendNameStart(rootName);
        Serialize(obj, writer);
        writer.AppendNameEnd();
        return writer.ToString();
    }

    public static void Serialize(object obj, SsWriter writer)
    {
        foreach (var property in obj.GetType().GetProperties())
        {
            if (property.GetCustomAttribute<SsIgnore>() is not null)
                continue;
            var subObj = property.GetValue(obj);
            if (property.GetCustomAttribute<SsRoot>() is SsRoot root)
            {
                var name = root.Name ?? property.Name;
                writer.AppendNameStart(name);
                if (subObj is not null)
                    Serialize(subObj, writer);
                writer.AppendNameEnd();
            }
            else if (property.GetCustomAttribute<SsCollection>() is SsCollection collection)
            {
                var name = collection.Name ?? property.Name;
                writer.AppendNameStart(name);
                var items = subObj as List<object>;
                if(items is not null)
                {
                    foreach (var item in items)
                    {
                        writer.AppendArrayStart(null);
                        Serialize(item, writer);
                        writer.AppendArrayEnd();
                    }
                }
                writer.AppendNameEnd();
            }
            else if (property.GetCustomAttribute<SsDictionary>() is SsDictionary dictionary)
            {
                var a = property.PropertyType.GetGenericTypeDefinition();
                var name = dictionary.Name ?? property.Name;
                int count = Convert.ToInt32(subObj.GetType().GetProperty("Count").GetValue(subObj, null));
                for (int i = 0; i < count; i++)
                {
                    object item = subObj.GetType().GetProperty("Keys");
                }
                var pairs = subObj as Dictionary<object, object>;
                if (pairs is not null)
                {
                    foreach (var (key, value) in pairs) 
                    {
                        writer.AppendArrayStart(key.ToString());
                        Serialize(value, writer);
                        writer.AppendArrayEnd();
                    }
                }
                writer.AppendNameEnd();
            }
            else
            {
                var name = property.GetCustomAttribute<SsItem>()?.Name ?? property.Name;
                writer.AppendTag(name, subObj?.ToString() ?? "");
            }
        }
    }
}