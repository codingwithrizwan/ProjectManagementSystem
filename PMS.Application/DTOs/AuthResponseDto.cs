namespace PMS.Application.DTOs
{
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
