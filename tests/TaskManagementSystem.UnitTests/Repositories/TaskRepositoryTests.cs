using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Repositories;
using Xunit;

namespace TaskManagementSystem.UnitTests.Repositories
{
    public class TaskRepositoryTests
    {
        [Fact]
        public void Test_SimplePass()
        {
            Assert.Equal(5, 2 + 3);
        }

        [Fact]
        public void Test_SimplePass_String()
        {
            Assert.Equal("Hello", "Hello");
        }

        [Fact]
        public async global::System.Threading.Tasks.Task AddAsync_ShouldAddTaskEntity_ToDatabase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var repository = new TaskRepository(context);

            var task = new TaskEntity
            {
                Title = "Test Task",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            var result = await repository.AddAsync(task);

            Assert.NotEqual(0, result.Id);
            Assert.Equal("Test Task", result.Title);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetByIdAsync_ShouldReturnTaskEntity_WhenExists()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var repository = new TaskRepository(context);

            var task = new TaskEntity
            {
                Title = "Existing Task",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            await repository.AddAsync(task);

            var result = await repository.GetByIdAsync(task.Id);

            Assert.NotNull(result);
            Assert.Equal("Existing Task", result.Title);
        }

        [Fact]
        public async global::System.Threading.Tasks.Task GetTasksByUserIdAsync_ShouldReturnUserTaskEntities()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var repository = new TaskRepository(context);

            var userId = "user123";

            var task1 = new TaskEntity { Title = "User Task 1", UserId = userId, DueDate = DateTime.UtcNow.AddDays(7) };
            var task2 = new TaskEntity { Title = "User Task 2", UserId = userId, DueDate = DateTime.UtcNow.AddDays(7) };
            var task3 = new TaskEntity { Title = "Other User Task", UserId = "other", DueDate = DateTime.UtcNow.AddDays(7) };

            await repository.AddAsync(task1);
            await repository.AddAsync(task2);
            await repository.AddAsync(task3);

            var result = await repository.GetTasksByUserIdAsync(userId);

            Assert.Equal(2, result.Count());
            foreach (var task in result)
            {
                Assert.Equal(userId, task.UserId);
            }
        }
    }
}