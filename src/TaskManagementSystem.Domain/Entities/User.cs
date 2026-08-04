using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Domain.Entities
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation property - one user has many tasks
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}