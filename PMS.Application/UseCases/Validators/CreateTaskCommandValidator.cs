using FluentValidation;
using PMS.Application.UseCases.Commands;

namespace PMS.Application.UseCases.Validators
{
    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {
            RuleFor(x => x.Dto).NotNull().WithMessage("Request body is required.");

            RuleFor(x => x.Dto.ProjectId)
                .GreaterThan(0).WithMessage("Valid project id is required.");

            RuleFor(x => x.Dto.AssignedEmployeeId)
                .GreaterThan(0).WithMessage("Valid assigned employee id is required.");

            RuleFor(x => x.Dto.Title)
                .NotEmpty().WithMessage("Task title is required.");
               
        }
    }
}