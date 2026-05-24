namespace PMS.Application.DTOs
{
    public class ProjectListItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TaskCount { get; set; }
        public double Progress { get; set; }
    }
}