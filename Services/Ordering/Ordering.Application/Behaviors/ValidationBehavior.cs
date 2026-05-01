using FluentValidation;
using MediatR;

namespace Ordering.Application.Behaviors
{
    //will collect the fluent validation rules and execute them before the request handler is called.
    //If any validation rules fail, it will throw a ValidationException with the details of the validation errors.
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any()) 
            {
                var context = new ValidationContext<TRequest>(request);
                //will run all the validators one by one and returns the validation result
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                if (validationResults.Any())
                {
                    //now check for any failures
                    var failures = validationResults.SelectMany(validationResults => validationResults.Errors)
                        .Where(f => f != null)
                        .ToList();
                    if (failures.Count != 0)
                    {
                        throw new ValidationException(failures);
                    }
                }
            }
            return await next();
        }
    }
}