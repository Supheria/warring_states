using System.ComponentModel;

namespace WarringStates.Flow;

public enum DateType
{
    [Description("周一")]
    Monday,
    [Description("周二")]
    Tuesday,
    [Description("周三")]
    Wednsday,
    [Description("周四")]
    Thursday,
    [Description("周五")]
    Friday,
    [Description("周六")]
    Saturday,
    [Description("周日")]
    Sunday,
}
