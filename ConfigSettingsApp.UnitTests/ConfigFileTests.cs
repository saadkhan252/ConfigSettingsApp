using ConfigSettingsApp.Entities;
using ConfigSettingsApp.Helpers;
using ConfigSettingsApp.Services;
using Microsoft.Extensions.Configuration;

namespace ConfigSettingsApp.UnitTests
{
    [TestClass]
    public class ConfigFileTests
    {
        private IConfigService _configService;
        private string _configFilePath;
        private List<Dictionary<string, string>> _Data;
        private List<Item> _listItems;

        [TestInitialize]
        public void Initialize()
        {
            _configService = new ConfigService();

            _configFilePath = "TestData/test-config.txt";
            _Data = _configService.ReadConfigFile(_configFilePath);

            _listItems = [
                new() { Code = "0", Description = "Default" },
                new() { Code = "1", Description = "SRVTST0003" },
                new() { Code = "2", Description = "SRVTST0006" },
                new() { Code = "3", Description = "SRVTST0009" },
                new() { Code = "4", Description = "SRVTST0012" },
                new() { Code = "5", Description = "SRVTST0016" },
                new() { Code = "6", Description = "SRVTST0019" }
            ];
        }

        [TestMethod]
        public void ReadDefaultConfig_ReadAndAddConfigLineToDictionary_ShouldAddDefaultKeyValuesToDictionary()
        {
            var expectedDictionary = new Dictionary<string, string>() { { "SERVER_NAME", "MRAPPPOOLPORTL01" } };

            var configSettings = new Dictionary<string, string>();
            var defaultConfigLine = "SERVER_NAME=MRAPPPOOLPORTL01";
            var parts = defaultConfigLine.Split('=');
            _configService.ReadDefaultConfig(configSettings, parts);

            Assert.AreEqual(expectedDictionary.First().Key, configSettings.First().Key);
            Assert.AreEqual(expectedDictionary.First().Value, configSettings.First().Value);
        }

        [TestMethod]
        public void ReadServerSpecificConfig_ReadAndAddConfigLineToDictionary_ShouldAddServerSpecificKeyValuesToDictionary()
        {
            var expectedDictionary = new Dictionary<string, string>() { { "SERVER_NAME{SRVTST0003}", "MRAPPPOOLPORTL0003" } };

            var configSettings = new Dictionary<string, string>();
            var serverConfigLine = "SERVER_NAME{SRVTST0003}=MRAPPPOOLPORTL0003";
            var parts = serverConfigLine.Split('=');
            _configService.ReadDefaultConfig(configSettings, parts);

            Assert.AreEqual(expectedDictionary.First().Key, configSettings.First().Key);
            Assert.AreEqual(expectedDictionary.First().Value, configSettings.First().Value);
        }

        [TestMethod]
        public void GetServerNames_GetServerNamesForDropDownList_ItemsCountShouldBe7()
        {
            var expectedListItems = _listItems;

            var actualListItems = _configService.GetServerNames(_Data);

            Assert.AreEqual(expectedListItems.Count, actualListItems.Count);
        }

        [TestMethod]
        public void GetServerNames_GetServerNamesForDropDownList_ItemCodesMustMatch()
        {
            var expectedListItems = _listItems;

            var actualListItems = _configService.GetServerNames(_Data);

            Assert.AreEqual(expectedListItems[0].Code, actualListItems[0].Code);
            Assert.AreEqual(expectedListItems[1].Code, actualListItems[1].Code);
            Assert.AreEqual(expectedListItems[2].Code, actualListItems[2].Code);
            Assert.AreEqual(expectedListItems[3].Code, actualListItems[3].Code);
            Assert.AreEqual(expectedListItems[4].Code, actualListItems[4].Code);
            Assert.AreEqual(expectedListItems[5].Code, actualListItems[5].Code);
        }

