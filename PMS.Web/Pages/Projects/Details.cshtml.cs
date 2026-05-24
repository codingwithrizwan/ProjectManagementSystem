using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMS.Web.Models;
using PMS.Web.Security;

namespace PMS.Web.Pages.Projects
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public long ProjectId { get; private set; }
        public string ProjectName { get; private set; } = string.Empty;
        public int Progress { get; private set; }
        public IReadOnlyList<TaskRowVm> Tasks { get; private set; } = [];
        public List<SelectListItem> EmployeeSelectItems { get; private set; } = [];
        public bool IsLoggedIn { get; private set; }
        public bool IsAdmin { get; private set; }
        public bool IsEmployee { get; private set; }
        public string? ApiError { get; private set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [BindProperty]
        public CreateTaskInputModel CreateTaskInput { get; set; } = new();

        [BindProperty]
        public UpdateStatusInputModel UpdateStatusInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (!id.HasValue)
            {
                return Redirect("/home");
            }

            LoadUserContext();
            var loaded = await LoadProjectAsync(id.Value);
            if (!loaded)
            {
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateTaskAsync(long id)
        {
            LoadUserContext();
            var loaded = await LoadProjectAsync(id);
            if (!loaded)
            {
                return Page();
            }

            if (!IsAdmin)
            {
                ApiError = "Only Admin can create and assign tasks.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("PmsApi");
                var response = await client.PostAsJsonAsync("/api/Projects/create-task", new
                {
                    projectId = id,
                    title = CreateTaskInput.Title,
                    description = CreateTaskInput.Description,
                    assignedEmployeeId = CreateTaskInput.AssignedEmployeeId
                });

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ApiError = error?.Errors?.FirstOrDefault()
                        ?? error?.Message
                        ?? $"Task create failed ({(int)response.StatusCode}).";
                    return Page();
                }

                SuccessMessage = "Task assigned successfully.";
                // reset form
                CreateTaskInput = new CreateTaskInputModel();
                ModelState.Clear();
                await LoadProjectAsync(id);
                return Page();
            }
            catch (HttpRequestException)
            {
                ApiError = "Could not reach API. Please make sure PMS.API is running.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(long id)
        {
            LoadUserContext();
            var loaded = await LoadProjectAsync(id);
            if (!loaded)
            {
                return Page();
            }

            if (!IsEmployee)
            {
                ApiError = "Only Employee can update task status.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("PmsApi");
                var response = await client.PutAsJsonAsync("/api/Projects/update-task-status", new
                {
                    taskId = UpdateStatusInput.TaskId,
                    status = UpdateStatusInput.Status
                });

                if (!response.IsSuccessStatusCode)
                {
                    ApiError = $"Status update failed ({(int)response.StatusCode}).";
                    return Page();
                }

                SuccessMessage = "Task status updated.";
                UpdateStatusInput = new UpdateStatusInputModel();
                ModelState.Clear();
                return RedirectToPage(new { id });
            }
            catch (HttpRequestException)
            {
                ApiError = "Could not reach API. Please make sure PMS.API is running.";
                return Page();
            }
        }

        private void LoadUserContext()
        {
            var cookies = CookieReader.GetCookieData(HttpContext);
            IsLoggedIn = !string.IsNullOrWhiteSpace(cookies?.UserName);
            var role = cookies?.Role;
            IsAdmin = role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
            IsEmployee = role?.Equals("Employee", StringComparison.OrdinalIgnoreCase) == true;
        }

        private async Task<bool> LoadProjectAsync(long id)
        {
            ProjectId = id;

            try
            {
                var client = _httpClientFactory.CreateClient("PmsApi");
                var projectEndpoint = $"/api/Projects/{id}";
                var apiResponse = await client.GetFromJsonAsync<ApiResponse<ProjectApiDto>>(projectEndpoint, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse == null)
                {
                    ApiError = "Unexpected empty response from API.";
                    ProjectName = "Project Detail";
                    Tasks = [];
                    EmployeeSelectItems = [];
                    return false;
                }

                if (!apiResponse.Success)
                {
                    ApiError = apiResponse.Errors?.FirstOrDefault() ?? apiResponse.Message;
                    ProjectName = "Project Not Found";
                    Tasks = [];
                    EmployeeSelectItems = [];
                    return false;
                }

                var dto = apiResponse.Data;

                if (dto == null)
                {
                    ApiError = "Project not found.";
                    ProjectName = "Project Not Found";
                    Tasks = [];
                    EmployeeSelectItems = [];
                    return false;
                }

                ProjectName = dto.Name;
                Progress = (int)Math.Round(dto.Progress);

                var visibleTasks = dto.Tasks;
                if (IsEmployee)
                {
                    var myTaskIds = await LoadMyTaskIdsForProjectAsync(client, id);
                    if (myTaskIds == null)
                    {
                        ApiError = "Could not load your assigned tasks.";
                        Tasks = [];
                        EmployeeSelectItems = [];
                        return false;
                    }

                    visibleTasks = dto.Tasks
                        .Where(t => myTaskIds.Contains(t.Id))
                        .ToList();
                }

                Tasks = visibleTasks.Select(MapTask).ToList();

                if (IsAdmin)
                {
                    await LoadEmployeesAsync(client);
                }
                else
                {
                    EmployeeSelectItems = [];
                }

                return true;
            }
            catch (HttpRequestException)
            {
                ApiError = "Could not reach API. Please make sure PMS.API is running.";
                ProjectName = "API Unavailable";
                Tasks = [];
                EmployeeSelectItems = [];
                return false;
            }
            catch (NotSupportedException)
            {
                ApiError = "Unexpected response from API.";
                ProjectName = "Project Detail";
                Tasks = [];
                EmployeeSelectItems = [];
                return false;
            }
            catch (JsonException)
            {
                ApiError = "Could not parse API response.";
                ProjectName = "Project Detail";
                Tasks = [];
                EmployeeSelectItems = [];
                return false;
            }
        }

        private async Task LoadEmployeesAsync(HttpClient client)
        {
            var employeesResponse = await client.GetFromJsonAsync<ApiResponse<List<EmployeeApiDto>>>("/api/Projects/employees", new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (employeesResponse?.Success != true || employeesResponse.Data == null)
            {
                EmployeeSelectItems = [];
                return;
            }

            EmployeeSelectItems = employeesResponse.Data
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(e.Name) ? e.Email : e.Name
                })
                .ToList();
        }

        private async Task<HashSet<long>?> LoadMyTaskIdsForProjectAsync(HttpClient client, long projectId)
        {
            var myTasksResponse = await client.GetFromJsonAsync<ApiResponse<ProjectApiDto>>("/api/Projects/my-tasks", new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (myTasksResponse?.Success != true || myTasksResponse.Data?.Tasks == null)
            {
                return null;
            }

            return myTasksResponse.Data.Tasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => t.Id)
                .ToHashSet();
        }

        private static TaskRowVm MapTask(TaskApiDto task)
        {
            var statusText = task.Status switch
            {
                1 => "In Progress",
                2 => "Done",
                _ => "To Do"
            };

            var statusClass = task.Status switch
            {
                1 => "badge-progress",
                2 => "badge-done",
                _ => "badge-todo"
            };

            return new TaskRowVm(
                task.Id,
                task.Title,
                statusText,
                statusClass,
                string.IsNullOrWhiteSpace(task.AssignedEmployeeName)
                    ? task.AssignedEmployeeId.ToString()
                    : task.AssignedEmployeeName,
                task.CreatedAt,
                task.StartedAt,
                task.CompletedAt);
        }

        public record TaskRowVm(
            long Id,
            string Title,
            string Status,
            string StatusClass,
            string AssignedEmployee,
            DateTime CreatedAt,
            DateTime? StartedAt,
            DateTime? CompletedAt)
        {
            public string StartedAtDisplay => StartedAt.HasValue ? StartedAt.Value.ToString("dd MMM yyyy, hh:mm tt") : "-";
            public string CompletedAtDisplay => CompletedAt.HasValue ? CompletedAt.Value.ToString("dd MMM yyyy, hh:mm tt") : "-";
        }

        public class CreateTaskInputModel
        {
            [Required]
            [Display(Name = "Task Title")]
            public string Title { get; set; } = string.Empty;

            [Display(Name = "Description")]
            public string? Description { get; set; }

            [Required]
            [Display(Name = "Assigned Employee")]
            public long? AssignedEmployeeId { get; set; }
        }

        public class UpdateStatusInputModel
        {
            [Required]
            public long TaskId { get; set; }

            [Range(0, 2)]
            public int Status { get; set; }
        }

        private class ProjectApiDto
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public double Progress { get; set; }
            public List<TaskApiDto> Tasks { get; set; } = [];
        }

        private class TaskApiDto
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int Status { get; set; }
            public long AssignedEmployeeId { get; set; }
            public string AssignedEmployeeName { get; set; } = string.Empty;
            public long ProjectId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? StartedAt { get; set; }
            public DateTime? CompletedAt { get; set; }
        }

        private class EmployeeApiDto
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }
    }
}
