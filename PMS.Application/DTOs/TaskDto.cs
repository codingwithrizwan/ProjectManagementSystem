namespace PMS.Application.DTOs
{
    public class TaskDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public PMS.Domain.Enums.TaskStatus Status { get; set; }
        public long AssignedEmployeeId { get; set; }
        public string AssignedEmployeeName { get; set; } = string.Empty;
        public long ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
