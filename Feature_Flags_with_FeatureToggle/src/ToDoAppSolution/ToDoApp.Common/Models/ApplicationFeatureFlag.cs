namespace ToDoApp.Common.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class ApplicationFeatureFlag
    {
        [JsonProperty]
        public int Id { get; set; }

        [Required]
        [JsonProperty]
        public string Name { get; set; }

        [Required]
        [JsonProperty]
        [DisplayName("Enabled")]
        public bool IsEnabled { get; set; }
    }
}