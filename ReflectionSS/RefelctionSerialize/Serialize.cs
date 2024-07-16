using LocalUtilities.SimpleScript.Common;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral.Convert;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RefelctionSerialize;

public static partial class SerializeTool
{
    public static Type TByte { get; } = typeof(byte);
    public static Type TChar { get; } = typeof(char);
    public static Type TShort { get; } = typeof(short);
    public static Type TInt { get; } = typeof(int);
    public static Type TLong { get; } = typeof(long);
    public static Type TFloat { get; } = typeof(float);
    public static Type TDouble { get; } = typeof(double);
    public static Type TEnum { get; } = typeof(Enum);
    public static Type TString { get; } = typeof(string);
    public static Type TPoint { get; } = typeof(Point);
    public static Type TRectangle { get; } = typeof(Rectangle);


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
        if (type == TPoint)
        {
            writer.AppendValue(((Point)obj).ToArrayString());
            return;
        }
        if (type == TRectangle) 
        {
            writer.AppendValue(((Rectangle)obj).ToArrayString());
            return;
        }
        if (typeof(IArrayStringConvertable).IsAssignableFrom(type)) 
        {
            writer.AppendValue(((IArrayStringConvertable)obj).ToArrayString());
            return;
        }
        if (root)
            writer.AppendName(name ?? type.Name);
        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            var pairType = type.GetGenericArguments();
            if (!IsSimpleType(pairType[0]))
            {
                writer.AppendValue("");
                return;
            }
            writer.AppendStart();
            var enumer = ((IDictionary)obj).GetEnumerator();
            for (var i = 0; i < ((IDictionary)obj).Count; i++)
            {
                enumer.MoveNext();
                //writer.AppendArrayStart(null);
                if (enumer.Value is null)
                    continue;
                Serialize(enumer.Value, writer, enumer.Key.ToString(), true);
                //writer.AppendEnd();
            }
            writer.AppendEnd();
            return;
        }
        if (typeof(ICollection).IsAssignableFrom(type))
        {
            var itemType = type.GetGenericArguments()[0];
            if (IsSimpleType(itemType))
            {
                writer.AppendValue(((ICollection)obj).ToArrayString());
                return;
            }
            writer.AppendStart();
            var enumer = ((ICollection)obj).GetEnumerator();
            for (var i = 0; i < ((ICollection)obj).Count; i++)
            {
                enumer.MoveNext();
                //writer.AppendArrayStart(null);
                Serialize(enumer.Current, writer, null, false);
                //writer.AppendEnd();
            }
            writer.AppendEnd();
            return;
        }
        else
        {
            writer.AppendStart();
            foreach (var property in type.GetProperties(Authority))
            {
                if (property.GetCustomAttribute<SsIgnore>() is not null)
                    continue;
                var subObj = property.GetValue(obj, Authority, null, null, null);
                var propertyName = property.GetCustomAttribute<SsItem>()?.Name ?? property.Name;
                writer.AppendName(propertyName);
                Serialize(subObj, writer, propertyName, false);
            }
            writer.AppendEnd();
        }
    }
    
    static BindingFlags Authority { get; } = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;


    private static bool IsSimpleType(Type type)
    {
        return type == TByte ||
            type == TChar ||
            type == TShort ||
            type == TInt ||
            type == TLong ||
            type == TFloat ||
            type == TDouble ||
            type == TEnum ||
            type == TString;
    }

    private static bool GetSimpleTypeConvert(Type type, [NotNullWhen(true)] out Func<string, object?>? convert)
    {
        if (type == TByte)
            convert = str => str.ToByte();
        else if (type == TChar)
            convert = str => str.ToChar();
        else if (type == TShort)
            convert = str => str.ToShort();
        else if (type == TInt)
            convert = str => str.ToInt();
        else if (type == TLong)
            convert = str => str.ToLong();
        else if (type == TFloat)
            convert = str => str.ToFloat();
        else if (type == TDouble)
            convert = str => str.ToDouble();
        else if (type == TEnum)
            convert = str => str.ToEnum(type);
        else if (type == TString)
            convert = str => str;
        else
        {
            convert = null;
            return false;
        }
        return true;
    }

    private static bool IsSimpleTypeCollection(Type type)
    {
        return typeof(ICollection).IsAssignableFrom(type) && type.GenericTypeArguments.Length is 1 && IsSimpleType(type.GenericTypeArguments[0]);
    }
}
