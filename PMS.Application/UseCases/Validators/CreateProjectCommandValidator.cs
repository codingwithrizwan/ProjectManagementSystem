using FluentValidation;
using PMS.Application.UseCases.Commands;

namespace PMS.Application.UseCases.Validators
{
    public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectCommandValidator()
        {
            RuleFor(x => x.Dto).NotNull().WithMessage("Request body is required.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Project name is required.");
                


        }
    }
}