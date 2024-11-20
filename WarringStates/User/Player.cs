using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates.User;

public class Player(string name, string password) : IRosterItem<string>
{
    [TableField(IsPrimaryKey = true)]
    public string Name { get; set; } = name;

    public string Password { get; set; } = password;

    public string Signature => Name;

    public Player() : this("", "")
    {

    }

    public static string ConvertPasswortText(string passwordText)
    {
        return HashTool.ToMd5HashString(passwordText);
    }

    public string GetHashId()
    {
        return HashTool.ToMd5HashString(Name + Password);
    }
}
