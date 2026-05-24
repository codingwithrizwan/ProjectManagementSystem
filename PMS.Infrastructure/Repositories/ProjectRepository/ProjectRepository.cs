using Microsoft.EntityFrameworkCore;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using PMS.Infrastructure.Persistence;

namespace PMS.Infrastructure.Repositories.ProjectRepository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _db;

        public ProjectRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
        {
            await _db.Projects.AddAsync(project, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var _projectName = name.Trim().ToLower();
            return await _db.Projects.AnyAsync(
                p => p.Name.ToLower() == _projectName,
                cancellationToken);
        }

        public async Task<Project?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _db.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Projects.AsNoTracking()
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
