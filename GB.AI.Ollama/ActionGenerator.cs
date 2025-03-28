using Microsoft.Extensions.AI;
using System.Runtime;
using System.Text;
using System.Text.Json;

namespace GB.AI.Ollama
{
    public class ActionGenerator
    {

        string userPrompt = @"""
You are an expert playing the game ""the legend of Zelda links awakening"" for Nintendo GameBoy.
You are going to be provided with images that represents the current game frame plus a history of the recent activities.

These are the key bindings:
{Keys.A, Button.Left},
{Keys.D, Button.Right},
{Keys.W, Button.Up},
{Keys.S, Button.Down},
{Keys.K, Button.A},
{Keys.O, Button.B},
{Keys.Enter, Button.Start},
{Keys.Back, Button.Select}

Analyze the current game frame, and using the recent game activity suggest the next action that the console need to perform. That means which key to press. In example: Key.A or Keys.D

The expected output should be a JSON object with 2 string fields:
- RecentActivity
- PressKey

This is the Recent Activity:
""";

        public async Task<ActionResponse> GenerateNextActionResponse(string imageLocation, string recentActivity)
        {
            var actionResponse = new ActionResponse();
            var jsonResponse = await GenerateNextAction(imageLocation, recentActivity);
            // deserialize the JSON response to ActionResponse object
            actionResponse = JsonSerializer.Deserialize<ActionResponse>(jsonResponse);
            return actionResponse;
        }

        public async Task<string> GenerateNextAction(string imageLocation, string recentActivity)
        {
            // get the media type from the image location
            var mediaType = GetMediaType(imageLocation);

            byte[] imageBytes = File.ReadAllBytes(imageLocation);


            List<ChatMessage> messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, userPrompt + $"{Environment.NewLine}{recentActivity}")
            };

            StringBuilder stringBuilder = new StringBuilder();
            Console.WriteLine($"Using Ollama");

            var chatOllama = new OllamaChatClient(
                new Uri(uriString: "http://localhost:11434/"), "gemma3");

            // in ollama the image should be added as byte array
            messages.Add(new ChatMessage(ChatRole.User, [new DataContent(imageBytes, mediaType)]));

            var imageAnalysis = await chatOllama.GetResponseAsync(messages);
            stringBuilder.AppendLine("Ollama: ");
            stringBuilder.AppendLine(imageAnalysis.Message.Text);
            stringBuilder.AppendLine();
            Console.WriteLine($">> Ollama done");
            Console.WriteLine();

            Console.WriteLine();
            return imageAnalysis.Message.Text;
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
    }
}