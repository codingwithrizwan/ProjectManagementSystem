using MediatR;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Application.UseCases.Queries;


namespace PMS.Application.UseCases.Handlers
{
    public class GetEmployeeAssignedTasksQueryHandler(ITaskRepository taskRepository) : IRequestHandler<GetEmployeeAssignedTasksQuery, ApiResponse<ProjectTaskDeatailsDto>>
    {
        public async Task<ApiResponse<ProjectTaskDeatailsDto>> Handle(GetEmployeeAssignedTasksQuery request, CancellationToken cancellationToken)
        {
            var _tasks = await taskRepository.GetTasksAsync(request.UserId, cancellationToken);
            var totalCount = _tasks.Count();
            var completed = _tasks.Count(t => t.Status == PMS.Domain.Enums.TaskStatus.Done);
            var progress = totalCount == 0 ? 0 : (double)completed / totalCount * 100.0;
            var dto = new ProjectTaskDeatailsDto
            {
                Name = "My Assigned Tasks",
                Description = "All tasks assigned to current employee",
                Progress = progress,
                Tasks = _tasks.ToList()
            };

            return ApiResponse<ProjectTaskDeatailsDto>.Success(dto, "Assigned tasks fetched successfully");
        }
    }
}