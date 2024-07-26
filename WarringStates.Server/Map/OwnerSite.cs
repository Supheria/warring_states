using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map;

namespace WarringStates.Server.Map;

internal class OwnerSite()
{
    [TableField(IsPrimaryKey = true)]
    public Coordinate Site { get; set; } = new();

    public LandTypes Type { get; set; } = LandTypes.None;

    public string PlayerId { get; set; } = "";
}
