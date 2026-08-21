using AutoMapper;
using TaskManagementSystem.Application.DTOs.Task;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Interfaces;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            var task = _mapper.Map<Domain.Entities.Task>(createTaskDto);

            var createdTask = await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TaskDto>(createdTask);
        }

        public async Task<bool> DeleteTaskAsync(int id, string userId, bool isAdmin)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
                return false;

            if (!isAdmin && task.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this task.");

            await _unitOfWork.Tasks.DeleteAsync(task);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync(string userId, bool isAdmin)
        {
            IEnumerable<Domain.Entities.Task> tasks;

            if (isAdmin)
            {
                tasks = await _unitOfWork.Tasks.GetTasksWithUserAsync();
            }
            else
            {
                tasks = await _unitOfWork.Tasks.GetTasksByUserIdAsync(userId);
            }

            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId, bool isAdmin)
        {
            IEnumerable<Domain.Entities.Task> tasks;

            if (isAdmin)
            {
                tasks = await _unitOfWork.Tasks.GetAllAsync();
            }
            else
            {
                tasks = await _unitOfWork.Tasks.GetTasksByUserIdAsync(userId);
            }

            var taskList = tasks.ToList();
            var now = DateTime.UtcNow;

            return new DashboardStatsDto
            {
                TotalTasks = taskList.Count,
                CompletedTasks = taskList.Count(t => t.Status == global::TaskManagementSystem.Domain.Enums.TaskStatus.Completed),
                InProgressTasks = taskList.Count(t => t.Status == global::TaskManagementSystem.Domain.Enums.TaskStatus.InProgress),
                PendingTasks = taskList.Count(t => t.Status == global::TaskManagementSystem.Domain.Enums.TaskStatus.Pending),
                OverdueTasks = taskList.Count(t => t.DueDate < now && t.Status != global::TaskManagementSystem.Domain.Enums.TaskStatus.Completed)
            };
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int id, string userId, bool isAdmin)
        {
            Domain.Entities.Task? task;

            if (isAdmin)
            {
                task = await _unitOfWork.Tasks.GetTaskWithUserAsync(id);
            }
            else
            {
                task = await _unitOfWork.Tasks.GetByIdAsync(id);
                if (task != null && task.UserId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to view this task.");
            }

            return task == null ? null : _mapper.Map<TaskDto>(task);
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByStatusAsync(string userId, global::TaskManagementSystem.Domain.Enums.TaskStatus status, bool isAdmin)
        {
            IEnumerable<Domain.Entities.Task> tasks;

            if (isAdmin)
            {
                var allTasks = await _unitOfWork.Tasks.GetAllAsync();
                tasks = allTasks.Where(t => t.Status == status);
            }
            else
            {
                tasks = await _unitOfWork.Tasks.GetTasksByStatusAsync(userId, status);
            }

            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<TaskDto> UpdateTaskAsync(UpdateTaskDto updateTaskDto)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(updateTaskDto.Id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {updateTaskDto.Id} not found.");

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Status = updateTaskDto.Status;
            task.Priority = updateTaskDto.Priority;
            task.DueDate = updateTaskDto.DueDate;
            task.Category = updateTaskDto.Category;
            task.UserId = updateTaskDto.UserId ?? task.UserId;
            task.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Tasks.UpdateAsync(task);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TaskDto>(task);
        }
    }
}