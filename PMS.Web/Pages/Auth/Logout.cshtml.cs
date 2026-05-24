using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMS.Web.Security;

namespace PMS.Web.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            Response.Cookies.Delete(CookieKeys.AuthToken);
            Response.Cookies.Delete(CookieKeys.Session);
            return RedirectToPage("/Auth/Login");
        }
    }
}
