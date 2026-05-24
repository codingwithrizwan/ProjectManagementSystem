using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMS.Web.Models;
using PMS.Web.Security;

namespace PMS.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public int TotalProjects { get; private set; }
        public int OverallAverageProgress { get; private set; }
        public IReadOnlyList<ProjectCardVm> Projects { get; private set; } = [];
        public IReadOnlyList<EmployeeTaskVm> EmployeeTasks { get; private set; } = [];
        public bool IsLoggedIn { get; private set; }
        public bool IsAdmin { get; private set; }
        public bool IsEmployee { get; private set; }
        public string? ApiError { get; private set; }

        [TempData]
        public string? Message { get; set; }

        public async Task OnGetAsync()
        {
            LoadUserContext();
            await LoadDashboardCardsAsync();
        }

        private void LoadUserContext()
        {
            var _cookies = CookieReader.GetCookieData(HttpContext);
            IsLoggedIn = !string.IsNullOrWhiteSpace(_cookies?.UserName);
            var roles = _cookies?.Role;
            IsAdmin = roles?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
            IsEmployee = roles?.Equals("Employee", StringComparison.OrdinalIgnoreCase) == true;
        }

        private async Task LoadDashboardCardsAsync()
        {
            Projects = [];
            EmployeeTasks = [];
            TotalProjects = 0;
            OverallAverageProgress = 0;

            if (!IsLoggedIn)
            {
                return;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("PmsApi");
                if(IsAdmin){
                var response = await client.GetFromJsonAsync<ApiResponse<ProjectDashboardApiDto>>("/api/Projects", new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (response?.Success != true || response.Data == null)
                {
                    ApiError = response?.Errors?.FirstOrDefault() ?? response?.Message ?? "Could not load projects from API.";
                    return;
                }

                var dashboard = response.Data;

                var projects = dashboard.Projects
                    .Select(p => new ProjectCardVm(
                        p.Id,
                        p.Name,
                        p.Description ?? string.Empty,
                        p.TaskCount,
                        (int)Math.Round(p.Progress)))
                    .ToList();

                Projects = projects;
                TotalProjects = dashboard.TotalProjects;
                OverallAverageProgress = dashboard.OverallAverageProgress;
                }
                if (IsEmployee)
                {
                    var myTasksResponse = await client.GetFromJsonAsync<ApiResponse<ProjectApiDto>>("/api/Projects/my-tasks", new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (myTasksResponse?.Success == true && myTasksResponse.Data?.Tasks != null)
                    {
                        EmployeeTasks = myTasksResponse.Data.Tasks
                            .Select(t => new EmployeeTaskVm(
                                t.Id,
                                t.Title,
                                t.ProjectId,
                                t.ProjectName,
                                MapStatus(t.Status),
                                t.CreatedAt,
                                t.StartedAt,
                                t.CompletedAt))
                            .ToList();
                    }
                }
            }
            catch (HttpRequestException)
            {
                ApiError = "Please make sure app is running.";
            }
            catch (NotSupportedException)
            {
                ApiError = "Unexpected response. Something went wrong when loading data.";
            }
            catch (JsonException)
            {
                ApiError = "Could not parse response. Something went wrong when loading data.";
            }
        }

        private class ProjectDashboardApiDto
        {
            public int TotalProjects { get; set; }
            public int OverallAverageProgress { get; set; }
            public List<ProjectListApiDto> Projects { get; set; } = [];
        }

        private class ProjectApiDto
        {
            public List<TaskApiDto> Tasks { get; set; } = [];
        }

        private class TaskApiDto
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public int Status { get; set; }
            public long ProjectId { get; set; }
            public string ProjectName { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? StartedAt { get; set; }
            public DateTime? CompletedAt { get; set; }
        }

        private class ProjectListApiDto
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int TaskCount { get; set; }
            public double Progress { get; set; }
        }

        public record ProjectCardVm(long Id, string Name, string Description, int TaskCount, int Progress);

        public record EmployeeTaskVm(
            long TaskId,
            string Title,
            long ProjectId,
            string ProjectName,
            EmployeeTaskStatus Status,
            DateTime CreatedAt,
            DateTime? StartedAt,
            DateTime? CompletedAt)
        {
            public string StartedAtDisplay => StartedAt.HasValue ? StartedAt.Value.ToString("dd MMM yyyy, hh:mm tt") : "-";
            public string CompletedAtDisplay => CompletedAt.HasValue ? CompletedAt.Value.ToString("dd MMM yyyy, hh:mm tt") : "-";

            public string StatusText => Status switch
            {
                EmployeeTaskStatus.InProgress => "In Progress",
                EmployeeTaskStatus.Done => "Done",
                _ => "To Do"
            };

            public string StatusCssClass => Status switch
            {
                EmployeeTaskStatus.Done => "role-pill",
                EmployeeTaskStatus.InProgress => "badge-status badge-progress",
                _ => "badge-status badge-todo"
            };
        }

        public enum EmployeeTaskStatus
        {
            ToDo = 0,
            InProgress = 1,
            Done = 2
        }

        private static EmployeeTaskStatus MapStatus(int status)
        {
            return status switch
            {
                1 => EmployeeTaskStatus.InProgress,
                2 => EmployeeTaskStatus.Done,
                _ => EmployeeTaskStatus.ToDo
            };
        }
    }
}
