using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Commands
{
    public record UpdateTaskStatusCommand(UpdateTaskStatusDto Dto) : IRequest<ApiResponse<object>>;
}
