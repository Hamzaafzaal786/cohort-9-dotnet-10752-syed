namespace TaskManagementSystem.Application.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}