using System.Text.Json.Serialization;

namespace GB.AI.Ollama
{
    public class ActionResponse
    {
        [JsonPropertyName("RecentActivity")]
        public string RecentActivity { get; set; }

        [JsonPropertyName("PressKey")]
        public string PressKey { get; set; }
    }
}
