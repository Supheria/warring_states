using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using WarringStates.Map;

namespace WarringStates.Server.Map;

internal class OwnerSite(Coordinate site, SourceLandTypes type, string playerId)
{
    [TableField(IsPrimaryKey = true)]
    public Coordinate Site { get; set; } = site;

    public SourceLandTypes LandType { get; set; } = type;

    public string PlayerId { get; set; } = playerId;

    public OwnerSite() : this(new(), SourceLandTypes.None, "")
    {

    }
}
