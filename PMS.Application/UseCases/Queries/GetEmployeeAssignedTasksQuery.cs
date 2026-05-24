using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Queries
{
    public record GetEmployeeAssignedTasksQuery(Guid UserId) : IRequest<ApiResponse<ProjectTaskDeatailsDto>>;
}