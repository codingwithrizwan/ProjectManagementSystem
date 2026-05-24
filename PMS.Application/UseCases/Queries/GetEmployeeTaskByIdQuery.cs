using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Queries
{
    public record GetEmployeeTaskByIdQuery(Guid UserId, long TaskId) : IRequest<ApiResponse<TaskDto>>;
}