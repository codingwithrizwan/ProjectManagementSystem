using MediatR;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Application.UseCases.Queries;
using PMS.Domain.Services;

namespace PMS.Application.UseCases.Handlers
{
    public class GetProjectsQueryHandler(IProjectRepository projectRepository) : IRequestHandler<GetProjectsQuery, ApiResponse<ProjectDashboardDto>>
    {

        public async Task<ApiResponse<ProjectDashboardDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await projectRepository.ListAsync(cancellationToken);

            var items = projects
                .Select(p => new ProjectListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    TaskCount = p.Tasks.Count,
                    Progress = ProjectProgressService.CalculateProjectProgress(p)
                })
                .ToList();

            var totalProjects = projects.Count;
            var overallProgress = ProjectProgressService.CalculateOverallAverageProgress(projects);

            var dashboard = new ProjectDashboardDto
            {
                TotalProjects = totalProjects,
                OverallAverageProgress = (int)Math.Round(overallProgress),
                Projects = items
            };

            return ApiResponse<ProjectDashboardDto>.Success(dashboard, "Projects fetched successfully");
        }
    }
}