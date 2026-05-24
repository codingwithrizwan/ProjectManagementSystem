using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Commands
{
    public record UpdateTaskStatusCommand(UpdateTaskStatusDto Dto, Guid? userId = null) : IRequest<ApiResponse<object>>;
}
