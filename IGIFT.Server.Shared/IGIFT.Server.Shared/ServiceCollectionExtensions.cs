﻿using System.Net;
using System.Text.Json.Serialization;
using IGift.Application.AppConfiguration;
using IGift.Application.Interfaces;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Interfaces.Serialization.Options;
using IGift.Application.Interfaces.Serialization.Settings;
using IGift.Application.Serialization;
using IGift.Application.Serialization.JsonConverters;
using IGift.Application.Serialization.Serializers;
using IGift.Application.Serialization.Settings;
using IGift.Infrastructure;
using IGift.Infrastructure.Data;
using IGift.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using IGift.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using IGift.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IGift.Shared;
using System.Text;

namespace IGIFT.Server.Shared
{
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// AddForwarding es una configuración que permite que la aplicación trabaje correctamente detrás de un proxy (como un balanceador de carga o una API Gateway), que es común en arquitecturas de microservicios. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IServiceCollection AddForwarding(this IServiceCollection services, IConfiguration configuration)
        {
            //ServiceName es una clave que puedes agregar en el archivo appsettings.json de cada microservicio para identificarlo de manera única.
            //          Ejemplo:
            //          {
            //              "ServiceName": "MicroservicioUno",
            //              "AppConfiguration": {
            //              "Secret": "someSecret",
            //              "BehindSSLProxy": true,
            //              "ProxyIP": "192.168.0.1",
            //              "ApplicationUrl": "https://microserviciouno.com"
            //          },
            var serviceName = configuration.GetValue<string>("ServiceName"); // Variable que identifica cada microservicio



            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));

            var config = applicationSettingsConfiguration.Get<AppConfiguration>();

            if (config!.BehindSSLProxy)
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {

                    #region Resumen de la linea siguiente
                    // Esta línea indica a ForwardedHeadersOptions que use los encabezados X-Forwarded - For y X-Forwarded - Proto para tomar decisiones sobre la dirección IP y el esquema del cliente. Estos encabezados son comunes cuando una aplicación está detrás de un proxy o balanceador de carga.

                    //X - Forwarded - For: Muestra la IP original del cliente, lo que permite a la aplicación registrar la dirección IP real del cliente en lugar de la del proxy.

                    //X - Forwarded - Proto: Indica el protocolo utilizado por el cliente(http o https).Así, la aplicación puede identificar si la solicitud se realizó por HTTP o HTTPS, aun si el proxy realiza cambios en el protocolo.

                    //Esto es esencial en microservicios y en aplicaciones distribuidas que pueden estar detrás de proxies y balanceadores de carga, ya que permite a cada microservicio trabajar como si estuviera directamente expuesto al cliente final, con una mayor precisión en la detección de IPs y el esquema original.
                    #endregion
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    if (!string.IsNullOrEmpty(config.ProxyIP))
                    {
                        var ipCheck = config.ProxyIP;
                        if (IPAddress.TryParse(ipCheck, out var proxyIP))
                            options.KnownProxies.Add(proxyIP);
                        else
                            Log.Logger.Warning("Proxy IP invalida por {IpCheck}, Not Loaded", ipCheck);
                    }
                });

                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .WithOrigins(config.ApplicationUrl.TrimEnd('/'));

                            // Configuración específica según el servicio
                            if (serviceName == "NombreDelMicroService")
                            {
                                builder.WithMethods("GET", "POST"); // Excluye PUT
                            }
                            else
                            {
                                builder.AllowAnyMethod();
                            }

                        });
                });
            };
            return services;
        }

        /// <summary>
        /// Configura interfaces para que se puedan usar implmemtancion de System.Text.Json o Newtonsoft.Json 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IServiceCollection AddSerialization(this IServiceCollection services, IConfiguration configuration)
        {
            // Obtener el nombre del servicio. Permite identificar y aplicar reglas particulares por microservicio.
            var serviceName = configuration["ServiceName"];

            // Configura System.Text.Json para manejar serialización y deserialización, incluyendo soporte para tipos personalizados como TimeSpan.
            services.AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>().Configure<SystemTextJsonOptions>(options =>
            {

                //TimeSpan es algo que no esta soportado por la libreria text.json por eso es que se crea timespanjsonconverter
                if (!options.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                    options.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());

                #region Configuración específica por servicio
                //Posibles escenarios donde esto es útil:
                //Un microservicio puede requerir serializar propiedades adicionales o excluir campos que otros no necesitan.
                //Algunos servicios pueden necesitar manejar formatos personalizados(por ejemplo, un formato ISO específico para fechas)
                if (serviceName == "ServiceA")
                {
                    // Ejemplo de Personalización para ServiceA
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                }
                else if (serviceName == "ServiceB")
                {
                    // Ejemplo de Personalización para ServiceB
                    options.JsonSerializerOptions.WriteIndented = true;
                }
                #endregion

            });

            // Configura Newtonsoft.Json para proporcionar flexibilidad en la configuración de serialización si es necesario.
            services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

            // Registro del serializador principal
            services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); //Puede cambiarse en el caso de utilizar un microservice u otro.

            return services;
        }

        internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceName = configuration["ServiceName"];

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // Obtener la cadena de conexión basada en el servicio
                var connectionStringKey = $"{serviceName}_ConnectionString";
                var connectionString = configuration.GetConnectionString(connectionStringKey);

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException($"No connection string found for service: {serviceName}");
                }

                // Configurar el proveedor de base de datos según el servicio
                switch (serviceName.ToLower())
                {
                    case "mysqlservice":
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                        break;

                    case "oracleservice":
                        options.UseOracle(connectionString);
                        break;

                    case "mongoservice":
                        throw new NotSupportedException("MongoDB is not directly supported by EF Core. Use an external MongoDB library.");

                    case "postgresqlservice":
                        options.UseNpgsql(connectionString);
                        break;

                    case "sqlserverservice":
                        options.UseSqlServer(connectionString);
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported database provider for service: {serviceName}");
                }
            });

            // Solo agregar DatabaseSeeder si se usa SQL Server
            if (serviceName.ToLower() == "sqlserverservice")
            {
                services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
            }

            return services;
        }

        internal static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<IGiftUser, IGiftRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            return services;
        }
        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityKey"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JwtIssuer"],
                    ValidAudience = configuration["JwtAudience"],
                    ClockSkew = TimeSpan.Zero//Para más información sobre esto leer el README
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query[AppConstants.StorageConstants.Local.Access_Token];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(AppConstants.SignalR.HubUrl))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception is SecurityTokenExpiredException)
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("The token is expired"));
                            return c.Response.WriteAsync(result);
                        }
                        else
                        {
#if DEBUG
                            c.NoResult();
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
#else
                                        c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        c.Response.ContentType = "application/json";
                                        var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                                        return c.Response.WriteAsync(result);
#endif
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized."));
                            return context.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource."));
                        return context.Response.WriteAsync(result);
                    }
                };
            });//TODO estudiar esto
               //TODO debemos agregar por acá el AddAuthorization de blazorHero para los permisos/roles
            return services;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                           .AddTransient(typeof(IRepository<,>), typeof(Repository<,>))
                           .AddTransient<IPeticionesRepository, PeticionesRepository>()
                           .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }
    }
}
