namespace ToDoApp.Common.Models
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Todo to be inserted.
    /// </summary>
    [JsonObject]
    public class TodoInsert
    {
        /// <summary>
        /// Todo's description.
        /// </summary>
        /// <value>The description.</value>
        [Required]
        [JsonProperty]
        public string Description { get; set; }
    }
}