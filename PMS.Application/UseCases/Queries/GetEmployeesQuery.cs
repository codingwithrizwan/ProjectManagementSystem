using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.UseCases.Queries
{
    public record GetEmployeesQuery : IRequest<ApiResponse<IReadOnlyList<EmployeeDto>>>;
}
