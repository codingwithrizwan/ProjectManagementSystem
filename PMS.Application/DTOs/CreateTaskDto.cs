namespace PMS.Application.DTOs
{
    public class CreateTaskDto
    {
        public long ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public long AssignedEmployeeId { get; set; }
    }
}
