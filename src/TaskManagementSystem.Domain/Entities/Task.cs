using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Domain.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Use FULL namespace to avoid ambiguity
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? Category { get; set; }

        // Foreign key and navigation property
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}