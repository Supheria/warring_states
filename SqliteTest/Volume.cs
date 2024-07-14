using LocalUtilities.TypeGeneral;
using System.Text;

namespace SqliteTest;

public class Volume
{
    public string Value { get; } 

    public Volume(string value)
    {
        Value = new StringBuilder()
            .Append(SignTable.Quote)
            .Append(value)
            .Append(SignTable.Quote)
            .ToString();
    }

    public Volume(Keywords keywords)
    {
        Value = keywords.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
