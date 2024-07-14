using LocalUtilities.TypeGeneral;
using System.Text;

namespace SqliteTest;

public class Volume
{
    public string Value { get; } 

    public Volume(string value)
    {
        Value = new StringBuilder()
            .Append(SignTable.SingleQuote)
            .Append(value)
            .Append(SignTable.SingleQuote)
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
