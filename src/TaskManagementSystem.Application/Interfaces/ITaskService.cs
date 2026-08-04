using TaskManagementSystem.Application.DTOs.Task;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllTasksAsync(string userId, bool isAdmin);
        Task<TaskDto?> GetTaskByIdAsync(int id, string userId, bool isAdmin);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<TaskDto> UpdateTaskAsync(UpdateTaskDto updateTaskDto);
        Task<bool> DeleteTaskAsync(int id, string userId, bool isAdmin);

        Task<IEnumerable<TaskDto>> GetTasksByStatusAsync(
            string userId,
            global::TaskManagementSystem.Domain.Enums.TaskStatus status,
            bool isAdmin);

        Task<DashboardStatsDto> GetDashboardStatsAsync(string userId, bool isAdmin);
    }
}