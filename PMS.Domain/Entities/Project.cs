using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PMS.Domain.Entities
{
    public class Project
    {
        [Key]
        public long Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private readonly List<TaskItem> _tasks = new();
        public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

        // EF ctor
        private Project() { }

        public Project(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Project name is required.");
            }

            Name = name.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            CreatedAt = DateTime.UtcNow;
        }

        public TaskItem AddTask(string title, long assignedEmployeeId, string? description = null)
        {
            var t = new TaskItem(title, assignedEmployeeId, description);
            t.AttachToProject(Id);
            _tasks.Add(t);
            return t;
        }

        // Domain logic: project progress = completed tasks / total tasks
        public double GetProgress()
        {
            if (!_tasks.Any()) return 0;
            var completed = _tasks.Count(t => t.Status == Enums.TaskStatus.Done);
            return (double)completed / _tasks.Count * 100.0;
        }
    }
}
