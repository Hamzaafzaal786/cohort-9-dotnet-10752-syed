using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.DTOs.Task
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public global::TaskManagementSystem.Domain.Enums.TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Category { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }
}