using MediatR;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Application.UseCases.Queries;

namespace PMS.Application.UseCases.Handlers
{
    public class GetEmployeeTaskByIdQueryHandler(ITaskRepository taskRepository) : IRequestHandler<GetEmployeeTaskByIdQuery, ApiResponse<TaskDto>>
    {
        public async Task<ApiResponse<TaskDto>> Handle(GetEmployeeTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await taskRepository.GetTaskById(request.UserId, request.TaskId, cancellationToken);
            if (task == null)
                return ApiResponse<TaskDto>.Fail("Assigned task fetch failed", ["Task not found or not assigned to current employee"]);

            return ApiResponse<TaskDto>.Success(task, "Assigned task fetched successfully");
        }
    }
}