using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesUI.Library.Api;
using RazorPagesUI.Models;

namespace RazorPagesUI.Pages
{
    public class LoginModel : PageModel
    {
        private IApiHelper _apiHelper;

        public LoginModel(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        [BindProperty]
        public UserModel User { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _apiHelper.Authenticate(User.UserName, User.Password);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
            }

            return RedirectToPage("/");
        }

        public void OnGet()
        {
        }
    }
}
