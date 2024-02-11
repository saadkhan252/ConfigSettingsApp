using ConfigSettingsApp.Services;

namespace ConfigSettingsApp.IntegrationTests
{
    [TestClass]
    public class WriteToFileTests
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
        public void WriteToConfigFile_UpdateDefaultSettings_DefaultUrlShouldBeUpdatedToLloydsUrl()
        {
            //get original url value
            _Data[0].TryGetValue("URL", out string originalDefaultUrlValue);

            //set new required url
            var requiredUrl = "test.com";
            _Data[0]["URL"] = requiredUrl;

            //write to file
            _configService.WriteToConfigFile(_Data, _configFilePath);

            //read updated file
            var updatedData = _configService.ReadConfigFile(_configFilePath);

            //get updated url value
            updatedData[0].TryGetValue("URL", out string updatedDefaultUrlValue);

            Assert.AreNotEqual(originalDefaultUrlValue, updatedDefaultUrlValue);
            Assert.AreEqual(requiredUrl, updatedDefaultUrlValue);
        }

        [TestMethod]
        public void WriteToConfigFile_AddNewDefaultSettings_NewSettingsShouldBeLocationUK()
        {
            //count of all original key/value pairs
            var originalKeyValuePairsCount = _Data[0].Count;

            //add new key/value pair
            var expectedKey = "LOCATION";
            var expectedValue = "UK";
            _Data[0].Add(expectedKey, expectedValue);

            //write to file
            _configService.WriteToConfigFile(_Data, _configFilePath);

            //read updated file
            var updatedData = _configService.ReadConfigFile(_configFilePath);

            //get new key and value existence status
            var actualKeyExistStatus = updatedData[0].ContainsKey(expectedKey);
            var actualValueExistStatus = updatedData[0].ContainsValue(expectedValue);

            Assert.AreNotEqual(originalKeyValuePairsCount, updatedData.Count);
            Assert.IsTrue(actualKeyExistStatus);
            Assert.IsTrue(actualValueExistStatus);
        }
    }
}
