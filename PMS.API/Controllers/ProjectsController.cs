using PMS.Application.UseCases.Commands;
using PMS.Application.UseCases.Queries;

namespace PMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController(IMediator _mediator) : ControllerBase  // I used primary constructor
    {
        #region Admin End Points

        [Authorize(Policy = ApiPolicies.AdminOnly)]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            var result = await _mediator.Send(new CreateProjectCommand(dto));
            if (!result.IsSuccess || result.Data <= 0)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetProject), new { id = result.Data }, result);
        }

        [Authorize(Policy = ApiPolicies.AdminOnly)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(long id)
        {
            var result = await _mediator.Send(new GetAllTaskByProjectIdQuery(id));
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Policy = ApiPolicies.AdminOnly)]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var result = await _mediator.Send(new GetProjectsQuery());
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Policy = ApiPolicies.AdminOnly)]
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await _mediator.Send(new GetEmployeesQuery());
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Policy = ApiPolicies.AdminOnly)]
        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var result = await _mediator.Send(new CreateTaskCommand(dto));
            if (!result.IsSuccess || result.Data <= 0)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetProject), new { id = dto.ProjectId }, result);
        }

        #endregion

        #region Employee End Points

        [Authorize(Policy = ApiPolicies.EmployeeOnly)]
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            if (!User.TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid user context"));
            }

            var result = await _mediator.Send(new GetEmployeeAssignedTasksQuery(userId));
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Policy = ApiPolicies.EmployeeOnly)]
        [HttpGet("my-tasks/{taskId:long}")]
        public async Task<IActionResult> GetMyTaskById(long taskId)
        {
            if (!User.TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid user context"));
            }

            var result = await _mediator.Send(new GetEmployeeTaskByIdQuery(userId, taskId));
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [Authorize(Policy = ApiPolicies.EmployeeOnly)]
        [HttpPut("update-task-status")]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] UpdateTaskStatusDto dto)
        {
            var result = await _mediator.Send(new UpdateTaskStatusCommand(dto));
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        #endregion
    }
}
