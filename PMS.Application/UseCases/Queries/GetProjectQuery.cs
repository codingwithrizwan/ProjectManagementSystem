using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Queries
{
    public class GetAllTaskByProjectIdQuery : IRequest<ApiResponse<ProjectTaskDeatailsDto>>
    {
        public long ProjectId { get; }
        public Guid UserId { get; }
        public GetAllTaskByProjectIdQuery(
            long projectId)
        {
            ProjectId = projectId;

        }
    }
}
