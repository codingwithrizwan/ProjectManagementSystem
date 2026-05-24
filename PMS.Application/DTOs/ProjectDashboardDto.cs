namespace PMS.Application.DTOs
{
    public class ProjectDashboardDto
    {
        public int TotalProjects { get; set; }
        public int OverallAverageProgress { get; set; }
        public List<ProjectListItemDto> Projects { get; set; } = [];
    }
}