using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMS.Web.Models;
using PMS.Web.Security;

namespace PMS.Web.Pages
{
    public class TaskModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TaskModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<TaskRowVm> Tasks { get; private set; } = [];
        public string? ApiError { get; private set; }
        public bool IsSingleTaskView { get; private set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(long? taskId)
        {
            if (!IsEmployee())
            {
                return Redirect("/home");
            }

            await LoadTasksAsync(taskId);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(long taskId, int status, long? openedTaskId)
        {
            if (!IsEmployee())
            {
                return Redirect("/home");
            }

            if (taskId <= 0 || status < 0 || status > 2)
            {
                ApiError = "Invalid task update request.";
                await LoadTasksAsync(openedTaskId);
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("PmsApi");
                var response = await client.PutAsJsonAsync("/api/Projects/update-task-status", new
                {
                    taskId,
                    status
                });

                var payload = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!response.IsSuccessStatusCode || payload?.Success != true)
                {
                    ApiError = payload?.Errors?.FirstOrDefault() ?? payload?.Message ?? "Task status update failed.";
                    await LoadTasksAsync(openedTaskId);
                    
                    return Page();
                }

                SuccessMessage = "Task status updated successfully.";
                if (openedTaskId.HasValue)
                {
                    return RedirectToPage("/Task", new { taskId = openedTaskId.Value });
                }

                return RedirectToPage("/Task");
            }
            catch (HttpRequestException)
            {
                ApiError = "Could not reach API. Please make sure PMS.API is running.";
                await LoadTasksAsync(openedTaskId);
                return Page();
            }
            catch (JsonException)
            {
                ApiError = "Could not parse API response.";
                await LoadTasksAsync(openedTaskId);
                return Page();
            }
        }

        private async Task LoadTasksAsync(long? taskId)
        {
            Tasks = [];
            IsSingleTaskView = taskId.HasValue;

            try
            {
                var client = _httpClientFactory.CreateClient("PmsApi");

                if (taskId.HasValue)
                {
                    var singleTaskResponse = await client.GetFromJsonAsync<ApiResponse<TaskApiDto>>($"/api/Projects/my-tasks/{taskId.Value}", new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (singleTaskResponse?.Success != true || singleTaskResponse.Data == null)
                    {
                        ApiError = singleTaskResponse?.Errors?.FirstOrDefault() ?? singleTaskResponse?.Message ?? "Selected task not found or not assigned to you.";
                        return;
                    }

                    var single = singleTaskResponse.Data;
                    Tasks =
                    [
                        new TaskRowVm(
                            single.Id,
                            single.Title,
                            single.ProjectName,
                            single.Status,
                            MapStatusText(single.Status),
                            single.CreatedAt,
                            single.StartedAt,
                            single.CompletedAt)
                    ];

                    return;
                }

                var response = await client.GetFromJsonAsync<ApiResponse<ProjectApiDto>>("/api/Projects/my-tasks", new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (response?.Success != true || response.Data?.Tasks == null)
                {
                    ApiError = response?.Errors?.FirstOrDefault() ?? response?.Message ?? "Could not load assigned tasks.";
                    return;
                }

                var mappedTasks = response.Data.Tasks
                    .Select(t => new TaskRowVm(
                        t.Id,
                        t.Title,
                        t.ProjectName,
                        t.Status,
                        MapStatusText(t.Status),
                        t.CreatedAt,
                        t.StartedAt,
                        t.CompletedAt))
                    .ToList();

                Tasks = mappedTasks;
            }
            catch (HttpRequestException)
            {
                ApiError = "Could not reach API. Please make sure PMS.API is running.";
            }
            catch (JsonException)
            {
                ApiError = "Could not parse API response.";
            }
        }

        private bool IsEmployee()
        {
            var cookie = CookieReader.GetCookieData(HttpContext);
            return cookie?.Role?.Equals("Employee", StringComparison.OrdinalIgnoreCase) == true;
        }

        private static string MapStatusText(int status)
        {
            return status switch
            {
                1 => "In Progress",
                2 => "Done",
                _ => "To Do"
            };
        }

        public record TaskRowVm(
            long Id,
            string Title,
            string ProjectName,
            int Status,
            string StatusText,
            DateTime CreatedAt,
            DateTime? StartedAt,
            DateTime? CompletedAt)
        {
            public string StartedAtDisplay => StartedAt.HasValue ? StartedAt.Value.ToString("dd MMM yyyy, hh:mm tt") : "-";
            public string CompletedAtDisplay => CompletedAt.HasValue ? CompletedAt.Value.ToString("dd MMM yyyy, hh:mm tt") : "-";
        }

        private class ProjectApiDto
        {
            public List<TaskApiDto> Tasks { get; set; } = [];
        }

        private class TaskApiDto
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string ProjectName { get; set; } = string.Empty;
            public int Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? StartedAt { get; set; }
            public DateTime? CompletedAt { get; set; }
        }
    }
}
