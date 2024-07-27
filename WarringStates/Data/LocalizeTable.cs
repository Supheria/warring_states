using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map;

namespace WarringStates.Data;

public class LocalizeTable
{
    public virtual string Null { get; set; } = "?";
    public virtual string X { get; set; } = "X";
    public virtual string BuildLand { get; set; } = "建造场地";
    public virtual string NoBuildableType { get; set; } = "没有可建造的类型";
    public virtual string Login { get; set; } = "登录";
    public virtual string Logout { get; set; } = "登出";
    public virtual string Join { get; set; } = "加入";
    public virtual string HorseLand { get; set; } = "马场";
    public virtual string MineLand { get; set; } = "矿山";
    public virtual string FarmLand { get; set; } = "农田";
    public virtual string MulberryLand { get; set; } = "桑田";
    public virtual string WoodLand { get; set; } = "林场";
    public virtual string FishLand { get; set; } = "渔场";
    public virtual string TerraceLand { get; set; } = "梯田";

    public string ConvertEnum(Enum type)
    {
        return type switch
        {
            SourceLandTypes.HorseLand => HorseLand,
            SourceLandTypes.MineLand => MineLand,
            SourceLandTypes.FarmLand => FarmLand,
            SourceLandTypes.MulberryLand => MulberryLand,
            SourceLandTypes.WoodLand => WoodLand,
            SourceLandTypes.FishLand => FishLand,
            SourceLandTypes.TerraceLand => TerraceLand,
            _ => Null,
        };
    }
}
