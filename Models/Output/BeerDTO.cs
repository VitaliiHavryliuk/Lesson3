using Newtonsoft.Json;

namespace Models.Output
{
    public class BeerDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
        public byte[] Image { get; set; }
    }
}