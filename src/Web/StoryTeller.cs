using OpenAI.Chat;
using OpenAI.Images;

namespace Web;

/// <summary>
/// Two models, one job each: the text model writes the story and describes the picture;
/// the image model draws it.
/// </summary>
public sealed class StoryTeller(ChatClient chat, ImageClient images)
{
    private const string Instructions = """
        You write very short stories for all ages, 120 words at most.
        Reply with the story, then one last line that starts with "PICTURE:" and describes
        a single illustration for it (characters, place, mood). No text in the picture.
        """;

    public async Task<Story> TellAsync(string idea, CancellationToken ct = default)
    {
        // 1. gpt-6-luna: the story + what to draw.
        ChatCompletion completion = await chat.CompleteChatAsync(
            [new SystemChatMessage(Instructions), new UserChatMessage(idea)],
            cancellationToken: ct);

        var parts = completion.Content[0].Text.Split("PICTURE:", 2);
        var text = parts[0].Trim();
        var picture = parts.Length > 1 ? parts[1].Trim() : idea;

        // 2. The image model: draws what the text model described.
        GeneratedImage image = await images.GenerateImageAsync(
            $"Storybook illustration, soft warm colours, no text. {picture}",
            new ImageGenerationOptions { Size = GeneratedImageSize.W1024xH1024 },
            ct);

        var dataUrl = $"data:image/png;base64,{Convert.ToBase64String(image.ImageBytes.ToArray())}";
        return new Story(text, dataUrl);
    }
}

public sealed record Story(string Text, string ImageDataUrl);
