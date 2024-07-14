using LocalUtilities.TypeGeneral;
using SqliteTest;
using System.Data.SQLite;
using System.Reflection;

partial class Program
{
    private static SqLiteHelper sql;

    static void Main(string[] args)
    {
        sql = new SqLiteHelper(new("..\\mydb.db"));

        var user = new User();
        //创建名为table1的数据表
        sql.CreateTable(user.GetType());
        sql.InsertFieldValues(user);
        //插入两条数据
        //sql.InsertValues(table, new string[] { "1", "张三", "22", "Zhang@163.com" });
        //sql.InsertValues("table1", new string[] { "2", "李四", "25", "Li4@163.com" });

        //更新数据，将Name="张三"的记录中的Name改为"Zhang3"
        //sql.UpdateValues("table1", new("Name", "张三", OperatorTypes.Equal), [new("Name", "Zhang3")]);

        ////删除Name="张三"且Age=26的记录,DeleteValuesOR方法类似
        //sql.DeleteValues("table1", [new("Name", "李四", OperatorTypes.Equal), new("Age", 25.ToString(), OperatorTypes.Equal)], Condition.Combos.And);


        //读取整张表
        //SQLiteDataReader reader = sql.ReadFullTable("table1");
        //while (reader.Read())
        //{
        //    //读取ID
        //    Log("" + reader.GetInt32(reader.GetOrdinal("ID")));
        //    //读取Name
        //    Log("" + reader.GetString(reader.GetOrdinal("Name")));
        //    //读取Age
        //    Log("" + reader.GetInt32(reader.GetOrdinal("Age")));
        //    //读取Email
        //    Log(reader.GetString(reader.GetOrdinal("Email")));
        //}
    }

    [Table]
    class User
    {
        [TableField(Name = "UID")]
        public int Id { get; } = 0;

        public string Name { get; } = "test";

        public int Age { get; } = 10;

        [TableFieldIgnore]
        public string Email { get; } = "xx@xx";

        //[TableField]
        public UserB UserB { get; } = new();
    }

    [Table]
    class UserB
    {
        //[Table(Name = "shit")]
        public UserC Font { get; } = new();
    }

    [Table]
    class UserC
    {
        //[Table(Name = "shit")]
        public FontData Font { get; } = new();
    }
}
