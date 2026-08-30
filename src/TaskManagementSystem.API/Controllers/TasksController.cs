using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using TaskManagementSystem.Application.DTOs.Task;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // ✅ FIXED: Gets UserId from token using "user_id" claim
        private string GetUserId()
        {
            // Try to get from "user_id" claim first
            var userId = User.FindFirst("user_id")?.Value;

            // If not found, try NameIdentifier (fallback)
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            return userId ?? string.Empty;
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();

                Log.Information("GetAllTasks - UserId: {UserId}, IsAdmin: {IsAdmin}", userId, isAdmin);

                var tasks = await _taskService.GetAllTasksAsync(userId, isAdmin);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting tasks");
                return StatusCode(500, new { message = "An error occurred while fetching tasks" });
            }
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                var task = await _taskService.GetTaskByIdAsync(id, userId, isAdmin);

                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while fetching the task" });
            }
        }

        // POST: api/tasks
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();

                // ✅ Non-admin users can ONLY create tasks for themselves
                if (!isAdmin)
                {
                    createTaskDto.UserId = userId;
                }
                // ✅ Admin can assign to others, or default to self
                else if (isAdmin && string.IsNullOrEmpty(createTaskDto.UserId))
                {
                    createTaskDto.UserId = userId;
                }

                var task = await _taskService.CreateTaskAsync(createTaskDto);
                Log.Information("Task {TaskId} created by user {UserId}", task.Id, userId);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating task");
                return StatusCode(500, new { message = "An error occurred while creating the task" });
            }
        }

        // PUT: api/tasks
        [HttpPut]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();

                // If not admin, ensure they can only update their own tasks
                if (!isAdmin)
                {
                    var existingTask = await _taskService.GetTaskByIdAsync(updateTaskDto.Id, userId, isAdmin);
                    if (existingTask == null)
                    {
                        return NotFound(new { message = "Task not found or you don't have permission" });
                    }
                }

                var task = await _taskService.UpdateTaskAsync(updateTaskDto);
                Log.Information("Task {TaskId} updated by user {UserId}", task.Id, userId);
                return Ok(task);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Task not found" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating task {TaskId}", updateTaskDto.Id);
                return StatusCode(500, new { message = "An error occurred while updating the task" });
            }
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                var result = await _taskService.DeleteTaskAsync(id, userId, isAdmin);

                if (!result)
                    return NotFound(new { message = "Task not found" });

                Log.Information("Task {TaskId} deleted by user {UserId}", id, userId);
                return Ok(new { message = "Task deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the task" });
            }
        }

        // GET: api/tasks/status/{status}
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetTasksByStatus(global::TaskManagementSystem.Domain.Enums.TaskStatus status)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                var tasks = await _taskService.GetTasksByStatusAsync(userId, status, isAdmin);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting tasks by status {Status}", status);
                return StatusCode(500, new { message = "An error occurred while fetching tasks" });
            }
        }

        // GET: api/tasks/dashboard/stats
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                var stats = await _taskService.GetDashboardStatsAsync(userId, isAdmin);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting dashboard stats");
                return StatusCode(500, new { message = "An error occurred while fetching dashboard stats" });
            }
        }
    }
}