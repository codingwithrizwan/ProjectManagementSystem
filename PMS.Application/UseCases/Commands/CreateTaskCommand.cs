using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Commands
{
    public record CreateTaskCommand(CreateTaskDto Dto) : IRequest<ApiResponse<long>>;
}
