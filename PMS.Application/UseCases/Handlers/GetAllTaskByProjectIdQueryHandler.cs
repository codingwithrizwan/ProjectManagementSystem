using MediatR;
using PMS.Application.UseCases.Queries;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Domain.Services;

namespace PMS.Application.UseCases.Handlers
{
    public class GetAllTaskByProjectIdQueryHandler(ITaskRepository taskRepository,IProjectRepository projectRepository) : IRequestHandler<GetAllTaskByProjectIdQuery, ApiResponse<ProjectTaskDeatailsDto>>
    {
        public async Task<ApiResponse<ProjectTaskDeatailsDto>> Handle(GetAllTaskByProjectIdQuery query, CancellationToken cancellationToken)
        {
            var _project = await projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
            if (_project == null)
            {
                return ApiResponse<ProjectTaskDeatailsDto>.Fail("Project fetch failed", ["Project not found"]);
            }

            var _tasks = await taskRepository.GetAllTasksByProjectIdAsync(query.ProjectId, cancellationToken);
            var progress = ProjectProgressService.CalculateProjectProgress(_project);

            var dto = new ProjectTaskDeatailsDto
            {
                Id = _project.Id,
                Name = _project.Name,
                Description = _project.Description ?? string.Empty,
                Progress = progress,
                Tasks = _tasks.ToList()
            };

            return ApiResponse<ProjectTaskDeatailsDto>.Success(dto, "Project fetched successfully");
        }
    }
}
