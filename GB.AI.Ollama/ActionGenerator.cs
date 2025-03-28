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
You are going to be provided with an image that represents the current game frame plus a history of the previous game frames description.

These are the key bindings for the Windows Keys and the GameBoy keys:
{Keys.A, Button.Left},
{Keys.D, Button.Right},
{Keys.W, Button.Up},
{Keys.S, Button.Down},
{Keys.K, Button.A},
{Keys.O, Button.B},
{Keys.Enter, Button.Start},
{Keys.Back, Button.Select}

Analyze in details the current game frame, and using the recent game activity suggest the next key that need to be pressed. The goal is to win the game.
The key to press should a string, in example: 'Keys.A' or 'Keys.D'
Generate a recent activity history that includes a complete description of the current game frame, and the previous game frame.

The expected output should be a JSON object with 2 string fields:
- RecentActivity
- PressKey

Rules:
- The lower left small ships are note the player, are the remaining lives

Return only the JSON object as a string.

---
This is the Recent Activity:
""";

        public async Task<ActionResponse> GenerateNextActionResponse(string imageLocation, string recentActivity)
        {
            var jsonResponse = await GenerateNextAction(imageLocation, recentActivity);

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
                RecentActivity = recentActivity,
                PressKey = "Keys.S" // Default key
            };
        }


        public async Task<string> GenerateNextAction(string imageLocation, string recentActivity)
        {
            // get the media type from the image location
            var mediaType = GetMediaType(imageLocation);

            byte[] imageBytes = File.ReadAllBytes(imageLocation);

            IChatClient chatClient = GetAoaiChatClient();

            List<ChatMessage> messages =
            [
                new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, userPrompt + $"{Environment.NewLine}{recentActivity}"),
                new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, [new DataContent(imageBytes, mediaType)])
            ];

            var imageAnalysis = await chatClient.GetResponseAsync(messages);
            Debug.WriteLine($">> Chat: {imageAnalysis.Text}");

            return imageAnalysis.Text;
        }

        private static IChatClient GetOllamaChatClient()
        {
            // OllamaChatClient is a wrapper around the Ollama API
            var chatOllama = new OllamaChatClient(
            //new Uri(uriString: "http://localhost:11434/"), "llama3.2-vision");
            new Uri(uriString: "http://localhost:11434/"), "gemma3");
            //new Uri(uriString: "http://localhost:11434/"), "granite3.2-vision");
            return chatOllama;
        }

        //private static IChatClient GetAoaiChatClient()
        //{

        //    var client = new ChatCompletionsClient(
        //        endpoint: new Uri(endpoint), 
        //        credential: new AzureKeyCredential(apiKey))
        //        .AsChatClient("gpt-4o-mini");

        //    return client;
        //}

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
                    !string.IsNullOrWhiteSpace(actionResponse.RecentActivity) &&
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
                            RecentActivity = recentActivityMatch.Groups[1].Value,
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