using ConfigSettingsApp.Services;

namespace ConfigSettingsApp.IntegrationTests
{
    [TestClass]
    public class ReadFileTests
    {
        private IConfigService _configService;
        private string _configFilePath;
        private List<Dictionary<string, string>> _Data;

        [TestInitialize]
        public void Initialize()
        {
            _configService = new ConfigService();
            _configFilePath = "TestData/test-config.txt";
            _Data = _configService.ReadConfigFile(_configFilePath);
        }

        [TestMethod]
        public void ReadConfigFile_GetCountOfServersIncludingDefault_ServerCountShouldBe7()
        {
            var expectedNoOfServersIncDefault = 7;

            var actualNoOfServersIncDefault = _Data.Count;

            Assert.AreEqual(expectedNoOfServersIncDefault, actualNoOfServersIncDefault);
        }

        [TestMethod]
        public void ReadConfigFile_GetCountOfDefaultKeyValues_DefaultKeyValuesCountShouldBe6()
        {
            var expectedNoOfDefaultKeyValues = 6;

            var actualNoOfDefaultKeyValues = _Data.First().Count;

            Assert.AreEqual(expectedNoOfDefaultKeyValues, actualNoOfDefaultKeyValues);
        }

        [TestMethod]
        public void ReadConfigFile_DefaultKeysExistenceCheck_DefaultKeysExistenceStatusShouldBeTrue()
        {
            var defaultKeysList = new List<string>()
            {
                "SERVER_NAME",
                "URL",
                "DB",
                "IP_ADDRESS",
                "DOMAIN",
                "COOKIE_DOMAIN"
            };
            var expectedExistStatus = true;

            var defaultDictionary = _Data.First();
            var actualExistStatus = defaultKeysList.All(defaultDictionary.ContainsKey);

            Assert.AreEqual(expectedExistStatus, actualExistStatus);
        }

        [TestMethod]
        public void ReadConfigFile_DefaultKeysExistenceCheck_DefaultKeysExistenceStatusShouldBeFalse()
        {
            var defaultKeysList = new List<string>()
            {
                "SERVER_NAME",
                "URL",
                "DB",
                "NEW_KEY"
            };
            var expectedExistStatus = false;

            var defaultDictionary = _Data.First();
            var actualExistStatus = defaultKeysList.All(defaultDictionary.ContainsKey);

            Assert.AreEqual(expectedExistStatus, actualExistStatus);
        }

        [TestMethod]
        public void ReadConfigFile_GetCountOfServerSpecificKeyValues_KeyValuesCountForServerSRVTST0003ShouldBe2()
        {
            var expectedNoOfServerSpecificKeyValues = 2;

            var serverSpecificDictionary = _Data.Find(x => x.ContainsKey("SERVER_NAME{SRVTST0003}"));
            var actualNoOfServerSpecificKeyValues = serverSpecificDictionary != null ? serverSpecificDictionary.Count : 0;

            Assert.AreEqual(expectedNoOfServerSpecificKeyValues, actualNoOfServerSpecificKeyValues);
        }
    }
}