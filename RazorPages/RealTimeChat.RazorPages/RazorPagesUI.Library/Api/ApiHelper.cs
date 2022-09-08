using System.Net.Http.Headers;
using System.Net.Http.Json;
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

    private void InitialazeClient()
    {
        string apiUri = "https://localhost:7234/";

        apiClient = new HttpClient();
        apiClient.BaseAddress = new Uri(apiUri);
        apiClient.DefaultRequestHeaders.Accept.Clear();
        apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<LoggedInUser> Authenticate(string username, string password, string confirmPassword, string email)
    {

        var content = new Dictionary<string, string>()
        {
            { "userName", username },
            { "password", password },
            { "confirmPassword", confirmPassword },
            { "email", email }
        };
        
        var data = new FormUrlEncodedContent(content);


        using (var response = await apiClient.PostAsync("api/Account/Login", data))
        {
            var cos = await response.Content.ReadAsStringAsync();
            
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