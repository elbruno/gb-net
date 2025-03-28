using Azure;
using Azure.AI.Inference;
using Azure.Identity;
using Microsoft.Extensions.AI;
using System.Diagnostics;
using System.Net;
using System.Runtime;
using System.Text;
using System.Text.Json;

namespace GB.AI.Ollama
{
    public class ActionGenerator
    {

        string userPrompt = @"""
You are an expert playing the game ""Galaga"" for Nintendo GameBoy.
You are going to be provided with an image that represents the current game frame plus the description of the previous game frame.

Key Bindings:
- These are the key bindings for the Windows Keys and the GameBoy keys:
 Windows Keys.A -> Button.Left
 Windows Keys.D ->  Button.Right
 Windows Keys.W ->  Button.Up
 Windows Keys.S ->  Button.Down
 Windows Keys.K ->  Button.A
 Windows Keys.O ->  Button.B
 Windows Keys.Enter ->  Button.Start
 Windows Keys.Back -> , Button.Select

Tasks:
- Analyze the image to get details of the current game frame. Perform a detailed analysis including the position of the player ship, the position of the enemies and the trend of where the enemies are moving or firing.
- Use using the current frame information and the previous frame information suggest the action that need to done. The goal is to win the game.
- The suggested actions can be 'Move Right', 'Move Left' or 'Fire'.
- The key to press should a string, in example: 'Keys.A' or 'Keys.D'
- The main keys in this game are 'A' to move left, 'D' to move right and 'K' to fire.
- The expected output should be a JSON object with 3 string fields: 'FrameAnalysis', 'PressKey' and 'SuggestedMove'.
- In the 'SuggestedMove' field, add an explanation on why the suggested move.
- Return only the JSON object as a string.

Rules:
- The lower left small ships are not the player, are the remaining lives.
- Try to kill as much enemies as you can.
- If the player is in the 'Bottom Left' corner of the screen, do not suggest to 'Move Left'.
- If the player is in the 'Bottom Right' corner of the screen, do not suggest to 'Move Right'.
- Only fire when you have enemies on top of the player, or when the enemies are moving towards the player location.

---
Current Frame:
""";

        public async Task<ActionResponse> GenerateNextActionResponse(string imageLocation, string frameAnalysis)
        {
            var jsonResponse = await GenerateNextAction(imageLocation, frameAnalysis);

            // Ensure we have valid JSON before deserializing
            var validJsonResponse = EnsureValidJsonResponse(jsonResponse);

            if (validJsonResponse != null)
            {
                try
                {
                    return JsonSerializer.Deserialize<ActionResponse>(validJsonResponse);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                }
            }

            // Fallback if JSON parsing fails completely
            return new ActionResponse
            {
                FrameAnalysis = frameAnalysis,
                PressKey = "Keys.Q" // Default key
            };
        }


        public async Task<string> GenerateNextAction(string imageLocation, string frameAnalysis)
        {
            // get the media type from the image location
            var mediaType = GetMediaType(imageLocation);

            byte[] imageBytes = File.ReadAllBytes(imageLocation);

            IChatClient chatClient = GetOllamaChatClient();

            List<ChatMessage> messages =
            [
                new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, userPrompt + $"{Environment.NewLine}{frameAnalysis}"),
                new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, [new DataContent(imageBytes, mediaType)])
            ];

            var imageAnalysis = await chatClient.GetResponseAsync(messages);
            Debug.WriteLine($">> Chat: {imageAnalysis.Text}");

            return imageAnalysis.Text;
        }

        private static IChatClient GetOllamaChatClient()
        {
            return new OllamaChatClient(
                new Uri(uriString: "http://localhost:11434/"), "gemma3");
        }


        public string GetMediaType(string imageLocation)
        {
            // Logic to determine the media type based on the file extension
            string extension = Path.GetExtension(imageLocation).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => throw new NotSupportedException($"File extension {extension} is not supported"),
            };
        }

        /// <summary>
        /// Ensures that a string is a valid JSON object containing RecentActivity and PressKey fields.
        /// Handles common formatting issues and returns a properly formatted JSON string.
        /// </summary>
        /// <param name="jsonString">The potentially malformed JSON string</param>
        /// <returns>A valid JSON string or null if conversion is not possible</returns>
        public string EnsureValidJsonResponse(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }

            try
            {
                // First try direct deserialization
                var actionResponse = JsonSerializer.Deserialize<ActionResponse>(jsonString);
                if (actionResponse != null)
                {
                    // If successful, return a properly formatted JSON string
                    return JsonSerializer.Serialize(actionResponse);
                }
            }
            catch (JsonException)
            {
                // Continue with cleanup if direct deserialization fails
            }

            // Clean up common issues
            string cleaned = jsonString.Trim();

            // Remove any leading/trailing text that isn't part of the JSON object
            int startBrace = cleaned.IndexOf('{');
            int endBrace = cleaned.LastIndexOf('}');

            if (startBrace >= 0 && endBrace > startBrace)
            {
                cleaned = cleaned.Substring(startBrace, endBrace - startBrace + 1);
            }

            try
            {
                // Try to deserialize the cleaned JSON
                var actionResponse = JsonSerializer.Deserialize<ActionResponse>(cleaned);

                // If we have valid fields, return properly formatted JSON
                if (actionResponse != null &&
                    !string.IsNullOrWhiteSpace(actionResponse.FrameAnalysis) &&
                    !string.IsNullOrWhiteSpace(actionResponse.PressKey))
                {
                    return JsonSerializer.Serialize(actionResponse);
                }

                return null;
            }
            catch (JsonException)
            {
                // Extract values using regex as a last resort
                try
                {
                    var recentActivityMatch = System.Text.RegularExpressions.Regex.Match(
                        cleaned, @"""RecentActivity""[\s]*:[\s]*""([^""]*)");
                    var pressKeyMatch = System.Text.RegularExpressions.Regex.Match(
                        cleaned, @"""PressKey""[\s]*:[\s]*""([^""]*)");

                    if (recentActivityMatch.Success && pressKeyMatch.Success)
                    {
                        var actionResponse = new ActionResponse
                        {
                            FrameAnalysis = recentActivityMatch.Groups[1].Value,
                            PressKey = pressKeyMatch.Groups[1].Value
                        };

                        return JsonSerializer.Serialize(actionResponse);
                    }
                }
                catch
                {
                    // If regex extraction fails, return null
                }

                return null;
            }
        }

    }
}