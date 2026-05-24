using PMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using TaskStatus = PMS.Domain.Enums.TaskStatus;

namespace PMS.Domain.Entities
{
    public class TaskItem
    {
        [Key]
        public long Id { get; private set; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public TaskStatus Status { get; private set; } = TaskStatus.ToDo;
        public long AssignedEmployeeId { get; private set; }
        public long ProjectId { get; private set; }
        public Project? Project { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        // EF Core ctor
        private TaskItem() { }

        public TaskItem(string title, long assignedEmployeeId, string? description = null)
        {
            Title = title;
            AssignedEmployeeId = assignedEmployeeId;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            Status = TaskStatus.ToDo;
        }

        internal void AttachToProject(long projectId)
        {
            ProjectId = projectId;
        }

        public void Start()
        {
            if (Status == TaskStatus.InProgress) return;
            Status = TaskStatus.InProgress;
            StartedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status == TaskStatus.Done) return;
            Status = TaskStatus.Done;
            CompletedAt = DateTime.UtcNow;
            if (!StartedAt.HasValue) StartedAt = CompletedAt;
        }

        public void UpdateStatus(TaskStatus newStatus)
        {
            if (newStatus == Status) return;
            switch (newStatus)
            {
                case TaskStatus.InProgress:
                    Start();
                    break;
                case TaskStatus.Done:
                    Complete();
                    break;
                default:
                    Status = newStatus;
                    break;
            }
        }
    }
}
