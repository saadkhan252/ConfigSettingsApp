using ConfigSettingsApp.Entities;

namespace ConfigSettingsApp.Services
{
    public interface IConfigService
    {
        List<Dictionary<string, string>> ReadConfigFile(string filePath);
        void ReadDefaultConfig(Dictionary<string, string> configSettings, string[] parts);
        void ReadServerSpecificConfig(Dictionary<string, string> configSettings, string[] parts, Dictionary<string, string> defaultConfigSettings);

        void WriteToConfigFile(List<Dictionary<string, string>> configSettings, string filePath);

        List<Item> GetServerNames(List<Dictionary<string, string>> allConfigSettings);
        void GetServerSpecificKeys(List<Dictionary<string, string>> allConfigSettings, Dictionary<string, string> configSettings, List<Item> listItems);
        List<string> ExtractKeysFromDictionary(Dictionary<string, string> configSettings, bool isDefaultSetting);
        void AppendServerNameToKeys(List<string> keys, string serverName, List<Item> listItems);

        bool ValidateUserInputs(Dictionary<string, string> configSettings);
    }
}