        [TestMethod]
        public void GetServerNames_GetServerNamesForDropDownList_ItemDescriptionsMustMatch()
        {
            var expectedListItems = _listItems;

            var actualListItems = _configService.GetServerNames(_Data);

            Assert.AreEqual(expectedListItems[0].Description, actualListItems[0].Description);
            Assert.AreEqual(expectedListItems[1].Description, actualListItems[1].Description);
            Assert.AreEqual(expectedListItems[2].Description, actualListItems[2].Description);
            Assert.AreEqual(expectedListItems[3].Description, actualListItems[3].Description);
            Assert.AreEqual(expectedListItems[4].Description, actualListItems[4].Description);
            Assert.AreEqual(expectedListItems[5].Description, actualListItems[5].Description);
        }

        [TestMethod]
        public void ExtractKeysFromDictionary_GetDefaultSettingKeys_KeysCountShouldBe6()
        {
            var expectedKeys = new List<string>() { 
                "SERVER_NAME",
                "URL", 
                "DB",
                "SQL_SERVER", 
                "DOMAIN", 
                "COOKIE_DOMAIN" 
            };
            var isDefault = true;

            var actualKeys = _configService.ExtractKeysFromDictionary(_Data[0], isDefault);

            Assert.AreEqual(expectedKeys.Count, actualKeys.Count);
        }

        [TestMethod]
        public void ExtractKeysFromDictionary_GetDefaultSettingKeys_DefaultKeysMustMatch()
        {
            var expectedKeys = new List<string>() {
                "SERVER_NAME",
                "URL",
                "DB",
                "IP_ADDRESS",
                "DOMAIN",
                "COOKIE_DOMAIN"
            };
            var isDefault = true;

            var actualKeys = _configService.ExtractKeysFromDictionary(_Data[0], isDefault);

            Assert.AreEqual(expectedKeys[0], actualKeys[0]);
            Assert.AreEqual(expectedKeys[1], actualKeys[1]);
            Assert.AreEqual(expectedKeys[2], actualKeys[2]);
            Assert.AreEqual(expectedKeys[3], actualKeys[3]);
            Assert.AreEqual(expectedKeys[4], actualKeys[4]);
            Assert.AreEqual(expectedKeys[5], actualKeys[5]);
        }

        [TestMethod]
        public void ExtractKeysFromDictionary_GetServerSpecificKeysWithoutServerSuffix_KeysCountShouldBe2()
        {
            //Without server suffix
            var expectedServerSpecificKeys = new List<string>() {
                "SERVER_NAME",
                "IP_ADDRESS",
            };
            var isDefault = false;

            var actualServerSpecificKeys = _configService.ExtractKeysFromDictionary(_Data[1], isDefault);

            Assert.AreEqual(expectedServerSpecificKeys.Count, actualServerSpecificKeys.Count);
        }

        [TestMethod]
        public void ExtractKeysFromDictionary_GetServerSpecificKeysWithoutServerSuffix_KeysMustMatch()
        {
            //Without server suffix
            var expectedServerSpecificKeys = new List<string>() {
                "SERVER_NAME",
                "IP_ADDRESS",
            };
            var isDefault = false;

            var actualServerSpecificKeys = _configService.ExtractKeysFromDictionary(_Data[1], isDefault);

            Assert.AreEqual(expectedServerSpecificKeys[0], actualServerSpecificKeys[0]);
            Assert.AreEqual(expectedServerSpecificKeys[1], actualServerSpecificKeys[1]);
        }

