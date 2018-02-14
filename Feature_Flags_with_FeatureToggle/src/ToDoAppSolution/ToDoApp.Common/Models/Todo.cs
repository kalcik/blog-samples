namespace ToDoApp.Common.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Features;
    using Newtonsoft.Json;

    [JsonObject]
    public class Todo
    {
        /// <summary>
        /// Gets or sets a value indicating whether Todo is completed.
        /// </summary>
        /// <value><c>true</c> if this Todo is completed; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        [Required]
        [DisplayName("Completed")]
        [JsonProperty]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the Todo description.
        /// </summary>
        /// <value>The Todo description.</value>
        [Required]
        [JsonProperty]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Todo identifier.
        /// </summary>
        /// <value>The Todo identifier.</value>
        [JsonProperty]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets if <see cref="IsCompleted" /> feature is enabled.
        /// </summary>
        /// <returns><see cref="IsCompletedFeatureFlag" /></returns>
        public IsCompletedFeatureFlag CompletedFeature() => new IsCompletedFeatureFlag();
    }
}