namespace ToDoApp.Common.Models
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class ApplicationFeatureFlagUpdate
    {
        [Required]
        [JsonProperty]
        public bool IsEnabled { get; set; }
    }
}