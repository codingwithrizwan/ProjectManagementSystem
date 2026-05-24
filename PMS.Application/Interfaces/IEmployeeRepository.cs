using PMS.Domain.Entities;

namespace PMS.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Employee>> ListAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
