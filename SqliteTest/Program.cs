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
        //������Ϊtable1�����ݱ�
        Database.CreateTable(user.GetType());
        Database.InsertFields(user);
        var a = Database.ReadFullTable(user.GetType());
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
