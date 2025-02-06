using IGift.Application.OptionsPattern;

namespace IGift.Server
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
