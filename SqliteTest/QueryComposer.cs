using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest;

internal class QueryComposer
{
    StringBuilder Query { get; } = new();

    public QueryComposer Finish()
    {
        Query.Append(SignTable.Semicolon)
            .Append(SignTable.Space);
        return this;
    }

    public QueryComposer Append(string word)
    {
        Query.Append(word)
            .Append(SignTable.Space);
        return this;
    }

    public QueryComposer Append(char sign)
    {
        Query.Append(sign)
            .Append(SignTable.Space);
        return this;
    }

    public QueryComposer Append(object obj)
    {
        Query.Append(obj.ToString())
            .Append(SignTable.Space);
        return this;
    }

    public QueryComposer AppendValue(string value)
    {
        Query.Append(SignTable.SingleQuote)
                .Append(value)
                .Append(SignTable.SingleQuote)
                .Append(SignTable.Space);
        return this;
    }

    public QueryComposer AppendValues(string[] values)
    {
        Query.Append(SignTable.OpenParenthesis)
            .AppendJoin(SignTable.Comma, values, (sb, value) =>
            {
                sb.Append(SignTable.SingleQuote)
                .Append(value)
                .Append(SignTable.SingleQuote);
            })
            .Append(SignTable.CloseParenthesis)
            .Append(SignTable.Space);
        return this;
    }

    public QueryComposer AppendColumnFields(ColumnField[] columnFields)
    {
        Query.AppendJoin(SignTable.Comma, columnFields, (sb, field) =>
        {
            sb.Append(field.Key)
            .Append(SignTable.Equal)
            .Append(SignTable.SingleQuote)
            .Append(field.Value)
            .Append(SignTable.SingleQuote)
            .Append(SignTable.Space);
        });
        return this;
    }

    public override string ToString()
    {
        return Query.ToString();
    }
}
