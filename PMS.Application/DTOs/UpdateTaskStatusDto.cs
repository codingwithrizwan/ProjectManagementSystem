namespace PMS.Application.DTOs
{
    public class UpdateTaskStatusDto
    {
        public long TaskId { get; set; }
        public PMS.Domain.Enums.TaskStatus Status { get; set; }
    }
}
