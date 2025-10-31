using System.ComponentModel.DataAnnotations;

namespace MvcIdentiyFirstPrinciples.ViewModels;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; } 
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
    [Required, DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required, EnumDataType(typeof(Roles))]
    public Roles Role { get; set; }
}
