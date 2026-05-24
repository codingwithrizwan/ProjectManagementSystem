using FluentValidation;
using PMS.Application.UseCases.Commands;

namespace PMS.Application.UseCases.Validators
{
    public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
    {
        public UpdateTaskStatusCommandValidator()
        {
            RuleFor(x => x.Dto).NotNull().WithMessage("Request body is required.");

            RuleFor(x => x.Dto.TaskId)
                .GreaterThan(0).WithMessage("Valid task id is required.");

            RuleFor(x => x.Dto.Status)
                .IsInEnum().WithMessage("Valid task status is required.");
        }
    }
}