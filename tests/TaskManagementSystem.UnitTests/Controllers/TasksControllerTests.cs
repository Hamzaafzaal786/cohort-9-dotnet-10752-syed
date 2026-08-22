using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.API.Controllers;
using TaskManagementSystem.Application.DTOs.Task;
using TaskManagementSystem.Application.Interfaces;
using Xunit;

namespace TaskManagementSystem.UnitTests.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _controller = new TasksController(_taskServiceMock.Object);
        }

        // ✅ FIXED: Uses "user_id" to match the controller
        private void SetupUserContext(string userId, bool isAdmin = false)
        {
            var claims = new List<Claim>
            {
                new Claim("user_id", userId)  // ✅ Must match controller's GetUserId()
            };

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetAllTasks_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var tasks = new List<TaskDto>
            {
                new TaskDto { Id = 1, Title = "Task 1" },
                new TaskDto { Id = 2, Title = "Task 2" }
            };

            _taskServiceMock.Setup(s => s.GetAllTasksAsync(userId, false))
                .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAllTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);
            Assert.Equal(2, returnedTasks.Count());
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetTaskById_ShouldReturnOk_WhenTaskExists()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var task = new TaskDto { Id = 1, Title = "Task 1" };

            _taskServiceMock.Setup(s => s.GetTaskByIdAsync(1, userId, false))
                .ReturnsAsync(task);

            // Act
            var result = await _controller.GetTaskById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTask = Assert.IsType<TaskDto>(okResult.Value);
            Assert.Equal(1, returnedTask.Id);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task CreateTask_ShouldReturnCreated_WhenTaskIsCreated()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            var createDto = new CreateTaskDto
            {
                Title = "New Task",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            var createdTask = new TaskDto { Id = 1, Title = "New Task" };

            _taskServiceMock.Setup(s => s.CreateTaskAsync(createDto))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _controller.CreateTask(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, createdResult.RouteValues["id"]);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task DeleteTask_ShouldReturnOk_WhenTaskIsDeleted()
        {
            // Arrange
            var userId = "user123";
            SetupUserContext(userId);

            _taskServiceMock.Setup(s => s.DeleteTaskAsync(1, userId, false))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTask(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
    }
}