﻿using System.Net;
using System.Text.Json.Serialization;
using IGift.Application.AppConfiguration;
using IGift.Application.Interfaces.Serialization.Options;
using IGift.Application.Interfaces.Serialization.Settings;
using IGift.Application.Serialization;
using IGift.Application.Serialization.JsonConverters;
using IGift.Application.Serialization.Serializers;
using IGift.Application.Serialization.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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
    }
}