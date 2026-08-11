using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository Tasks { get; }
        IBaseRepository<User> Users { get; }
        Task<int> CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITaskRepository? _taskRepository;
        private IBaseRepository<User>? _userRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITaskRepository Tasks =>
            _taskRepository ??= new TaskRepository(_context);

        public IBaseRepository<User> Users =>
            _userRepository ??= new BaseRepository<User>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}