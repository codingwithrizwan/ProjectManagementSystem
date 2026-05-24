using MediatR;
using PMS.Application.Common;
using PMS.Application.DTOs;
using PMS.Application.UseCases.Commands;
using PMS.Application.Interfaces;

namespace PMS.Application.UseCases.Handlers
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ApiResponse<long>>
    {
        private readonly IProjectRepository _projects;
        private readonly IEmployeeRepository _employees;
        private readonly ITaskRepository _tasks;
        private readonly IAppCache _cache;

        public CreateTaskCommandHandler(IProjectRepository projects, IEmployeeRepository employees, ITaskRepository tasks, IAppCache cache)
        {
            _projects = projects;
            _employees = employees;
            _tasks = tasks;
            _cache = cache;
        }

        public async Task<ApiResponse<long>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            var dto = command.Dto;
            var project = await _projects.GetByIdAsync(dto.ProjectId, cancellationToken);
            if (project == null)
            {
                return ApiResponse<long>.Fail("Task create failed", ["Project not found"]);
            }

            var employee = await _employees.GetByIdAsync(dto.AssignedEmployeeId, cancellationToken);

            if (employee == null)
            {
                return ApiResponse<long>.Fail("Task create failed", ["Employee not found"]);
            }

            var task = project.AddTask(dto.Title, employee.Id, dto.Description);
            await _tasks.AddAsync(task, cancellationToken);
            await _tasks.SaveChangesAsync(cancellationToken);

            _cache.Remove(CacheKeys.DashboardProjects);

            return ApiResponse<long>.Success(task.Id, "Task created successfully");
        }
    }
}
