﻿using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using SqliteTest;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

/// <summary>
/// SQLite 操作类
/// </summary>
class SqLiteHelper
{
    public static Volume Version = new("3");

    static string SubModulStampField = "sub-module stamp";
    /// <summary>
    /// 数据库连接定义
    /// </summary>
    SQLiteConnection Connection { get; }

    /// <summary>
    /// SQL命令定义
    /// </summary>
    SQLiteCommand? Command { get; set; }

    /// <summary>
    /// 数据读取定义
    /// </summary>
    SQLiteDataReader? DataReader { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="connectionString">连接SQLite库字符串</param>
    public SqLiteHelper(Volume filePath)
    {
        var query = new QueryComposer()
            .Append(Keywords.DataSource)
            .Append(Keywords.Equal)
            .Append(filePath)
            .Finish()
            .Append(Keywords.Version)
            .Append(Keywords.Equal)
            .Append(Version)
            .Finish()
            .ToString();
        Connection = new(query);
        Connection.Open();
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public void Close()
    {
        Command?.Cancel();
        Command = null;
        DataReader?.Close();
        DataReader = null;
        Connection.Close();

    }

    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    public SQLiteDataReader ExecuteQuery(QueryComposer queryComposer)
    {
        Command = Connection.CreateCommand();
        Command.CommandText = queryComposer.ToString();
        DataReader = Command.ExecuteReader();
        return DataReader;
    }

    public object[] ReadFullTable(Type type)
    {
        return ReadTable(type, null, null);
    }

    /// <summary>
    /// 读取整张数据表
    /// </summary>
    /// <returns>The full table.</returns>
    /// <param name="tableName">数据表名称</param>
    private object[] ReadTable(Type type, string? tableName, string? stamp)
    {
        var objects = new List<object>();
        var table = type.GetCustomAttribute<Table>();
        if (table is null)
            return objects.ToArray();
        if (tableName is null)
            tableName = table.Name ?? type.Name;
        else
            tableName = tableName + SignTable.Dot + (table.Name ?? type.Name);
        var query = new QueryComposer()
            .Append(Keywords.Select)
            .Append(Keywords.Any)
            .Append(Keywords.From)
            .Append(new(tableName));
        if (stamp is not null)
            query.AppendCondition(new(new(SubModulStampField), new(stamp), Condition.Operates.Equal));
        query.Finish();
        var reader = ExecuteQuery(query);
        while(reader.Read())
        {
            var obj = Assembly.GetExecutingAssembly().CreateInstance(type.FullName ?? "");
            if (obj is null)
                continue;
            foreach (var property in type.GetProperties())
            {
                if (property.GetCustomAttribute<TableFieldIgnore>() is not null)
                    continue;
                var subTable = property.PropertyType.GetCustomAttribute<Table>();
                if (subTable is not null)
                {
                    stamp = reader.GetInt64(reader.GetOrdinal(subTable.Name ?? property.Name)).ToString();
                    var subObj = ReadTable(property.PropertyType, tableName, stamp);
                    if (subObj is not null)
                        property.SetValue(obj, subObj[0]);
                    continue;
                }
                var ordinal = reader.GetOrdinal(property.GetCustomAttribute<TableField>()?.Name ?? property.Name);
                property.SetValue(obj, FromSqlType(property.PropertyType, reader, ordinal));
            }
            objects.Add(obj);
        }
        return objects.ToArray();
    }

    private object? FromSqlType(Type type, SQLiteDataReader reader, int ordinal)
    {
        if (type == typeof(short))
            return reader.GetInt16(ordinal);
        else if (type == typeof(int))
            return reader.GetInt32(ordinal);
        else if (type == typeof(long))
            return reader.GetInt64(ordinal);
        else if (type == typeof(float))
            return reader.GetFloat(ordinal);
        else if (type == typeof(double))
            return reader.GetDouble(ordinal);
        else if (type == typeof(ISsSerializable))
        {
            var obj = Assembly.GetExecutingAssembly().CreateInstance(type.FullName ?? "");
            (obj as ISsSerializable)?.ParseSs(reader.GetString(ordinal));
            return obj;
        }
        else
            return reader.GetString(ordinal);
    }

    public SQLiteDataReader? CreateTable(Type type)
    {
        return CreateTable(type, null);
    }

    private SQLiteDataReader? CreateTable(Type type, string? tableName)
    {
        var table = type.GetCustomAttribute<Table>();
        if (table is null)
            return null;
        var fields = new List<Field>();
        if (tableName is null)
            tableName = table.Name ?? type.Name;
        else
        {
            tableName = tableName + SignTable.Dot + (table.Name ?? type.Name);
            fields.Add(new(SubModulStampField, Keywords.Integer));
        }
        foreach (var property in type.GetProperties())
        {
            if (property.GetCustomAttribute<TableFieldIgnore>() is not null)
                continue;
            var subTable = property.PropertyType.GetCustomAttribute<Table>();
            if (subTable is not null)
            {
                CreateTable(property.PropertyType, tableName);
                fields.Add(new(subTable.Name ?? property.Name, Keywords.Integer));
                continue;
            }
            var tableField = property.GetCustomAttribute<TableField>();
            fields.Add(new(tableField?.Name ?? property.Name, ToSqlType(property.PropertyType)));
        }
        var query = new QueryComposer()
            .Append(Keywords.CreateTableNotExists)
            .Append(new(tableName))
            .AppendFields(fields.ToArray())
            .Finish();
        return ExecuteQuery(query);
    }

    private Keywords ToSqlType(Type type)
    {
        if (type == typeof(short) ||
            type == typeof(int) ||
            type == typeof(long))
            return Keywords.Integer;
        if (type == typeof(float) ||
            type == typeof(double))
            return Keywords.Real;
        return Keywords.Text;
    }

    public SQLiteDataReader? InsertFields(object obj)
    {
        return InsertFields(obj, null, null);
    }

    private SQLiteDataReader? InsertFields(object? obj, string? tableName, Volume? stamp)
    {
        if (obj is null)
            return null;
        var type = obj.GetType();
        var table = type.GetCustomAttribute<Table>();
        if (table is null)
            return null;
        if (tableName is null)
            tableName = table.Name ?? type.Name;
        else
            tableName = tableName + SignTable.Dot + (table.Name ?? type.Name);
        var fieldValues = new List<Volume>();
        if (stamp is not null)
            fieldValues.Add(stamp);
        stamp = GetStamp();
        foreach (var property in type.GetProperties())
        {
            if (property.GetCustomAttribute<TableFieldIgnore>() is not null)
                continue;
            var subObj = property.GetValue(obj);
            if (property.PropertyType.GetCustomAttribute<Table>() is not null)
            {
                InsertFields(subObj, tableName, stamp);
                fieldValues.Add(stamp);
                continue;
            }
            fieldValues.Add(new(GetSqlString(subObj)));
        }
        var query = new QueryComposer()
             .Append(Keywords.InsertInto)
             .Append(new(tableName))
             .AppendValues(fieldValues.ToArray())
             .Finish();
        return ExecuteQuery(query);
    }

    private static Volume GetStamp()
    {
        return new(DateTime.Now.ToBinary().ToString());
    }

    private static string GetSqlString(object? obj)
    {
        return obj switch
        {
            ISsSerializable iss => iss.ToSsString(),
            _ => obj?.ToString() ?? ""
        };
    }

    /// <summary>
    /// 更新指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    /// <param name="key">关键字</param>
    /// <param name="value">关键字对应的值</param>
    /// <param name="operation">运算符：=,<,>,...，默认“=”</param>
    public SQLiteDataReader UpdateValues(Volume tableName, Condition condition, params Assignment[] updateFields)
    {
        var query = new QueryComposer()
            .Append(Keywords.Update)
            .Append(tableName)
            .Append(Keywords.Set)
            .AppendColumnFields(updateFields)
            .AppendCondition(condition)
            .Finish();
        return ExecuteQuery(query);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public SQLiteDataReader DeleteValues(Volume tableName, Condition[] conditions, Condition.Combos combo)
    {
        var query = new QueryComposer()
            .Append(Keywords.Delete)
            .Append(Keywords.From)
            .Append(tableName)
            .AppendConditions(conditions, combo)
            .Finish();
        return ExecuteQuery(query);
    }

    /// <summary>
    /// Reads the table.
    /// </summary>
    /// <returns>The table.</returns>
    /// <param name="tableName">Table name.</param>
    /// <param name="items">Items.</param>
    /// <param name="colNames">Col names.</param>
    /// <param name="operations">Operations.</param>
    /// <param name="colValues">Col values.</param>
    //public SQLiteDataReader ReadTable(string tableName, string[] items, string[] colNames, string[] operations, string[] colValues)
    //{
    //    string queryString = "SELECT " + items[0];
    //    for (int i = 1; i < items.Length; i++)
    //    {
    //        queryString += ", " + items[i];
    //    }
    //    queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " + operations[0] + " " + colValues[0];
    //    for (int i = 0; i < colNames.Length; i++)
    //    {
    //        queryString += " AND " + colNames[i] + " " + operations[i] + " " + colValues[0] + " ";
    //    }
    //    return ExecuteQuery(queryString);
    //}
}