using Newtonsoft.Json;

namespace Models.Output
{
    public class Beer
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}