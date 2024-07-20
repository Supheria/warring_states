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
        //������Ϊtable1�����ݱ�
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
        //������������
        //sql.InsertValues(table, new string[] { "1", "����", "22", "Zhang@163.com" });
        //sql.InsertValues("table1", new string[] { "2", "����", "25", "Li4@163.com" });

        //�������ݣ���Name="����"�ļ�¼�е�Name��Ϊ"Zhang3"
        //sql.UpdateValues("table1", new("Name", "����", OperatorTypes.Equal), [new("Name", "Zhang3")]);

        ////ɾ��Name="����"��Age=26�ļ�¼,DeleteValuesOR��������
        //sql.DeleteValues("table1", [new("Name", "����", OperatorTypes.Equal), new("Age", 25.ToString(), OperatorTypes.Equal)], Condition.Combos.And);


        //��ȡ���ű�
        //SQLiteDataReader reader = sql.ReadFullTable("table1");
        //while (reader.Read())
        //{
        //    //��ȡID
        //    Log("" + reader.GetInt32(reader.GetOrdinal("ID")));
        //    //��ȡName
        //    Log("" + reader.GetString(reader.GetOrdinal("Name")));
        //    //��ȡAge
        //    Log("" + reader.GetInt32(reader.GetOrdinal("Age")));
        //    //��ȡEmail
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
