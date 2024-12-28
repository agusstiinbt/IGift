using IGift.Application.OptionsPattern;
using Microsoft.Extensions.Configuration;

namespace IGIFT.Server.Shared
{
    public static class ServerManager
    {
        public static AppConfiguration GetApplicationSettings(IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            return applicationSettingsConfiguration.Get<AppConfiguration>();
        }
    }
}
