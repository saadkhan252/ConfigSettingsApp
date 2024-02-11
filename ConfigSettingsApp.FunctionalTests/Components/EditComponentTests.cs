using Bunit;
using ConfigSettingsApp.Components.Pages;
using ConfigSettingsApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSettingsApp.FunctionalTests.Components
{
    [TestClass]
    public class EditComponentTests : Bunit.TestContext
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
        public void RenderEditComponent_SubmitFormWithUpdatedConfigSettings_ShouldRenderConfirmationMessageFollowingSuccessfulSubmission()
        {
            //set Id parameter
            var cut = RenderComponent<Edit>(parameters => parameters
            .Add(p => p.Id, "0")
            );

            //update input fields
            cut.Find("input[id=SERVER_NAME]").Change("DUMMY_SERVER_NAME");
            cut.Find("input[id=URL]").Change("DUMMY_URL");
            cut.Find("input[id=DB]").Change("DMMY_DB");
            cut.Find("input[id=IP_ADDRESS]").Change("DUMMY_IP_ADDRESS");
            cut.Find("input[id=DOMAIN]").Change("DUMMY_DOMAIN");
            cut.Find("input[id=COOKIE_DOMAIN]").Change("DUMMY_COOKIE_DOMAIN");

            //submit form
            cut.Find("form").Submit();

            //check confirmation message
            var confirmationMessage = cut.Find("#message");
            confirmationMessage.MarkupMatches(@"<p id=""message"" style=""padding-top: 20px; color: green"">Config updated successfully</p>");
        }

        [TestMethod]
        public void RenderEditComponent_ServerNameValueFieldIsEmpty_ShouldRenderWarningMessageFollowingSubmission()
        {
            //set Id parameter
            var cut = RenderComponent<Edit>(parameters => parameters
            .Add(p => p.Id, "0")
            );

            //update input fields
            cut.Find("input[id=SERVER_NAME]").Change("");

            //submit form
            cut.Find("form").Submit();

            //check confirmation message
            var confirmationMessage = cut.Find("#message");
            confirmationMessage.MarkupMatches(@"<p id=""message"" style=""padding-top: 20px; color: red"">Input must match the regular expression '^[a-zA-Z0-9._:\/{}]+$'.</p>");
        }
    }
}