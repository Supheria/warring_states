using LocalUtilities.TypeGeneral;
using SqliteTest;
using System.Data.SQLite;
using System.Windows.Forms;

/// <summary>
/// SQLite 操作类
/// </summary>
class SqLiteHelper
{
    public const int Version = 3;
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
    public SqLiteHelper(string filePath)
    {
        var query = new QueryComposer()
            .Append(StringTable.Data)
            .Append(StringTable.Source)
            .Append(SignTable.Equal)
            .Append(filePath)
            .Finish()
            .Append(StringTable.Version)
            .Append(SignTable.Equal)
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
    public SQLiteDataReader ReadFullTable(string tableName)
    {
        var query = new QueryComposer()
            .Append(StringTable.Select)
            .Append(SignTable.Asterisk)
            .Append(StringTable.From)
            .Append(tableName)
            .Finish();
        return ExecuteQuery(query);
    }


    /// <summary>
    /// 向指定数据表中插入数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="values">插入的数值</param>
    public SQLiteDataReader InsertValues(string tableName, params string[] values)
    {
        ReadFullTable(tableName).ThrowIfFieldCountNotMatch(values);
        var query = new QueryComposer()
            .Append(StringTable.InsertInto)
            .Append(tableName)
            .Append(StringTable.Values)
            .AppendValues(values)
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
    public SQLiteDataReader UpdateValues(string tableName, Condition condition, params ColumnField[] updateFields)
    {
        var query = new QueryComposer()
            .Append(StringTable.Update)
            .Append(tableName)
            .Append(StringTable.Set)
            .AppendColumnFields(updateFields)
            .Append(StringTable.Where)
            .Append(condition.Key)
            .Append(condition.Operate.ToChar())
            .AppendValue(condition.Value)
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
    public SQLiteDataReader DeleteValuesOR(string tableName, string[] colNames, string[] colValues, string[] operations)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
        {
            throw new SQLiteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + "'" + colValues[0] + "'";
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += "OR " + colNames[i] + operations[0] + "'" + colValues[i] + "'";
        }
        return ExecuteQuery(queryString);
    }

    /// <summary>
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名对应的数据</param>
    public SQLiteDataReader DeleteValuesAND(string tableName, string[] colNames, string[] colValues, string[] operations)
    {
        //当字段名称和字段数值不对应时引发异常
        if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
        {
            throw new SQLiteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + "'" + colValues[0] + "'";
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += " AND " + colNames[i] + operations[i] + "'" + colValues[i] + "'";
        }
        return ExecuteQuery(queryString);
    }


    /// <summary>
    /// 创建数据表
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">数据表名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
    public SQLiteDataReader CreateTable(string tableName, string[] colNames, string[] colTypes)
    {
        string queryString = "CREATE TABLE IF NOT EXISTS " + tableName + "( " + colNames[0] + " " + colTypes[0];
        for (int i = 1; i < colNames.Length; i++)
        {
            queryString += ", " + colNames[i] + " " + colTypes[i];
        }
        queryString += "  ) ";
        return ExecuteQuery(queryString);
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

    /// <summary>
    /// 本类log
    /// </summary>
    /// <param name="s"></param>
    static void Log(string s)
    {
        Console.WriteLine("class SqLiteHelper:::" + s);
    }
}