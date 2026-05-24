using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Commands
{
    public record CreateProjectCommand(CreateProjectDto Dto) : IRequest<ApiResponse<long>>;
}
