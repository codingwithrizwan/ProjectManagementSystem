using MediatR;
using PMS.Application.UseCases.Commands.Auth;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;

namespace PMS.Application.UseCases.Handlers.Auth
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IUserService _users;
        private readonly IJwtTokenService _jwt;

        public RegisterCommandHandler(IUserService users, IJwtTokenService jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var created = await _users.CreateUserAsync(dto.UserName, dto.Email, dto.Password);
            if (created is null)
            {
                return ApiResponse<AuthResponseDto>.Fail("Registration failed", ["Could not create user."]);
            }

            var roleClaims = created.Roles?.Select(r => new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, r));
            var token = _jwt.CreateToken(created.Id, created.UserName, roleClaims);
            var primaryRole = created.Roles?.FirstOrDefault() ?? string.Empty;

            var authData = new AuthResponseDto 
            { 
                UserId = created.Id, 
                Token = token, 
                UserName = created.UserName, 
                Email = created.Email, 
                Role = primaryRole 
            };
            return ApiResponse<AuthResponseDto>.Success(authData, "Registration successful");
        }
    }
}
