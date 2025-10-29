using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MvcIdentiyFirstPrinciples.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    [Required, StringLength(50)]
    public string Username { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; } = "User";

    public static ClaimsIdentity CreateClaimIdentity(User user)
    {
        ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new(ClaimTypes.Name, user.Username));
        identity.AddClaim(new(ClaimTypes.Email, user.Email));
        identity.AddClaim(new(ClaimTypes.Role, user.Role));
        return identity;
    }

}
