using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System.Data.SQLite;
using System.Reflection;

partial class Program
{
    private static DatabaseOperation Database;

    static void Main(string[] args)
    {
        Database = new(new("..\\mydb.db"));

        var user = new User(12345, "hello", 3.1415926);
        //创建名为table1的数据表
        Database.CreateTable(user.GetType());
        Database.InsertFields(user);
        var a = Database.ReadFullTable(user.GetType());
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
    class User(int id, string name, double age)
    {
        [TableField(Name = "UID")]
        public int Id { get; private set; } = id;

        public string Name { get; private set; } = name;

        public double Age { get; private set; } = age;

        [TableFieldIgnore]
        public string Email { get; private set; } = "xx@xx";

        public UserB UserB { get; set; } = new();

        public User(): this(0, "", 0)
        {

        }
    }

    [Table]
    class UserB
    {
        [TableField(Name = "shit")]
        public FontData Font { get; private set; } = new();

        [TableField(Name = "UserC")]
        public UserC Fuck { get; set; } = new();
    }

    //[Table]
    class UserC
    {
        //[Table(Name = "shit")]
        public string Font { get; set; } = "";
    }
}
