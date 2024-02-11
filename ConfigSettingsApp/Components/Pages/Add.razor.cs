using ConfigSettingsApp.Entities;
using ConfigSettingsApp.Helpers;
using ConfigSettingsApp.Services;
using Microsoft.AspNetCore.Components;

namespace ConfigSettingsApp.Components.Pages
{
    public partial class Add
    {
        [Inject]
        protected IConfigService ConfigService { get; set; }

        [Inject]
        protected IConfiguration Configuration { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Id { get; set; } = string.Empty;

        public Config Config { get; set; } = new Config();

        public string FilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;

        public List<Dictionary<string, string>> AllConfigSettings { get; set; } = [];
        public Dictionary<string, string> SpecificConfigSettings { get; set; } = [];
        public List<Item> DropDownListItems { get; set; } = [];

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

        public void OnValidSubmit()
        {
            AddKeyValue(Config);
            ConfigService.WriteToConfigFile(AllConfigSettings, FilePath);
            ShowMessage = true;
            Message = "New config added successfully";
            MessageColour = "green";
        }

        public void AddKeyValue(Config setting)
        {
            SpecificConfigSettings.Add(setting.Key, setting.Value);
        }

        public void GetSpecificConfigSettings()
        {
            if (AllConfigSettings.Count > 0 && !string.IsNullOrWhiteSpace(Id))
            {
                var validItem = int.TryParse(Id, out int requestedId);
                if (validItem)
                {
                    SpecificConfigSettings = AllConfigSettings.ElementAt(requestedId);
                    Title = GetTitle();

                    PopulateDropDownListForServerSpecificSelection(requestedId);
                    WarnIfNoListItemsAreAvailable();
                }
            }
        }

        public void DropDownListItemSelection(string selectedItem)
        {
            if (!string.IsNullOrWhiteSpace(selectedItem))
            {
                Config.Key = selectedItem;
            }
        }

        public string GetTitle()
        {
            var serverKey = SpecificConfigSettings.ElementAt(0).Key;
            Server = serverKey == "SERVER_NAME" ? "Default" : serverKey.Split('{', '}')[1];
            return $"Add {Server} Settings";
        }

        public void PopulateDropDownListForServerSpecificSelection(int id)
        {
            if (id != 0)
            {
                ConfigService.GetServerSpecificKeys(AllConfigSettings, SpecificConfigSettings, DropDownListItems);
            }
        }

        public void WarnIfNoListItemsAreAvailable()
        {
            if (DropDownListItems.Count == 0)
            {
                ShowMessage = true;
                Message = "You already have all the necessary config keys. You must add new keys to Default settings first before you can add any new ones for this server.";
                MessageColour = "red";
            }
        }

        public void ResetMessage()
        {
            ShowMessage = false;
            Message = string.Empty;
        }
    }
}
