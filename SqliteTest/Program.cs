using LocalUtilities.TypeGeneral;
using System.Data.SQLite;

class Program
{
    private static SqLiteHelper sql;

    static void Main(string[] args)
    {
        sql = new SqLiteHelper("mydb.db");


        //创建名为table1的数据表
        sql.CreateTable("table1", new string[] { "ID", "Name", "Age", "Email" }, new string[] { "INTEGER", "TEXT", "INTEGER", "TEXT" });
        //插入两条数据
        sql.InsertValues("table1", new string[] { "1", "张三", "22", "Zhang@163.com" });
        sql.InsertValues("table1", new string[] { "2", "李四", "25", "Li4@163.com" });

        //更新数据，将Name="张三"的记录中的Name改为"Zhang3"
        sql.UpdateValues("table1", new("Name", "张三", OperatorTypes.Equal), [new("Name", "Zhang3")]);

        //删除Name="张三"且Age=26的记录,DeleteValuesOR方法类似
        //sql.DeleteValuesAND("table1", new string[] { "Name", "Age" }, new string[] { "张三", "22" }, new string[] { "=", "=" });


        //读取整张表
        SQLiteDataReader reader = sql.ReadFullTable("table1");
        while (reader.Read())
        {
            //读取ID
            Log("" + reader.GetInt32(reader.GetOrdinal("ID")));
            //读取Name
            Log("" + reader.GetString(reader.GetOrdinal("Name")));
            //读取Age
            Log("" + reader.GetInt32(reader.GetOrdinal("Age")));
            //读取Email
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

    ////创建一个空的数据库
    //void createNewDatabase()
    //{
    //    SQLiteConnection.CreateFile("MyDatabase.db");//可以不要此句
    //}

    ////创建一个连接到指定数据库
    //void connectToDatabase()
    //{
    //    m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");//没有数据库则自动创建
    //    m_dbConnection.Open();
    //}

    ////在指定数据库中创建一个table
    //void createTable()
    //{
    //    string sql = "create table  if not exists highscores (name varchar(20), score int)";
    //    SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
    //    command.ExecuteNonQuery();
    //}

    ////插入一些数据
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

    ////使用sql查询语句，并显示结果
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