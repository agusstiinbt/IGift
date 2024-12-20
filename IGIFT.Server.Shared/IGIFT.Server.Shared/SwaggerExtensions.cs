using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace IGIFT.Server.Shared
{
    /// <summary>
    /// Esta extensión ayudará a estandarizar la configuración básica de Swagger en cada servidor.
    /// </summary>
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, string serviceName)
        {
            services.AddSwaggerGen(c =>
            {
                //TODO agregar el siguiente codigo en todos los microservicios dentro del propertyGroup para que esto funcione:
                //TODO de pasoo reemplazar todos los server por servers 6.0
                //< GenerateDocumentationFile > true </ GenerateDocumentationFile >
                //< NoWarn >$(NoWarn); 1591 </ NoWarn >


                // Incluir comentarios XML de todos los ensamblados
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        var xmlFile = $"{assembly.GetName().Name}.xml";
                        var xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            c.IncludeXmlComments(xmlPath);
                        }
                    }
                }

                // Documento unificado para la API Gateway
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API Gateway - Centralized Swagger",
                    Description = "This Swagger UI provides a unified view of all endpoints available through the API Gateway.",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Definición de seguridad para autenticación JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });

                // Requisito de seguridad para todos los endpoints
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });

            return services;
        }
    }
}
