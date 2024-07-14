using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using SqliteTest;
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
    public SQLiteDataReader? ExecuteQuery(QueryComposer queryComposer)
    {
        try
        {
            Command = Connection.CreateCommand();
            Command.CommandText = queryComposer.ToString();
            DataReader = Command.ExecuteReader();
            return DataReader;
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// 执行SQL命令
    /// </summary>
    /// <returns>The query.</returns>
    /// <param name="queryString">SQL命令字符串</param>
    public SQLiteDataReader ExecuteQuery(string queryComposer)
    {
        Command = Connection.CreateCommand();
        Command.CommandText = queryComposer.ToString();
        DataReader = Command.ExecuteReader();
        return DataReader;
    }

    /// <summary>
    /// 读取整张数据表
    /// </summary>
    /// <returns>The full table.</returns>
    /// <param name="tableName">数据表名称</param>
    public SQLiteDataReader? ReadFullTable(Volume tableName)
    {
        var query = new QueryComposer()
            .Append(Keywords.Select)
            .Append(Keywords.Any)
            .Append(Keywords.From)
            .Append(tableName)
            .Finish();
        return ExecuteQuery(query);
    }

    public SQLiteDataReader? CreateTable(Type type)
    {
        return CreateSubTable(type, null);
    }

    private SQLiteDataReader? CreateSubTable(Type type, string? tableName)
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
            fields.Add(new("sub-module stamp"));
        }
        foreach (var property in type.GetProperties())
        {
            if (property.GetCustomAttribute<TableFieldIgnore>() is not null)
                continue;
            var tableField = property.GetCustomAttribute<TableField>();
            if (tableField is null)
                CreateSubTable(property.PropertyType, tableName);
            var fieldName = tableField?.Name ?? property.Name;
            fields.Add(new(fieldName));
        }
        var query = new QueryComposer()
            .Append(Keywords.CreateTableNotExists)
            .Append(new(tableName))
            .AppendFields(fields.ToArray())
            .Finish();
        return ExecuteQuery(query);
    }

    private static string? SerializeObject(object? obj)
    {
        return obj switch
        {
            ISsSerializable iss => iss.ToSsString(),
            object o => o.ToString(),
            _ => null
        };
    }

    public SQLiteDataReader? InsertFieldValues(object obj)
    {
        InsertSubFieldValues(obj, null, null, out var reader);
        return reader;
    }

    private bool InsertSubFieldValues(object obj, string? tableName, Volume? stamp, out SQLiteDataReader? reader)
    {
        reader = null;
        var type = obj.GetType();
        var table = type.GetCustomAttribute<Table>();
        if (table is null)
            return false;
        if (tableName is null)
            tableName = table.Name ?? type.Name;
        else
            tableName = tableName + SignTable.Dot + (table.Name ?? type.Name);
        var fieldValues = new List<Volume>();
        if (stamp is not null)
            fieldValues.Add(stamp);
        stamp = new Volume(DateTime.Now.ToBinary().ToString());
        foreach (var property in type.GetProperties())
        {
            if (property.GetCustomAttribute<TableFieldIgnore>() is not null)
                continue;
            var subObj = property.GetValue(obj);
            if (subObj is null ||
                property.GetCustomAttribute<TableField>() is not null ||
                !InsertSubFieldValues(subObj, tableName, stamp, out _))
            {
                var value = SerializeObject(subObj);
                fieldValues.Add(new(value ?? ""));
            }
            else
                fieldValues.Add(stamp);
        }
        reader = InsertFieldValues(new(tableName), fieldValues.ToArray());
        return true;
    }

    private SQLiteDataReader? InsertFieldValues(Volume tableName, Volume[] fieldValues)
    {
        var query = new QueryComposer()
            .Append(Keywords.InsertInto)
            .Append(tableName)
            .AppendValues(fieldValues.ToArray())
            .Finish();
        return ExecuteQuery(query);
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
    public SQLiteDataReader ReadTable(string tableName, string[] items, string[] colNames, string[] operations, string[] colValues)
    {
        string queryString = "SELECT " + items[0];
        for (int i = 1; i < items.Length; i++)
        {
            queryString += ", " + items[i];
        }
        queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " + operations[0] + " " + colValues[0];
        for (int i = 0; i < colNames.Length; i++)
        {
            queryString += " AND " + colNames[i] + " " + operations[i] + " " + colValues[0] + " ";
        }
        return ExecuteQuery(queryString);
    }
}