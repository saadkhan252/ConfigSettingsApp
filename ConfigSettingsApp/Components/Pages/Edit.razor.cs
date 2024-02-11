using ConfigSettingsApp.Helpers;
using ConfigSettingsApp.Services;
using Microsoft.AspNetCore.Components;

namespace ConfigSettingsApp.Components.Pages
{
    public partial class Edit
    {
        [Inject]
        protected IConfigService ConfigService { get; set; }

        [Inject]
        protected IConfiguration Configuration { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        public List<Dictionary<string, string>> AllConfigSettings { get; set; } = [];
        public Dictionary<string, string> SpecificConfigSettings { get; set; } = [];

        public bool ShowMessage { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string MessageColour { get; set; } = string.Empty;

        protected override void OnInitialized()
        {
            GetData();
            GetSpecificConfigSettings();
            ResetMessage();
        }

        public void GetData()
        {
            FilePath = ConfigHelpers.GetFilePath(Configuration, "ConfigFilePath");
            AllConfigSettings = ConfigService.ReadConfigFile(FilePath);
        }

        public void GetSpecificConfigSettings()
        {
            if (AllConfigSettings.Count > 0 && !string.IsNullOrWhiteSpace(Id))
            {
                var validItem = int.TryParse(Id, out int selection);
                if (validItem)
                {
                    SpecificConfigSettings = AllConfigSettings.ElementAt(selection);
                    Title = selection == 0 ? "Edit Default Settings" : "Edit Server Specific Settings";
                }
            }
        }

        public void HandleSubmit()
        {
            var dataIsValid = ConfigService.ValidateUserInputs(SpecificConfigSettings);
            if (dataIsValid)
            {
                ConfigService.WriteToConfigFile(AllConfigSettings, FilePath);
                ShowMessage = true;
                Message = "Config updated successfully";
                MessageColour = "green";
            }
            else
            {
                ShowMessage = true;
                Message = @"Input must match the regular expression '^[a-zA-Z0-9._:\/{}]+$'.";
                MessageColour = "red";
            }
        }

        public void ChangeValue(string key, string newValue)
        {
            SpecificConfigSettings[key] = newValue;
        }

        public void ResetMessage()
        {
            ShowMessage = false;
            Message = string.Empty;
        }
    }
}
