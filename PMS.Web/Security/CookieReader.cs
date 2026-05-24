

using PMS.Web.Models;
using System.Text.Json;

namespace PMS.Web.Security
{
    public static class CookieReader
    {
        private const string CookieName = CookieKeys.Session;

        public static LoginResponse? GetCookieData(HttpContext context)
        {
            if (!context.Request.Cookies.TryGetValue(CookieName, out var json) ||
                string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            try
            {
                return JsonSerializer.Deserialize<LoginResponse>(json);
            }
            catch
            {
                return null;
            }
        }

        public static void ClearAuthCookies(HttpContext context)
        {
            context.Response.Cookies.Delete(CookieKeys.AuthToken);
            context.Response.Cookies.Delete(CookieKeys.Session);
        }

        public static string? GetToken(HttpContext context)
        {
            return GetCookieData(context)?.Token;
        }

        public static string? GetRole(HttpContext context)
        {
            return GetCookieData(context)?.Role;
        }
    }
}