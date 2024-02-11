using ConfigSettingsApp.Entities;
using ConfigSettingsApp.Helpers;
using ConfigSettingsApp.Services;
using Microsoft.AspNetCore.Components;

namespace ConfigSettingsApp.Components.Pages
{
    public partial class View
    {
        [Inject]
        protected IConfigService ConfigService { get; set; }

        [Inject]
        protected IConfiguration Configuration { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public List<Dictionary<string, string>> AllConfigSettings { get; set; } = [];
        public Dictionary<string, string> SelectedConfigSettings { get; set; } = [];

        public List<Item> DropDownListItems { get; set; } = [];
        public string SelectedItem { get; set; } = string.Empty;

        protected override void OnInitialized()
        {
            GetData();
            GetServerNames();
        }

        public void GetData()
        {
            FilePath = ConfigHelpers.GetFilePath(Configuration, "ConfigFilePath");
            AllConfigSettings = ConfigService.ReadConfigFile(FilePath);
        }

        public void GetServerNames()
        {
            DropDownListItems = ConfigService.GetServerNames(AllConfigSettings);
        }

        public void DropDownListItemSelection(string selectedItem)
        {
            var validItem = int.TryParse(selectedItem, out int selection);
            if (validItem)
            {
                SelectedItem = selectedItem;
                SelectedConfigSettings = AllConfigSettings.ElementAt(selection);
            }
        }
    }
}
