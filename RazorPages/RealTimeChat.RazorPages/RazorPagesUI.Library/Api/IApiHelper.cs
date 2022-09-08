using RazorPagesUI.Library.Models;

namespace RazorPagesUI.Library.Api;

public interface IApiHelper
{
    Task<LoggedInUser> Authenticate(string username, string password, string confirmPassword, string email);
}