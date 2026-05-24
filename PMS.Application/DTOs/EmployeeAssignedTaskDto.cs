namespace PMS.Application.DTOs
{
    public class EmployeeAssignedTaskDto
    {
        public long TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public PMS.Domain.Enums.TaskStatus Status { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}