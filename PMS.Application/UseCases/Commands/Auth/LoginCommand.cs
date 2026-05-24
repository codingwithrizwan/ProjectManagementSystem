using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Commands.Auth
{
    public record LoginCommand(LoginRequestDto Dto) : IRequest<ApiResponse<AuthResponseDto>>;
}
