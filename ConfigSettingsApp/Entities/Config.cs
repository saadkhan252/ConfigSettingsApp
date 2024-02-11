using System.ComponentModel.DataAnnotations;

namespace ConfigSettingsApp.Entities
{
    public class Config
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._:\/{}]+$")]
        public string Key { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._:\/{}]+$")]
        public string Value { get; set; }
    }
}
