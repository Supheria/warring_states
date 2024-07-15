using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral.Convert;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RefelctionSerialize;

public static class SerializeTool
{
    public static string Serialize(this object obj, bool writeIntoMultiLines, string? name)
    {
        var writer = new SsWriter(writeIntoMultiLines);
        Serialize(obj, writer, name, true);
        return writer.ToString();
    }

    public static void Serialize(object? obj, SsWriter writer, string? name, bool root)
    {
        if (obj is null)
        {
            writer.AppendValue("");
            return;
        }
        var type = obj.GetType();
        if (IsSimpleType(type))
        {
            writer.AppendValue(obj.ToString() ?? "");
            return;
        }
        if (obj is Point point) 
        {
            writer.AppendValue(point.ToArrayString());
            return;
        }
        if (obj is Rectangle rect) 
        {
            writer.AppendValue(rect.ToArrayString());
            return;
        }
        if (obj is IArrayStringConvertable iarray) 
        {
            writer.AppendValue(iarray.ToArrayString());
            return;
        }
        if (root)
            writer.AppendName(name ?? type.Name);
        if (typeof(IDictionary).IsAssignableFrom(type))
        {

            writer.AppendStart();
            var enumer = ((IDictionary)obj).GetEnumerator();
            for (var i = 0; i < ((IDictionary)obj).Count; i++)
            {
                enumer.MoveNext();
                writer.AppendArrayStart(enumer.Key.ToString());
                if (enumer.Value is null)
                    continue;
                Serialize(enumer.Value, writer, null, true);
                writer.AppendEnd();
            }
            writer.AppendEnd();
            return;
        }
        if (typeof(ICollection).IsAssignableFrom(type))
        {
            if (IsSimpleTypeCollection(type))
            {
                writer.AppendValue(((ICollection)obj).ToArrayString());
                return;
            }
            writer.AppendStart();
            var enumer = ((ICollection)obj).GetEnumerator();
            for (var i = 0; i < ((ICollection)obj).Count; i++)
            {
                enumer.MoveNext();
                writer.AppendArrayStart(null);
                Serialize(enumer.Current, writer, null, true);
                writer.AppendEnd();
            }
            writer.AppendEnd();
            return;
        }
        writer.AppendStart();
        foreach (var property in obj.GetType().GetProperties())
        {
            if (property.GetCustomAttribute<SsIgnore>() is not null)
                continue;
            var subObj = property.GetValue(obj);
            var propertyName = property.GetCustomAttribute<SsItem>()?.Name ?? property.Name;
            writer.AppendName(propertyName);
            Serialize(subObj, writer, propertyName, false);
        }
        writer.AppendEnd();
    }

    private static bool IsSimpleType(Type type)
    {
        return type == typeof(byte) ||
            type == typeof(char) ||
            type == typeof(short) ||
            type == typeof(int) ||
            type == typeof(long) ||
            type == typeof(float) ||
            type == typeof(double) ||
            type == typeof(Enum) ||
            type == typeof(string);
    }

    private static bool IsSimpleTypeCollection(Type type)
    {
        return typeof(ICollection).IsAssignableFrom(type) && type.GenericTypeArguments.Length is 1 && IsSimpleType(type.GenericTypeArguments[0]);
    }
}
