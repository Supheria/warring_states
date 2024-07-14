using System.Text;

namespace SqliteTest;

public class Field(string name, Keywords type)
{
    public Volume Name { get; } = new(name);

    public Volume Type { get; } = new(type);

    public override string ToString()
    {
        return new StringBuilder()
            .Append(Name)
            .Append(Type)
            .ToString();
    }
}
