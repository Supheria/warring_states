using LocalUtilities.SimpleScript;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.SQLiteHelper.Data;
using LocalUtilities.TypeGeneral;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;

partial class Program
{
    private static SQLiteQuery Database { get; } = new();

    static void Main(string[] args)
    {
        Database.Connect("..\\mydb.db");

        var user = new User(DateTime.Now.ToBinary(), "Shit3", 3.1415926);
        var name = "name";
        var fields = TableTool.GetFieldsName<User>();
        //创建名为table1的数据表
        Database.CreateTable(name, fields);
        fields = TableTool.GetFieldsValue(user);
        Database.InsertFieldsValue(name, fields);
        var watch = new Stopwatch();
        watch.Start();
        var a = Database.SelectFieldsValue(name, [new("Id", 12345.00000001, Condition.Operates.LessOrEqual)], fields);
        watch.Stop();
        MessageBox.Show(watch.ElapsedMilliseconds.ToString());
        var assignments = new Field[] { new("Id", 12345), new("Name", "waht fuch"), new("UserB", new UserB() { WahtFcuh = 1022 }) };
        var conditions = new Condition[]
        {
            new("Name", user.Name, Condition.Operates.Equal),
            new("Id", user.Id, Condition.Operates.Equal),
        };
        Database.UpdateFieldsValues(name, new(Conditions.Combos.Or, conditions), assignments);
        conditions = new Condition[]
        {
            new("Name","waht fuch", Condition.Operates.Equal),
            new("Id", 12345, Condition.Operates.Equal),
        };
        //Database.DeleteFields(name, new(Conditions.Combos.And, conditions));
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
    class User(long id, string name, double age)
    {
        [TableField(IsPrimaryKey = true)]
        public long Id { get; private set; } = id;

        public string Name { get; private set; } = name;

        [TableField(Name = "fuck")]
        public double Age { get; private set; } = age;

        [TableIgnore]
        public string Email { get; private set; } = "xx@xx";

        public UserB UserB { get; set; } = new();

        public User(): this(0, "", 0)
        {

        }
    }

    class UserB
    {
        public FontData Font { get; private set; } = new();

        public UserC Fuck { get; set; } = new();

        public long WahtFcuh { get; set; } = 0;
    }

    class UserC
    {
        public FontData Font { get; set; } = new();
    }
}
