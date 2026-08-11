using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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

        private string GetUserId() => User.FindFirst("UserId")?.Value ?? string.Empty;
        private bool IsAdmin() => User.IsInRole("Admin");

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();
                var tasks = await _taskService.GetAllTasksAsync(userId, isAdmin);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting tasks");
                return StatusCode(500, new { message = "An error occurred while fetching tasks" });
            }
        }

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
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while fetching the task" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                var task = await _taskService.CreateTaskAsync(createTaskDto);
                Log.Information("Task {TaskId} created by user {UserId}", task.Id, task.UserId);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating task");
                return StatusCode(500, new { message = "An error occurred while creating the task" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                var task = await _taskService.UpdateTaskAsync(updateTaskDto);
                Log.Information("Task {TaskId} updated", task.Id);
                return Ok(task);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Task not found" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating task {TaskId}", updateTaskDto.Id);
                return StatusCode(500, new { message = "An error occurred while updating the task" });
            }
        }

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

                Log.Information("Task {TaskId} deleted", id);
                return Ok(new { message = "Task deleted successfully" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the task" });
            }
        }

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