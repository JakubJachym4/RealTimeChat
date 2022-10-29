using RazorPagesUI.Library.Models;

namespace RazorPagesUI.Library.Api;

public interface IApiHelper
{
    Task<LoggedInUser> Register(string username, string password, string confirmPassword, string email);

    Task<LoggedInUser> Authenticate(string username, string password);
}