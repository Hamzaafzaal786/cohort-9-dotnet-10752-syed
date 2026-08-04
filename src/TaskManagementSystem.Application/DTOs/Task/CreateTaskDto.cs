using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.DTOs.Task
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime DueDate { get; set; }
        public string? Category { get; set; }
        public string? UserId { get; set; }
    }
}