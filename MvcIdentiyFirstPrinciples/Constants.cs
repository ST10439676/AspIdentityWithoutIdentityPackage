using System;
using System.ComponentModel.DataAnnotations;

namespace MvcIdentiyFirstPrinciples;

public enum Roles
{
    [Display(Name = "Admin")]
    ADMIN_ROLE,
    [Display(Name = "User")]
    USER_ROLE
}