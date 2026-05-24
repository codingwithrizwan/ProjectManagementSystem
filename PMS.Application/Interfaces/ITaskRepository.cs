using PMS.Application.DTOs;
using PMS.Domain.Entities;
using System.Linq;

namespace PMS.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetTasksAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<TaskDto?> GetTaskById(Guid userId, long taskId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskDto>> GetAllTasksByProjectIdAsync(long Id, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
