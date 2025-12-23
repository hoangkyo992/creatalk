using System.ComponentModel;

namespace Common.Domain.Enums;

public enum ActivityLogAction
{
    [Description("C")]
    Create = 0,

    [Description("U")]
    Update = 1,

    [Description("D")]
    Delete = 2
}