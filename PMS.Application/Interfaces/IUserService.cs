using PMS.Application.DTOs;

namespace PMS.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserInfoDto?> CreateUserAsync(string userName, string email, string password, string? role = null);
        Task<UserInfoDto?> ValidateUserAsync(string userName, string password);
    }
}
