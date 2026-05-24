using FluentValidation;
using MediatR;
using PMS.Application.DTOs;

namespace PMS.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var errors = results
                .Where(r => !r.IsValid)
                .SelectMany(r => r.Errors)
                .Select(e => e.ErrorMessage)
                .Distinct()
                .ToList();

            if (errors.Count == 0)
            {
                return await next();
            }

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                var response = Activator.CreateInstance(typeof(TResponse));
                if (response is null)
                {
                    throw new InvalidOperationException("Could not create validation response.");
                }

                var messageProp = typeof(TResponse).GetProperty(nameof(ApiResponse<object>.Message));
                var successProp = typeof(TResponse).GetProperty(nameof(ApiResponse<object>.IsSuccess));
                var errorsProp = typeof(TResponse).GetProperty(nameof(ApiResponse<object>.Errors));

                messageProp?.SetValue(response, "Validation failed");
                successProp?.SetValue(response, false);
                errorsProp?.SetValue(response, errors);

                return (TResponse)response;
            }

            throw new ValidationException(errors.Select(e => new FluentValidation.Results.ValidationFailure(string.Empty, e)));
        }
    }
}