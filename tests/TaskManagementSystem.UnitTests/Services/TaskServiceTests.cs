using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using TaskManagementSystem.Application.DTOs.Task;
using TaskManagementSystem.Application.Mappings;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Interfaces;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Repositories;
using Xunit;

namespace TaskManagementSystem.UnitTests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly IMapper _mapper;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _taskRepositoryMock = new Mock<ITaskRepository>();

            _unitOfWorkMock.Setup(u => u.Tasks).Returns(_taskRepositoryMock.Object);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _taskService = new TaskService(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task DeleteTaskEntityAsync_ShouldReturnFalse_WhenTaskNotFound()
        {
            _taskRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((TaskEntity)null);

            var result = await _taskService.DeleteTaskAsync(1, "user123", false);

            Assert.False(result);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetAllTaskEntitiesAsync_ShouldReturnTaskEntities_ForRegularUser()
        {
            var userId = "user123";
            var tasks = new List<TaskEntity>
            {
                new TaskEntity { Id = 1, Title = "Task 1", UserId = userId },
                new TaskEntity { Id = 2, Title = "Task 2", UserId = userId }
            };

            _taskRepositoryMock.Setup(r => r.GetTasksByUserIdAsync(userId))
                .ReturnsAsync(tasks);

            var result = await _taskService.GetAllTasksAsync(userId, false);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetTaskEntitiesByStatusAsync_ShouldReturnFilteredTaskEntities()
        {
            var userId = "user123";
            // ✅ Use FULL namespace to avoid ambiguity
            var status = global::TaskManagementSystem.Domain.Enums.TaskStatus.Completed;
            var tasks = new List<TaskEntity>
            {
                new TaskEntity { Id = 1, Title = "Completed Task", Status = status, UserId = userId }
            };

            _taskRepositoryMock.Setup(r => r.GetTasksByStatusAsync(userId, status))
                .ReturnsAsync(tasks);

            var result = await _taskService.GetTasksByStatusAsync(userId, status, false);

            Assert.Single(result);
            Assert.Equal(global::TaskManagementSystem.Domain.Enums.TaskStatus.Completed, result.First().Status);
        }
    }
}