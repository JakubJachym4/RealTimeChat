namespace RazorPagesUI.Library.Models;

public interface ILoggedInUser
{
    string UserName { get; set; }
    string Password { get; set; }
    string? ConfirmPassword { get; set; }
    string? Email { get; set; }
}