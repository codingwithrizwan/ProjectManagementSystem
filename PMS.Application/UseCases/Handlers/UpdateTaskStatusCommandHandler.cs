using MediatR;
using PMS.Application.Common;
using PMS.Application.DTOs;
using PMS.Application.UseCases.Commands;
using PMS.Application.Interfaces;

namespace PMS.Application.UseCases.Handlers
{
    public class UpdateTaskStatusCommandHandler(ITaskRepository taskRepository, IEmployeeRepository employeeRepository, IAppCache  appCache) : IRequestHandler<UpdateTaskStatusCommand, ApiResponse<object>>
    {
        public async Task<ApiResponse<object>> Handle(UpdateTaskStatusCommand command, CancellationToken cancellationToken)
        {
            var dto = command.Dto;
            if (dto.TaskId <= 0)
            {
                return ApiResponse<object>.Fail("Task update failed", ["Invalid task id"]);
            }

            if (!Enum.IsDefined(dto.Status))
            {
                return ApiResponse<object>.Fail("Task update failed", ["Invalid task status"]);
            }

            var task = await taskRepository.GetByIdAsync(dto.TaskId, cancellationToken);
            if (task == null)
            {
                return ApiResponse<object>.Fail("Task update failed", ["Task not found"]);
            }

            if (command.userId.HasValue)
            {
                var employee = await employeeRepository.GetByUserIdAsync(command.userId.Value, cancellationToken);
                if (employee == null)
                {
                    return ApiResponse<object>.Fail("Task update failed", ["Employee not found for current user"]);
                }

                if (task.AssignedEmployeeId != employee.Id)
                {
                    return ApiResponse<object>.Fail("Task update failed", ["You can update only your assigned tasks"]);
                }
            }

            task.UpdateStatus(dto.Status);// domain business logic 
            await taskRepository.SaveChangesAsync(cancellationToken);

            // while creating i am refreshing cache that updated task will be show on page quickly.
            appCache.Remove(CacheKeys.DashboardProjects);

            return ApiResponse<object>.Success(null, "Task status updated successfully");
        }
    }
}
