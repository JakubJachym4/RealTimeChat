using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;

namespace RazorPagesUI.Models;

public class UserModel
{
    [Required(ErrorMessage = "This field is required.")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "This field is required.")]
    [MinLength(6, ErrorMessage = "Password is too short.")]
    public string Password { get; set; }
    [Compare("Password", ErrorMessage = "Passwords must match.")]
    public string? ConfirmPassword { get; set; }
    public string? Email { get; set; }
}