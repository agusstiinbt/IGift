using FluentValidation.AspNetCore;
using IGift.Application.OptionsPattern;

namespace IGift.Server
{
    internal static class MvcBuilderExtensions
    {
        internal static IMvcBuilder AddValidators(this IMvcBuilder builder)
        {
            //RegisterValidatorsFromAssemblyContaining<T>: Busca todas las clases dentro del ensamblado que implementen IValidator<T>
            builder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AppConfiguration>());
            return builder;
        }
    }
}
