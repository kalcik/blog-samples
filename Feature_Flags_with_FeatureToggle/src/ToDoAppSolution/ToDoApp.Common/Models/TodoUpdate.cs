namespace ToDoApp.Common.Models
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Todo to be updated.
    /// </summary>
    [JsonObject]
    public class TodoUpdate
    {
        /// <summary>
        /// Todo's description.
        /// </summary>
        /// <value>The description.</value>
        [Required]
        [JsonProperty]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Todo is completed.
        /// </summary>
        /// <value><c>true</c> if this Todo is completed; otherwise, <c>false</c>.</value>
        [JsonProperty]
        public bool IsCompleted { get; set; }
    }
}