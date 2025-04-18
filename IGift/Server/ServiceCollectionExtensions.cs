using System.Net;
using System.Text;
using FluentValidation;
using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Interfaces.Communication.Chat;
using IGift.Application.Interfaces.Communication.Mail;
using IGift.Application.Interfaces.Dates;
using IGift.Application.Interfaces.DDBB.Sql;
using IGift.Application.Interfaces.Files;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Application.Interfaces.Repositories.Generic.NonAuditable;
using IGift.Application.Interfaces.Repositories.NonGeneric.Peticiones;
using IGift.Application.Interfaces.Serialization.Options;
using IGift.Application.Interfaces.Serialization.Settings;
using IGift.Application.OptionsPattern;
using IGift.Application.Validators;
using IGift.Infrastructure.Data;
using IGift.Infrastructure.Mappings.Titulos;
using IGift.Infrastructure.Models;
using IGift.Infrastructure.Repositories.Generic.Auditable;
using IGift.Infrastructure.Repositories.Generic.NonAuditable;
using IGift.Infrastructure.Repositories.NonGeneric;
using IGift.Infrastructure.Serialization;
using IGift.Infrastructure.Serialization.JsonConverters;
using IGift.Infrastructure.Serialization.Serializers;
using IGift.Infrastructure.Serialization.Settings;
using IGift.Infrastructure.Services.Communication;
using IGift.Infrastructure.Services.Dates;
using IGift.Infrastructure.Services.DDBB.Sql;
using IGift.Infrastructure.Services.Files;
using IGift.Infrastructure.Services.Identity;
using IGift.Infrastructure.Services.Mail;
using IGift.Infrastructure.Services.Validators;
using IGift.Server.Redis;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
namespace IGift.Server
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
            //var serviceName = configuration.GetValue<string>("ServiceName"); // Variable que identifica cada microservicio

            AppConfiguration config = ServerManager.GetApplicationSettings(configuration);

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
                                .AllowAnyMethod()
                            .WithOrigins(config.ApplicationUrl.TrimEnd('/'));

                        });
                });
            }
            ;
            ;
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
            services
                 .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
                 .Configure<SystemTextJsonOptions>(configureOptions =>
                 {
                     if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                         configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                 });
            services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

            services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it
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
        internal static IServiceCollection AddDatabase(
                 this IServiceCollection services,
                 IConfiguration configuration)
                 => services
                     .AddDbContext<ApplicationDbContext>(options => options
                         .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                 .AddTransient<IDatabaseSeeder, SQLDatabaseSeeder>();
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
        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppConfiguration config)
        {
            var key = Encoding.UTF8.GetBytes(config.Secret);
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
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
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
                            var path = c.HttpContext.Request.Path;
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
                        var path = context.HttpContext.Request.Path;

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
                        var path = context.HttpContext.Request.Path;

                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource."));
                        return context.Response.WriteAsync(result);
                    }
                };
            });
            services.AddAuthorization();
            return services;
        }
        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                        .AddTransient(typeof(IAuditableUnitOfWork<>), typeof(AuditableUnitOfWork<>))
                        .AddTransient(typeof(INonAuditableUnitOfWork<>), typeof(NonAuditableUnitOfWork<>))
                        .AddTransient(typeof(IAuditableRepository<,>), typeof(AuditableRepository<,>))
                        .AddTransient(typeof(INonAuditableRepository<,>), typeof(NonAuditableRepository<,>))
                        .AddTransient<IPeticionesRepository, PeticionesRepository>();
        }
        /// <summary>
        /// Este método es ideal para registrar los componentes básicos y servicios transversales de la capa de la aplicación que se refieren más a la infraestructura o comportamiento común. NO debería contener servicios de negocio específicos
        /// </summary>
        /// <param name="services"></param>
        internal static void AddApplicationLayer(this IServiceCollection services)
        {
            //Aunque todos los microservicios utilicen AutoMapper, lo importante es que cada uno registre únicamente los perfiles relevantes para su ámbito. Aquí es donde usar Assembly.GetExecutingAssembly() sigue siendo útil porque asegura que solo se carguen los perfiles que pertenecen al ensamblado actual, sin riesgo de conflictos ni sobrecarga.
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());


            //Esta sintaxis le dice a MediatR que registre automáticamente todos los handlers (commands, queries, etc.) y pipeline behaviors que estén definidos en el ensamblado especificado.

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAllPeticionesQuery>());


            //IPipeline... es una interfaz de MediatR que permite interceptar las solicitudes (queries o commands) antes o después de que lleguen a sus respectivos handlers. Esto es útil para implementar comportamientos transversales, como validaciones, logging, manejo de transacciones, etc.
            //Si un microservicio no usa validaciones, por ejemplo, puedes omitir
            services.AddValidatorsFromAssemblyContaining<AddEditProductCommandValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
        /// <summary>
        /// Este método es el lugar adecuado para registrar servicios específicos de la lógica de negocio, que representan el comportamiento central de tu aplicación, como el manejo de productos, cuentas, usuarios, etc. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddApplicationServices(this IServiceCollection services, string serverName)
        {
            services.AddTransient<IDatabaseSeeder, SQLDatabaseSeeder>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IProfilePicture, ProfilePictureService>();
            services.AddTransient<IUploadFileService, UploadService>();

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
            services.AddAutoMapper(typeof(TitulosDesconectadoProfile).Assembly);
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

        internal static AppConfiguration GetApplicationSettings(
         this IServiceCollection services,
         IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<AppConfiguration>();
        }
    }
}
