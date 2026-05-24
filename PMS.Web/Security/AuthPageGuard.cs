using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PMS.Web.Security
{
    public static class AuthPageGuard
    {
        public static IActionResult? RedirectAuthenticatedUser(PageModel pageModel)
        {
            var session = CookieReader.GetCookieData(pageModel.HttpContext);
            if (string.IsNullOrWhiteSpace(session?.Token) || string.IsNullOrWhiteSpace(session.UserName))
            {
                return null;
            }

            var redirectPage = "/Index";

            return pageModel.RedirectToPage(redirectPage);
        }
    }
}
