using PMS.Domain.Entities;

namespace PMS.Domain.Services
{
    // calculation will be in domain layer as per requirement.
    public static class ProjectProgressService
    {
        public static double CalculateProjectProgress(Project project)
        {
            return project.GetProgress();
        }

        public static double CalculateOverallAverageProgress(IEnumerable<Project> projects)
        {
            var projectList = projects.ToList();
            if (projectList.Count == 0)
            {
                return 0;
            }

            return projectList.Average(p => p.GetProgress());
        }
    }
}