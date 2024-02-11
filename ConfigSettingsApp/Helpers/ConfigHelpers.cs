using Microsoft.Extensions.Configuration;

namespace ConfigSettingsApp.Helpers
{
    public class ConfigHelpers
    {
        public static string GetFilePath(IConfiguration configuration, string key)
        {
            var filePath = configuration.GetValue<string>(key);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                var ex = new FileNotFoundException();
                ex.Data.Add("Error", "An error has occurred while retrieving the config file path.");
                throw ex;
            }

            return filePath;
        }
    }
}
