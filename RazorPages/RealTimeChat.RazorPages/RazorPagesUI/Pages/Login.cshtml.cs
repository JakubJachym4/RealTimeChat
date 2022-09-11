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
        
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            try
            {
                var result = await _apiHelper.Authenticate(User.UserName, User.Password,
                    User.ConfirmPassword, User.Email);
            }
            catch (Exception ex)
            {
                var ErrorMessage = ex.Message;
            }
            
            
            return RedirectToPage("/Login");
        }
    }
}