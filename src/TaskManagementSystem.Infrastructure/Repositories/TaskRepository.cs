using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Interfaces;
using TaskManagementSystem.Infrastructure.Data;
// Use alias to avoid ambiguity with System.Threading.Tasks.Task
using TaskEntity = TaskManagementSystem.Domain.Entities.Task;

namespace TaskManagementSystem.Infrastructure.Repositories
{
    public interface ITaskRepository : IBaseRepository<TaskEntity>
    {
        Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(string userId);
        // Use FULL path for TaskStatus to avoid ambiguity
        Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(string userId, global::TaskManagementSystem.Domain.Enums.TaskStatus status);
        Task<IEnumerable<TaskEntity>> GetTasksWithUserAsync();
        Task<TaskEntity?> GetTaskWithUserAsync(int id);
        Task<int> CountTasksByStatusAsync(string userId, global::TaskManagementSystem.Domain.Enums.TaskStatus status);
        Task<IEnumerable<TaskEntity>> GetOverdueTasksAsync(string userId);
    }

    public class TaskRepository : BaseRepository<TaskEntity>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(string userId, global::TaskManagementSystem.Domain.Enums.TaskStatus status)
        {
            return await _dbSet
                .Where(t => t.UserId == userId && t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksWithUserAsync()
        {
            return await _dbSet
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TaskEntity?> GetTaskWithUserAsync(int id)
        {
            return await _dbSet
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> CountTasksByStatusAsync(string userId, global::TaskManagementSystem.Domain.Enums.TaskStatus status)
        {
            return await _dbSet
                .CountAsync(t => t.UserId == userId && t.Status == status);
        }

        public async Task<IEnumerable<TaskEntity>> GetOverdueTasksAsync(string userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId &&
                           t.DueDate < DateTime.UtcNow &&
                           t.Status != global::TaskManagementSystem.Domain.Enums.TaskStatus.Completed)
                .ToListAsync();
        }
    }
}