using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMS.Web.Exceptions;
using PMS.Web.Models;
using PMS.Web.Security;

namespace PMS.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        public IActionResult OnGet()
        {
            var redirect = AuthPageGuard.RedirectAuthenticatedUser(this);
            if (redirect != null)
            {
                return redirect;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var redirect = AuthPageGuard.RedirectAuthenticatedUser(this);
            if (redirect != null)
            {
                return redirect;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("PmsApi");
                var response = await client.PostAsJsonAsync("/api/Auth/login", new
                {
                    userName = Input.UserName,
                    password = Input.Password
                });

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiClientException("Login request failed.", (int)response.StatusCode);
                }

                var payload = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!payload?.Success ?? true)
                {
                    var firstError = payload?.Errors?.FirstOrDefault();
                    ModelState.AddModelError(string.Empty, firstError ?? payload?.Message ?? "Login failed.");
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(payload?.Data?.Token))
                {
                    ModelState.AddModelError(string.Empty, "Login succeeded but token was not returned.");
                    return Page();
                }

                Response.Cookies.Append(CookieKeys.AuthToken, payload.Data.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });

                Response.Cookies.Append(CookieKeys.Session, JsonSerializer.Serialize(payload.Data), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });

                return Redirect("/home");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Could not reach API. Please make sure PMS.API is running.");
                return Page();
            }
            catch (ApiClientException ex) when (ex.StatusCode == 401)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }
            catch (ApiClientException)
            {
                ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
                return Page();
            }
        }

        public class LoginInput
        {
            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }
    }
}
