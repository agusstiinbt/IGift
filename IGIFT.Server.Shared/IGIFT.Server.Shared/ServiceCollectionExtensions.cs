using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using IGift.Application.Interfaces.Communication.Chat;
using IGift.Application.Interfaces.Communication.Mail;
using IGift.Application.Interfaces.Dates;
using IGift.Application.Interfaces.DDBB.Sql;
using IGift.Application.Interfaces.Files;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Interfaces.Serialization.Options;
using IGift.Application.Interfaces.Serialization.Settings;
using IGift.Application.OptionsPattern;
using IGift.Infrastructure.Data;
using IGift.Infrastructure.Models;
using IGift.Infrastructure.Repositories;
using IGift.Infrastructure.Serialization;
using IGift.Infrastructure.Serialization.JsonConverters;
using IGift.Infrastructure.Serialization.Serializers;
using IGift.Infrastructure.Serialization.Settings;
using IGift.Infrastructure.Services.Communication;
using IGift.Infrastructure.Services.Dates;
using IGift.Infrastructure.Services.DDBB.MongoDB;
using IGift.Infrastructure.Services.DDBB.MySql;
using IGift.Infrastructure.Services.DDBB.Oracle;
using IGift.Infrastructure.Services.DDBB.PostgreSqL;
using IGift.Infrastructure.Services.DDBB.Sql;
using IGift.Infrastructure.Services.Files;
using IGift.Infrastructure.Services.Identity;
using IGift.Infrastructure.Services.Mail;
using IGift.Infrastructure.Services.Validators;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using IGIFT.Server.Shared.Redis;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace IGIFT.Server.Shared
{
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Este método configura servicios necesarios para que la aplicación pueda manejar solicitudes provenientes de un proxy o balanceador de carga. Está pensado para sistemas distribuidos donde los microservicios pueden estar detrás de proxies SSL (como NGINX o AWS ALB). Además, establece configuraciones de CORS específicas para cada microservicio.
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


                #region Resumen
                //Configura la política CORS(Cross-Origin Resource Sharing).
                //Permite o restringe el acceso de clientes de diferentes orígenes(URLs):
                #endregion
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

            // Registro del serializador principal. Dependiendo del microservice segun su nombre se podra usar uno o el otro

            if (serviceName == "ServiceName")
            {
                services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); //Puede cambiarse en el caso de utilizar un microservice u otro.
            }
            else
            {
                services.AddScoped<IJsonSerializer, NewtonSoftJsonSerializer>();
            }

            return services;
        }
        /// <summary>
        /// Debe ser omitido por el ApiGateWay
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
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
                        services.AddTransient<IDatabaseSeeder, MySqlDatabaseSeeder>();

                        break;

                    case "oracleservice":
                        options.UseOracle(connectionString);
                        services.AddTransient<IDatabaseSeeder, OracleDatabaseSeeder>();

                        break;

                    case AppConstants.Server.ChatService:
                        services.AddTransient<IDatabaseSeeder, MongoDBDatabaseSeeder>();
                        //throw new NotSupportedException("MongoDB is not directly supported by EF Core. Use an external MongoDB library.");
                        break;
                    case "postgresqlservice"://TODO pensar en el nombre para el servidor que usara esta BBDD
                        options.UseNpgsql(connectionString);
                        services.AddTransient<IDatabaseSeeder, PostgreSQLDatabaseSeeder>();

                        break;

                    case AppConstants.Server.AuthService:
                        options.UseSqlServer(connectionString);
                        services.AddTransient<IDatabaseSeeder, SQLDatabaseSeeder>();
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported database provider for service: {serviceName}");
                }
            });



            return services;
        }
        /// <summary>
        /// Configura usuarios, roles, tokens y demas datos. Debe estar exclusivamente en el AuthService, ya que este es el responsable de gestionar usuarios, roles y datos relacionados con la autenticación
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
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
        /// <summary>
        ///Solo debe ser implementado por el ApiGateWay El middleware de autenticación JWT (AddJwtAuthentication) es necesario únicamente en servidores que reciben y procesan solicitudes entrantes con un token JWT. Este middleware valida el token, comprueba su firma, y carga los claims en el contexto de seguridad de ASP.NET.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
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
                        var accessToken = context.Request.Query[AppConstants.Local.Access_Token];

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
        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                           .AddTransient(typeof(IRepository<,>), typeof(Repository<,>))
                           .AddTransient<IPeticionesRepository, PeticionesRepository>()
                           .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }
        /// <summary>
        /// Este método es ideal para registrar los componentes básicos y servicios transversales de la capa de la aplicación que se refieren más a la infraestructura o comportamiento común. NO debería contener servicios de negocio específicos
        /// </summary>
        /// <param name="services"></param>
        internal static void AddApplicationLayer(this IServiceCollection services)
        {
            //Aunque todos los microservicios utilicen AutoMapper, lo importante es que cada uno registre únicamente los perfiles relevantes para su ámbito. Aquí es donde usar Assembly.GetExecutingAssembly() sigue siendo útil porque asegura que solo se carguen los perfiles que pertenecen al ensamblado actual, sin riesgo de conflictos ni sobrecarga.
            services.AddAutoMapper(Assembly.GetExecutingAssembly());


            //Esta sintaxis le dice a MediatR que registre automáticamente todos los handlers (commands, queries, etc.) y pipeline behaviors que estén definidos en el ensamblado especificado.

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


            //IPipeline... es una interfaz de MediatR que permite interceptar las solicitudes (queries o commands) antes o después de que lleguen a sus respectivos handlers. Esto es útil para implementar comportamientos transversales, como validaciones, logging, manejo de transacciones, etc.
            //Si un microservicio no usa validaciones, por ejemplo, puedes omitir
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
        /// <summary>
        /// Este método es el lugar adecuado para registrar servicios específicos de la lógica de negocio, que representan el comportamiento central de tu aplicación, como el manejo de productos, cuentas, usuarios, etc. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddApplicationServices(this IServiceCollection services, string serverName)
        {
            switch (serverName)
            {
                case AppConstants.Server.AuthService:
                    services.AddTransient<IDatabaseSeeder, SQLDatabaseSeeder>();
                    services.AddTransient<ITokenService, TokenService>();
                    services.AddTransient<IUserService, UserService>();
                    break;
                case AppConstants.Server.ChatService:
                    services.AddTransient<IMailService, MailService>();
                    services.AddTransient<IChatService, ChatService>();
                    services.AddTransient<IProfilePicture, ProfilePictureService>();
                    services.AddTransient<IUploadService, UploadService>();
                    break;

                default:
                    break;
            }
            return services;
        }
        internal static IServiceCollection AddSharedInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, SystemDateTimeService>();
            //TODO se usa aqui el Options Pattern
            //services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            //services.AddTransient<IMailService, SMTPMailService>();
            return services;
        }
        /// <summary>
        /// AddInfrastructureMappings es simplemente una forma de centralizar la configuración de AutoMapper y no tiene necesidad de devolver IServiceCollection como los demás métodos de extensión en el IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        internal static void AddInfrastructureMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
        public static IServiceCollection AddSharedRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            //var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString");

            //if (string.IsNullOrEmpty(redisConnectionString))
            //{
            //    throw new Exception("Redis connection string not configured.");
            //}

            //services.AddSingleton<IConnectionMultiplexer>(sp =>
            //    ConnectionMultiplexer.Connect(redisConnectionString));

            //return services;


            var redisConfig = configuration.GetSection("Redis").Get<RedisOptions>();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
            return services;
        }
    }
}
