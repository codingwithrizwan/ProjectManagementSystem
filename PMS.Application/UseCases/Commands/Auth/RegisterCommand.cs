using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Commands.Auth
{
    public record RegisterCommand(RegisterRequestDto Dto) : IRequest<ApiResponse<AuthResponseDto>>;
}
