using System.Text.Json.Serialization;

namespace GB.AI.Ollama
{
    public class ActionResponse
    {
        [JsonPropertyName("FrameAnalysis")]
        public string FrameAnalysis { get; set; }

        [JsonPropertyName("PressKey")]
        public string PressKey { get; set; }

        [JsonPropertyName("SuggestedMove")]
        public string SuggestedMove { get; set; }
    }
}
