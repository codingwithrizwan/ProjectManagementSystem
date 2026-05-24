using MediatR;
using PMS.Application.UseCases.Commands.Auth;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;

namespace PMS.Application.UseCases.Handlers.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IUserService _users;
        private readonly IJwtTokenService _jwt;

        public LoginCommandHandler(IUserService users, IJwtTokenService jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var user = await _users.ValidateUserAsync(dto.UserName, dto.Password);
            if (user is null)
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid credentials", ["Username or password is incorrect."]);
            }

            var roleClaims = user.Roles?.Select(r => new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, r));
            var token = _jwt.CreateToken(user.Id, user.UserName, roleClaims);
            var primaryRole = user.Roles?.FirstOrDefault() ?? string.Empty;
            var authData = new AuthResponseDto
            {
                UserId = user.Id,
                Token = token,
                UserName = user.UserName,
                Email = user.Email,
                Role = primaryRole
            };
            return ApiResponse<AuthResponseDto>.Success(authData, "Login successful");
        }
    }
}
