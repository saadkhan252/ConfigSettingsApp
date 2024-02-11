using ConfigSettingsApp.Components.Pages;
using ConfigSettingsApp.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ConfigSettingsApp.FunctionalTests.Components
{
    [TestClass]
    public class ViewComponentTests : Bunit.TestContext
    {
        [TestInitialize]
        public void Initialize()
        {
            var filePathKeyValueSettings = new Dictionary<string, string> { { "ConfigFilePath", "Files/TestData/test-read-config.txt" } };
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(filePathKeyValueSettings).Build();
            Services.AddSingleton(configuration);
            Services.AddSingleton<IConfigService>(new ConfigService());
        }

        [TestMethod]
        public void RenderViewComponent_VerifyRenderedMarkup_RenderedMarkupShouldContainHeadingAndDropDownList()
        {
            var cut = RenderComponent<View>();

            cut.MarkupMatches(@"<h1>All Config Settings</h1>
                <div class=""form-group"">
                  <label class=""form-label"">Config Type:
                  </label>
                  <div>
                    <select class=""form-control"" id=""ddl-servers"" >
                      <option value="""">Please Select</option>
                      <option value=""0"">Default</option>
                      <option value=""1"">SRVTST0003</option>
                      <option value=""2"">SRVTST0006</option>
                      <option value=""3"">SRVTST0009</option>
                      <option value=""4"">SRVTST0012</option>
                      <option value=""5"">SRVTST0016</option>
                      <option value=""6"">SRVTST0019</option>
                    </select>
                  </div>
                </div>
                ");
        }

        [TestMethod]
        public void RenderViewComponent_VerifyAllConfigSettingsData_TotalConfigServersCountShould7()
        {
            var expectedConfigSettingsCount = 7;

            var cut = RenderComponent<View>();
            var actualConfigSettings = cut.Instance.AllConfigSettings;

            Assert.AreEqual(expectedConfigSettingsCount, actualConfigSettings.Count);
        }

        [TestMethod]
        public void RenderViewComponent_VerifyAllDropDownItems_TotalDropDownItemsCountShould7()
        {
            var expectedDropDropListItems = 7;

            var cut = RenderComponent<View>();
            var actualDropDropListItems = cut.Instance.DropDownListItems;

            Assert.AreEqual(expectedDropDropListItems, actualDropDropListItems.Count);
        }

        [TestMethod]
        public void RenderViewComponent_SelectItemFromConfigTypeDropDownList_SelectedItemShouldBe1()
        {
            var expectedItem = "1";
            var cut = RenderComponent<View>();

            cut.Find("#ddl-servers").Change(expectedItem);
            var actualSelectedItem = cut.Instance.SelectedItem;

            Assert.AreEqual(expectedItem, actualSelectedItem);
        }

        [TestMethod]
        public void RenderViewComponent_SelectItemFromConfigTypeDropDownList_SelectedConfigSettingsCountShouldBe2()
        {
            var expectedConfigSettingsCount = 2;
            
            var cut = RenderComponent<View>();
            var itemToBeSelected = "1";
            cut.Find("#ddl-servers").Change(itemToBeSelected);
            var actualSelectedConfigSettings = cut.Instance.SelectedConfigSettings;

            Assert.AreEqual(expectedConfigSettingsCount, actualSelectedConfigSettings.Count);
        }

        [TestMethod]
        public void RenderViewComponent_SelectItemFromConfigTypeDropDownList_MarkupMustContainTableWithKeysAndValues()
        {
            var cut = RenderComponent<View>();

            var itemToBeSelected = "1";
            cut.Find("#ddl-servers").Change(itemToBeSelected);

            var configSettingsTable = cut.Find("table");

            configSettingsTable.MarkupMatches(@"<table class=""table table-hover mt-4"" id=""tbl-config-settings"">
                <thead>
                <tr>
                    <th scope=""col"">Key</th>
                    <th scope=""col"">Value</th>
                </tr>
                </thead>
                <tbody>
                <tr>
                    <td>SERVER_NAME{SRVTST0003}</td>
                    <td>MRAPPPOOLPORTL0003</td>
                </tr>
                <tr>
                    <td>IP_ADDRESS{SRVTST0003}</td>
                    <td>10.200.0.100</td>
                </tr>
                </tbody>
            </table>");
        }
    }
}