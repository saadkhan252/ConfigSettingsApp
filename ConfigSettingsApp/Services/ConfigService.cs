
using ConfigSettingsApp.Entities;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace ConfigSettingsApp.Services
{
    public class ConfigService : IConfigService
    {
        public List<Dictionary<string, string>> ReadConfigFile(string filePath)
        {
            var list = new List<Dictionary<string, string>>();
            var configSettings = new Dictionary<string, string>();
            var lines = File.ReadAllLines(filePath);
            var linesWithoutComments = lines.Where(line => !line.StartsWith(';')).ToArray();

            foreach (string line in linesWithoutComments)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var parts = line.Split('=');
                    if (!line.Contains('{'))
                    {
                        ReadDefaultConfig(configSettings, parts);
                    }
                    else
                    {
                        ReadServerSpecificConfig(configSettings, parts, list[0]);
                    }
                }

                if (string.IsNullOrEmpty(line) || line == linesWithoutComments.Last())
                {
                    var newDictionary = configSettings.ToDictionary(entry => entry.Key,
                                           entry => entry.Value);

                    list.Add(newDictionary);
                    configSettings.Clear();
                }
            }

            return list;
        }

        public void ReadDefaultConfig(Dictionary<string, string> configSettings, string[] parts)
        {
            configSettings[parts[0]] = parts[1];
        }

        public void ReadServerSpecificConfig(Dictionary<string, string> configSettings, string[] parts, Dictionary<string,string> defaultConfigSettings)
        {
            var key = parts[0].Split('{', '}')[1];
            var prefix = parts[0].Split('{')[0];
            var value = parts[1];

            var allDefaultKeys = ExtractKeysFromDictionary(defaultConfigSettings, true);

            if (allDefaultKeys.Contains(prefix))
            {
                configSettings[$"{prefix}{{{key}}}"] = value;
            }
        }

        public void WriteToConfigFile(List<Dictionary<string, string>> configSettingsList, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var configSettings in configSettingsList)
                {
                    foreach (var entry in configSettings)
                    {
                        writer.WriteLine($"{entry.Key}={entry.Value}");
                    }

                    writer.WriteLine();
                }
            }
        }

        public List<Item> GetServerNames(List<Dictionary<string, string>> allConfigSettings)
        {
            var allServerNameKeys = allConfigSettings.SelectMany(dict => dict.Keys).Where(key => key.StartsWith("SERVER_NAME")).ToList();
            var keysList = new List<Item>();

            for (int i = 0; i < allServerNameKeys.Count; i++)
            {
                keysList.Add(new Item()
                {
                    Code = i.ToString(),
                    Description = allServerNameKeys[i] == "SERVER_NAME" ? "Default" : allServerNameKeys[i].Split('{', '}')[1]
                });
            }

            return keysList;
        }

        public void GetServerSpecificKeys(List<Dictionary<string, string>> allConfigSettings, Dictionary<string, string> configSettings, List<Item> listItems)
        {
            var defaultKeys = ExtractKeysFromDictionary(allConfigSettings[0], true);
            var serverSpecificKeys = ExtractKeysFromDictionary(configSettings, false);
            var remainingUnusedKeys = defaultKeys.Except(serverSpecificKeys).ToList();
            AppendServerNameToKeys(remainingUnusedKeys, configSettings.ElementAt(0).Key, listItems);
        }

        public List<string> ExtractKeysFromDictionary(Dictionary<string, string> configSettings, bool isDefaultSetting)
        {
            var keys = new List<string>();

            foreach (var setting in configSettings)
            {
                if (isDefaultSetting)
                {
                    keys.Add(setting.Key);
                }
                else
                {
                    keys.Add(Regex.Replace(setting.Key, @" ?\{.*?\}", string.Empty));
                }
            }

            return keys;
        }

        public void AppendServerNameToKeys(List<string> keys, string serverName, List<Item> listItems)
        {
            var server = serverName.Split('{', '}')[1];
            for (int i = 0; i < keys.Count(); i++)
            {
                listItems.Add(new Item()
                {
                    Code = i.ToString(),
                    Description = keys[i] + "{" + server + "}"
                });
            }
        }

        public bool ValidateUserInputs(Dictionary<string, string> configSettings)
        {
            var isValid = true;
            var pattern = @"^[a-zA-Z0-9._:\/{}]+$";

            foreach (var setting in configSettings)
            {
                if ((!Regex.IsMatch(setting.Key, pattern)) || (!Regex.IsMatch(setting.Value, pattern)))
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }
    }
}