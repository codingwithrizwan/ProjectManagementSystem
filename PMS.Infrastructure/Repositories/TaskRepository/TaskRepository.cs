using Microsoft.EntityFrameworkCore;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using PMS.Infrastructure.Persistence;

namespace PMS.Infrastructure.Repositories.TaskRepository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _db;

        public TaskRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            await _db.Tasks.AddAsync(task, cancellationToken);
        }
        public async Task<TaskItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskDto>> GetTasksAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var _allTasksQuery = (from task in _db.Tasks.AsNoTracking()
                                 join project in _db.Projects.AsNoTracking()
                                 on task.ProjectId equals project.Id
                                 join employee in _db.Employees.AsNoTracking()
                                 on task.AssignedEmployeeId equals employee.Id
                                 where employee.UserId==userId
                                 orderby task.Status, task.CreatedAt descending
                                 select new TaskDto
                                 {
                                     Id = task.Id,
                                     Title = task.Title,
                                     Status = task.Status,
                                     AssignedEmployeeId = employee.Id,
                                     AssignedEmployeeName = employee.Name,
                                     ProjectId = task.ProjectId,
                                     ProjectName = project.Name,
                                     CreatedAt = task.CreatedAt,
                                     StartedAt = task.StartedAt,
                                     CompletedAt = task.CompletedAt
                                 });

            return await _allTasksQuery.ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<TaskDto>> GetAllTasksByProjectIdAsync(long Id, CancellationToken cancellationToken = default)
        {
            var _allTasksQuery = (from task in _db.Tasks.AsNoTracking()
                                  join project in _db.Projects.AsNoTracking()
                                  on task.ProjectId equals project.Id
                                  join employee in _db.Employees.AsNoTracking()
                                  on task.AssignedEmployeeId equals employee.Id
                                  where project.Id == Id
                                  orderby task.Status, task.CreatedAt descending
                                  select new TaskDto
                                  {
                                      Id = task.Id,
                                      Title = task.Title,
                                      Status = task.Status,
                                      AssignedEmployeeId = employee.Id,
                                      AssignedEmployeeName = employee.Name,
                                      ProjectId = task.ProjectId,
                                      ProjectName = project.Name,
                                      CreatedAt = task.CreatedAt,
                                      StartedAt = task.StartedAt,
                                      CompletedAt = task.CompletedAt
                                  });

            return await _allTasksQuery.ToListAsync(cancellationToken);
        }

        public async Task<TaskDto?> GetTaskById(Guid userId, long taskId, CancellationToken cancellationToken = default)
        {
            var _SingleTaskQuery = from task in _db.Tasks.AsNoTracking()
                                 join project in _db.Projects.AsNoTracking()
                                 on task.ProjectId equals project.Id
                                 join employee in _db.Employees.AsNoTracking()
                                 on task.AssignedEmployeeId equals employee.Id
                                 where employee.UserId == userId && task.Id== taskId
                                 select new TaskDto
                                 {
                                     Id = task.Id,
                                     Title = task.Title,
                                     Status = task.Status,
                                     AssignedEmployeeId = task.AssignedEmployeeId,
                                     AssignedEmployeeName = employee.Name,
                                     ProjectId = task.ProjectId,
                                     ProjectName = project.Name,
                                     CreatedAt = task.CreatedAt,
                                     StartedAt = task.StartedAt,
                                     CompletedAt = task.CompletedAt
                                 };

            return await _SingleTaskQuery.FirstOrDefaultAsync(cancellationToken);
        }

       
    }
}
