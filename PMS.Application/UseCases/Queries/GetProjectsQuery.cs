using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Queries
{
    public record GetProjectsQuery : IRequest<ApiResponse<ProjectDashboardDto>>;
}