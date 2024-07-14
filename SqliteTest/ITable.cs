using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest;

public interface ITable
{
    public string LocalName { get; }

    public List<Field> WriteFields();

    public void ReadFields(List<Field> fields);
}
