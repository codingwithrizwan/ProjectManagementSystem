using Microsoft.EntityFrameworkCore;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using PMS.Infrastructure.Persistence;

namespace PMS.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _db;

        public EmployeeRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            await _db.Employees.AddAsync(employee, cancellationToken);
        }

        public async Task<Employee?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _db.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.Employees.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
        }

        public async Task<IEnumerable<Employee>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Employees.ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
