using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest;

internal class Condition(string key, string value, OperatorTypes operate)
{
    public string Key { get; } = key;

    public string Value { get; } = value;

    public OperatorTypes Operate { get; } = operate;
}


internal class ColumnField(string key, string value)
{
    public string Key { get; set; } = key;

    public string Value { get; set; } = value;
}