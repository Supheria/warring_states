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
        //������Ϊtable1�����ݱ�
        sql.CreateTable(user.GetType());
        sql.InsertFieldValues(user);
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
