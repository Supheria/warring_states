using LocalUtilities.TypeGeneral;
using System.Data.SQLite;

class Program
{
    private static SqLiteHelper sql;

    static void Main(string[] args)
    {
        sql = new SqLiteHelper("mydb.db");


        //������Ϊtable1�����ݱ�
        sql.CreateTable("table1", new string[] { "ID", "Name", "Age", "Email" }, new string[] { "INTEGER", "TEXT", "INTEGER", "TEXT" });
        //������������
        sql.InsertValues("table1", new string[] { "1", "����", "22", "Zhang@163.com" });
        sql.InsertValues("table1", new string[] { "2", "����", "25", "Li4@163.com" });

        //�������ݣ���Name="����"�ļ�¼�е�Name��Ϊ"Zhang3"
        sql.UpdateValues("table1", new("Name", "����", OperatorTypes.Equal), [new("Name", "Zhang3")]);

        //ɾ��Name="����"��Age=26�ļ�¼,DeleteValuesOR��������
        //sql.DeleteValuesAND("table1", new string[] { "Name", "Age" }, new string[] { "����", "22" }, new string[] { "=", "=" });


        //��ȡ���ű�
        SQLiteDataReader reader = sql.ReadFullTable("table1");
        while (reader.Read())
        {
            //��ȡID
            Log("" + reader.GetInt32(reader.GetOrdinal("ID")));
            //��ȡName
            Log("" + reader.GetString(reader.GetOrdinal("Name")));
            //��ȡAge
            Log("" + reader.GetInt32(reader.GetOrdinal("Age")));
            //��ȡEmail
            Log(reader.GetString(reader.GetOrdinal("Email")));
        }

        while (true)
        {
            Console.ReadLine();
        }
    }

    static void Log(string s)
    {
        Console.WriteLine("" + s);
    }

    //public Program()
    //{
    //    createNewDatabase();
    //    connectToDatabase();
    //    createTable();
    //    fillTable();
    //    printHighscores();
    //}

    ////����һ���յ����ݿ�
    //void createNewDatabase()
    //{
    //    SQLiteConnection.CreateFile("MyDatabase.db");//���Բ�Ҫ�˾�
    //}

    ////����һ�����ӵ�ָ�����ݿ�
    //void connectToDatabase()
    //{
    //    m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");//û�����ݿ����Զ�����
    //    m_dbConnection.Open();
    //}

    ////��ָ�����ݿ��д���һ��table
    //void createTable()
    //{
    //    string sql = "create table  if not exists highscores (name varchar(20), score int)";
    //    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
    //    command.ExecuteNonQuery();
    //}

    ////����һЩ����
    //void fillTable()
    //{
    //    string sql = "insert into highscores (name, score) values ('Me', 3000)";
    //    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
    //    command.ExecuteNonQuery();

    //    sql = "insert into highscores (name, score) values ('Myself', 6000)";
    //    command = new SQLiteCommand(sql, m_dbConnection);
    //    command.ExecuteNonQuery();

    //    sql = "insert into highscores (name, score) values ('And I', 9001)";
    //    command = new SQLiteCommand(sql, m_dbConnection);
    //    command.ExecuteNonQuery();
    //}

    ////ʹ��sql��ѯ��䣬����ʾ���
    //void printHighscores()
    //{
    //    string sql = "select * from highscores order by score desc";
    //    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
    //    SQLiteDataReader reader = command.ExecuteReader();
    //    while (reader.Read())
    //        Console.WriteLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);
    //    Console.ReadLine();
    //}
}