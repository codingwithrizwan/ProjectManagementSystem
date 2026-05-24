using PMS.Domain.Entities;
using System.Linq;

namespace PMS.Application.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task AddAsync(Project project, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
