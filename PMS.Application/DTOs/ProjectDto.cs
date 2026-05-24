using System.Collections.Generic;

namespace PMS.Application.DTOs
{
    public class ProjectTaskDeatailsDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public double Progress { get; set; }
        public List<TaskDto> Tasks { get; set; } = new();
    }
}
