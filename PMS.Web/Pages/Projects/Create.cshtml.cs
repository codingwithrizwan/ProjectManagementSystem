using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMS.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using static PMS.Web.Pages.Auth.RegisterModel;

namespace PMS.Web.Pages.Projects
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public CreateProjectInputModel Input { get; set; } = new();

        public string? ApiError { get; private set; }

        public IActionResult OnGet()
        {
            TempData["Message"] = "";
            var cookie = CookieReader.GetCookieData(HttpContext);
            if (cookie == null || string.IsNullOrWhiteSpace(cookie.Role) || !cookie.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
           
            var cookie = CookieReader.GetCookieData(HttpContext);
            if (cookie == null || string.IsNullOrWhiteSpace(cookie.Role) || !cookie.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Auth/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("PmsApi");
                var response = await client.PostAsJsonAsync("/api/Projects", new
                {
                    name = Input.Name,
                    description = Input.Description
                });

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, $"Project create failed ({(int)response.StatusCode}).");
                    return Page();
                }
                Input = new CreateProjectInputModel();
                TempData["Message"] = "Project created successfully.";
                return RedirectToPage("/Index");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Please make sure app is running.");
                return Page();
            }
        }

        public class CreateProjectInputModel
        {
            [Required]
            [Display(Name = "Project Name")]
            public string Name { get; set; } = string.Empty;

            [Display(Name = "Description")]
            public string? Description { get; set; }
        }
    }
}
