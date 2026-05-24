using MediatR;
using PMS.Application.Common;
using PMS.Application.DTOs;
using PMS.Application.UseCases.Commands;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;

namespace PMS.Application.UseCases.Handlers
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ApiResponse<long>>
    {
        private readonly IProjectRepository _projects;
        private readonly IAppCache _cache;

        public CreateProjectCommandHandler(IProjectRepository projects, IAppCache cache)
        {
            _projects = projects;
            _cache = cache;
        }

        public async Task<ApiResponse<long>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
        {
            var dto = command.Dto;
            var name = dto.Name.Trim();
            var exists = await _projects.ExistsByNameAsync(name, cancellationToken);
            if (exists)
            {
                return ApiResponse<long>.Fail("Project create failed", ["Project with the same name already exists."]);
            }
            var project = new Project(name, dto.Description);
            await _projects.AddAsync(project, cancellationToken);
            await _projects.SaveChangesAsync(cancellationToken);

            _cache.Remove(CacheKeys.DashboardProjects);

            return ApiResponse<long>.Success(project.Id, "Project created successfully");
        }
    }
}
