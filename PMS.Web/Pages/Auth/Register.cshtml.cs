using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMS.Web.Exceptions;
using PMS.Web.Models;
using PMS.Web.Security;

namespace PMS.Web.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        public IActionResult OnGet()
        {
            var redirect = PMS.Web.Security.AuthPageGuard.RedirectAuthenticatedUser(this);
            if (redirect != null)
            {
                return redirect;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var redirect = PMS.Web.Security.AuthPageGuard.RedirectAuthenticatedUser(this);
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
                var response = await client.PostAsJsonAsync("/api/Auth/register", new
                {
                    userName = Input.UserName,
                    email = Input.Email,
                    password = Input.Password
                });

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiClientException("Registration request failed.", (int)response.StatusCode);
                }

                var payload = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!payload?.Success ?? true)
                {
                    var firstError = payload?.Errors?.FirstOrDefault();
                    ModelState.AddModelError(string.Empty, firstError ?? payload?.Message ?? "Registration failed.");
                    return Page();
                }

                Input = new RegisterInput();
                Message = "Registration successful. Please login.";
                return RedirectToPage("/Auth/Login");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Could not reach API. Please make sure PMS.API is running.");
                return Page();
            }
            catch (ApiClientException ex) when (ex.StatusCode == 400)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Please check your inputs.");
                return Page();
            }
            catch (ApiClientException)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                return Page();
            }
        }

        public class RegisterInput
        {
            [Required]
            [Display(Name = "Username")]
            [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
            public string UserName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
            [RegularExpression(@"^[A-Z].*", ErrorMessage = "Password must start with a capital letter. (i.e 'Password@123')")]
            public string Password { get; set; } = string.Empty;
        }
    }
}
