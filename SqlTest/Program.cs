// See https://aka.ms/new-console-template for more information
using LocalUtilities.SQLiteHelper;
using System.Diagnostics;

var stop = new Stopwatch();
var players = new List<Player>();
for (var i = 0; i < 50000; i++)
{
    players.Add(new Player("\"1  \\\"\\\" 345435b #/ 23456\"", "123456"));
}
for (var i = 0; i < 50000; i++)
{
    players.Add(new Player("fuck", "54321" + i.ToString()));
}
using var q = new SQLiteQuery("test.db");
q.CreateTable<Player>("test");
q.CreateTable<Player>("shit");
var names = q.ListAllTableNames();
stop.Start();
q.InsertItem("test", new Player("shit", "12345"), InsertTypes.ReplaceIfExists);
q.Commit();
q.InsertItem("test", new Player(null, "54321"), InsertTypes.IgnoreIfExists);
q.Commit();
var p = q.SelectItems<Player>("test", null).FirstOrDefault();
q.InsertItems("test", players.ToArray(), InsertTypes.ReplaceIfExists);
q.Commit();
stop.Stop();
Console.WriteLine("insert: " + stop.ElapsedMilliseconds);
stop.Restart();
q.Begin();
var x = q.SelectItems<Player>("test", null);
stop.Stop();
Console.WriteLine($"select({x.Length}): " + stop.ElapsedMilliseconds);
//var a = q.SelectItems<Player>("test", null, null);
var playerShit = new Player("\"1  \\\"\\\" 345435b #/ 23456\"", "123456");
var playerFuck = new Player("fuck", "54321");
var playerHello = new Player("hello", "78910");
var condition1 = SQLiteQuery.GetCondition(playerShit, Operators.Equal, nameof(Player.Name));
var condition2 = SQLiteQuery.GetCondition(playerFuck, Operators.Equal, nameof(Player.Name));
stop.Restart();
q.UpdateItems("test", SQLiteQuery.GetFieldValues(playerHello), [condition1, condition2], ConditionCombo.Or);
Console.WriteLine(q.Sum("test", null, null));
q.Commit();
stop.Stop();
Console.WriteLine("update: " + stop.ElapsedMilliseconds);
var playerWorld = new Player("World", "joskoadfmof");
q.Begin();
q.UpdateItems(
    "test",
    SQLiteQuery.GetFieldValues(playerWorld),
    SQLiteQuery.GetCondition(playerHello, Operators.Equal, nameof(Player.Name)));
var player = new Player("tte", "35");
q.InsertItem("test", player, InsertTypes.IgnoreIfExists);
player = new Player("tte", "35");
Console.WriteLine("exist: " + q.Exist("test", player).ToString());
//q.UpdateItems(
//    "test",
//    SQLiteQuery.GetFieldValues(playerWorld),
//    SQLiteQuery.GetConditions(playerHello, nameof(Player.Name), nameof(Player.Password)).ToArray(),
//    ConditionCombo.And);
//q.Dispose();

public class Player(string? name, string password)
{
    [TableField(IsPrimaryKey = true/*, Name = "shit fuck it"*/)]
    public string Id { get; private set; } = "10";

    //[TableField(Name = "shit the shit wha tfuch @@\' \" # sjf= -dsmsdf oiwee9\" ")]
    [TableField(IsUnique = true)]
    public string? Name { get; set; } = name;

    public string Password { get; private set; } = password;

    [TableField(IsUnique = true)]
    public Coordinate? Coordinate { get; set; } = null;

    public Player() : this("admin", "password")
    {

    }
}