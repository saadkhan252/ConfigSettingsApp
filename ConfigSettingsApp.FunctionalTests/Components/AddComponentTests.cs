using Bunit;
using ConfigSettingsApp.Components.Pages;
using ConfigSettingsApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSettingsApp.FunctionalTests.Components
{
    [TestClass]
    public class AddComponentTests : Bunit.TestContext
    {
        [TestInitialize]
        public void Initialize()
        {
            var filePathKeyValueSettings = new Dictionary<string, string> { { "ConfigFilePath", "Files/TestData/test-add-update-config.txt" } };
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(filePathKeyValueSettings).Build();
            Services.AddSingleton(configuration);
            Services.AddSingleton<IConfigService>(new ConfigService());
        }

        [TestMethod]
        public void RenderAddComponent_NewDefaultConfigSettings_ShouldRenderConfirmationMessageFollowingSuccessfulFormSubmission()
        {
            //set Id parameter
            var cut = RenderComponent<Add>(parameters => parameters
            .Add(p => p.Id, "0")
            );

            //enter key and value
            cut.Find("input[id=key]").Change("LOCATION");
            cut.Find("input[id=value]").Change("UK");

            //submit form
            cut.Find("form").Submit();
            
            //check confirmation message
            var confirmationMessage = cut.Find("#message");
            confirmationMessage.MarkupMatches(@"<p id=""message"" style=""padding-top: 20px; color: green"">New config added successfully</p>");
        }

        [TestMethod]
        public void RenderAddComponent_NewServerSpecificConfigSettings_ShouldRenderConfirmationMessageFollowingSuccessfulFormSubmission()
        {
            //set Id parameter
            var cut = RenderComponent<Add>(parameters => parameters
            .Add(p => p.Id, "1")
            );

            //select a key from the available keys drop down list and enter value
            cut.Find("#ddl-keys").Change("DB{SRVTST0003}");
            cut.Find("input[id=value]").Change("DUMMY_DB");

            //submit form
            cut.Find("form").Submit();

            //check confirmation message
            var confirmationMessage = cut.Find("#message");
            confirmationMessage.MarkupMatches(@"<p id=""message"" style=""padding-top: 20px; color: green"">New config added successfully</p>");
        }
    }
}