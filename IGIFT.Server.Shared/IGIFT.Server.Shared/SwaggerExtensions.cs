using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace IGIFT.Server.Shared
{
    /// <summary>
    /// Esta extensión ayudará a estandarizar la configuración básica de Swagger en cada servidor.
    /// </summary>
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerForMicroservice(this IServiceCollection services, string serviceName)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"{serviceName} API",
                    Version = "v1",
                    Description = $"API documentation for the {serviceName} microservice.",
                    Contact = new OpenApiContact
                    {
                        Name = "IGift Team",
                        Email = "support@igift.com"
                    }
                });

                // Incluye comentarios XML (si existen)
                var xmlFile = $"{serviceName}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Configuración de seguridad para JWT (si aplica)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer {your token}' to authenticate."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "Bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
            });

            return services;
        }
    }

}
