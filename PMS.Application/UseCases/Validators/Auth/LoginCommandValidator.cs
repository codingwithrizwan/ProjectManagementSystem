using FluentValidation;
using PMS.Application.UseCases.Commands.Auth;

namespace PMS.Application.UseCases.Validators.Auth
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Dto).NotNull().WithMessage("Request body is required.");

            RuleFor(x => x.Dto.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}