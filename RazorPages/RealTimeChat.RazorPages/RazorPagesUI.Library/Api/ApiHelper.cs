using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using RazorPagesUI.Library.Models;

namespace RazorPagesUI.Library.Api;

public class ApiHelper : IApiHelper
{
    private HttpClient apiClient { get; set; }
    private ILoggedInUser _loggedInUser { get; set; }

    public ApiHelper(ILoggedInUser loggedInUser)
    {
        InitialazeClient();
        _loggedInUser = loggedInUser;
    }

    private async Task InitialazeClient()
    {
        string apiUri = "https://localhost:7234/";

        apiClient = new HttpClient();
        apiClient.BaseAddress = new Uri(apiUri);
        apiClient.DefaultRequestHeaders.Accept.Clear();
        apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
       
    }

    public async Task<LoggedInUser> Register(string username, string password, string confirmPassword, string email)
    {

        var data = new Dictionary<string, string>()
         {
             { "userName", username },
             { "password", password },
             { "confirmPassword", confirmPassword },
             { "email", email }
         };

        // var data = new FormUrlEncodedContent(content);

        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using (var response = await apiClient.PostAsync("api/Account/Register", content))
        {

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoggedInUser>();
                return result;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }

    public async Task<LoggedInUser> Authenticate(string username, string password)
    {
        var data = new Dictionary<string, string>()
        {
            {"userName", username},
            {"password", password}
        };

        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using (var response = await apiClient.PostAsync("api/Account/Login", content))
        {

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoggedInUser>();
                return result;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}