        [TestMethod]
        public void AppendServerNameToKeys_AppendServerNameWithCurlyBrackets_ListItemCodesMustMatch()
        {
            var Keys = new List<string>() {
                "SERVER_NAME",
                "IP_ADDRESS",
                "DOMAIN",
                "COOKIE_DOMAIN"
            };
            var serverName = "SERVER_NAME{SRVTST0016}";
            var listItems = new List<Item>();

            var expectedKeys = new List<Item>() {
                new() { Code = "0", Description = "SERVER_NAME{SRVTST0016}" },
                new() { Code = "1", Description = "IP_ADDRESS{SRVTST0016}" },
                new() { Code = "2", Description = "DOMAIN{SRVTST0016}" },
                new() { Code = "3", Description = "COOKIE_DOMAIN{SRVTST0016}" }
            };

            _configService.AppendServerNameToKeys(Keys, serverName, listItems);

            Assert.AreEqual(expectedKeys[0].Code, listItems[0].Code);
            Assert.AreEqual(expectedKeys[1].Code, listItems[1].Code);
            Assert.AreEqual(expectedKeys[2].Code, listItems[2].Code);
            Assert.AreEqual(expectedKeys[3].Code, listItems[3].Code);
        }

        [TestMethod]
        public void AppendServerNameToKeys_AppendServerNameWithCurlyBrackets_ListItemDescriptionsMustMatch()
        {
            var Keys = new List<string>() {
                "SERVER_NAME",
                "IP_ADDRESS",
                "DOMAIN",
                "COOKIE_DOMAIN"
            };
            var serverName = "SERVER_NAME{SRVTST0016}";
            var listItems = new List<Item>();

            var expectedKeys = new List<Item>() {
                new() { Code = "0", Description = "SERVER_NAME{SRVTST0016}" },
                new() { Code = "1", Description = "IP_ADDRESS{SRVTST0016}" },
                new() { Code = "2", Description = "DOMAIN{SRVTST0016}" },
                new() { Code = "3", Description = "COOKIE_DOMAIN{SRVTST0016}" }
            };

            _configService.AppendServerNameToKeys(Keys, serverName, listItems);

            Assert.AreEqual(expectedKeys[0].Description, listItems[0].Description);
            Assert.AreEqual(expectedKeys[1].Description, listItems[1].Description);
            Assert.AreEqual(expectedKeys[2].Description, listItems[2].Description);
            Assert.AreEqual(expectedKeys[3].Description, listItems[3].Description);
        }

        [TestMethod]
        public void ValidateUserInputs_ValidExpression_DefaultConfig_ShouldReturnTrue()
        {
            var configSettings = new Dictionary<string, string>() { { "SERVER_NAME", "MRAPPPOOLPORTL01" } };
            var isValid = _configService.ValidateUserInputs(configSettings);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void ValidateUserInputs_ExpressionContainsBrackets_ShouldReturnFalse()
        {
            var configSettings = new Dictionary<string, string>() { { "SERVER_NAME{SRVTST0003}", "MRAPPPOOLPORTL0003()" } };
            var isValid = _configService.ValidateUserInputs(configSettings);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidateUserInputs_ExpressionContainsSpaces_ShouldReturnFalse()
        {
            var configSettings = new Dictionary<string, string>() { { "SERVER_NAME{SRVTST0003}", "MRAPPPOOLPORTL0003 TEST SERVER" } };
            var isValid = _configService.ValidateUserInputs(configSettings);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidateUserInputs_ExpressionContainsAsterisk_ShouldReturnFalse()
        {
            var configSettings = new Dictionary<string, string>() { { "SERVER_NAME{SRVTST0003}", "SELECT * FROM Servers" } };
            var isValid = _configService.ValidateUserInputs(configSettings);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ConfigurationFile_ValidPath_FilePathShouldBeN()
        {
            var filePathKeyValueSettings = new Dictionary<string, string> {
                { "ConfigFilePath", "Files/config.txt" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(filePathKeyValueSettings).Build();

            var filePath = ConfigHelpers.GetFilePath(configuration, filePathKeyValueSettings.First().Key);

            Assert.IsNotNull(filePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ConfigurationFile_InvalidKeyToConfigPath_ShouldThrowException()
        {
            var filePathKeyValueSettings = new Dictionary<string, string> { 
                { "ConfigFilePath", "Files/config.txt" } 
            };
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(filePathKeyValueSettings).Build();

            ConfigHelpers.GetFilePath(configuration, "ConfigFile");
        }
    }
}