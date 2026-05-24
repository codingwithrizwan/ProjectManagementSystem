using MediatR;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Application.UseCases.Queries;

namespace PMS.Application.UseCases.Handlers
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, ApiResponse<IReadOnlyList<EmployeeDto>>>
    {
        private readonly IEmployeeRepository _employees;

        public GetEmployeesQueryHandler(IEmployeeRepository employees)
        {
            _employees = employees;
        }

        public async Task<ApiResponse<IReadOnlyList<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var list = await _employees.ListAsync(cancellationToken);
            var employees = list
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Email = e.Email
                })
                .OrderBy(e => e.Name)
                .ToList();

            return ApiResponse<IReadOnlyList<EmployeeDto>>.Success(employees, "Employees fetched successfully");
        }
    }
}
