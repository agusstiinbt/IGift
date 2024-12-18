using FluentValidation;
using MediatR;

namespace IGift.Infrastructure.Services.Validators
{
    /// <summary>
    /// El objetivo principal de esta clase es validar una solicitud antes de que sea procesada por su manejador correspondiente
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Si hay validadores asociados a la solicitud actual
            if (_validators.Any())
            {
                // Ejecuta todas las validaciones definidas para la solicitud
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults
                    .SelectMany(result => result.Errors)
                    .Where(f => f != null)
                    .ToList();

                // Si hay errores, lanza una excepción
                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }

            // Si todo está correcto, pasa al siguiente componente en la tubería
            return await next();
        }
    }
}

//TODO importante: esta clase evita que si usamos fluentvalidation evitemos crear validaciones personalizadas en cada clase cqrs: Utilizar el siguiente ejemplo y luego construir el codigo para que ya quede hecho:

//Codigo de Ejemplo: ESTO ES LO QUE SE DEBE EVITAR EN LA LINEA 57

//    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
//{
//    private readonly IValidator<GetProductQuery> _validator;

//    public GetProductQueryHandler(IValidator<GetProductQuery> validator)
//    {
//        _validator = validator;
//    }

//    public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
//    {
//        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
//        if (!validationResult.IsValid)
//            throw new ValidationException(validationResult.Errors);

//        // Continuar con el procesamiento...
//        return new ProductDto();
//    }
//}


//Ejemplo bien hecho:
//public class GetProductQuery : IRequest<ProductDto>
//{
//    public int ProductId { get; set; }
//}

//// Validador para GetProductQuery
//public class GetProductQueryValidator : AbstractValidator<GetProductQuery>
//{
//    public GetProductQueryValidator()
//    {
//        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("El ProductId debe ser mayor que cero.");
//    }
//}