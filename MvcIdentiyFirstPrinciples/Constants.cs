using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MvcIdentiyFirstPrinciples;

public enum Roles
{
    [Display(Name = "Admin")]
    ADMIN_ROLE,
    [Display(Name = "User")]
    USER_ROLE
}

public static class RoleDisplayName {
    public static  string GetDisplayName(Roles role)
    {
        FieldInfo info = typeof(Roles).GetField(Enum.GetName(role)!);
        var display = info.GetCustomAttribute<DisplayAttribute>();
        return display.GetName();
    }
}