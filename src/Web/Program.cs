using System.ClientModel.Primitives;
using Azure.Identity;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;
using Web;
using Web.Components;

// Spans for every model call in the Aspire dashboard (experimental in the OpenAI SDK).
AppContext.SetSwitch("OpenAI.Experimental.EnableOpenTelemetry", true);

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Aspire injects STORY_URI / STORY_MODELNAME and IMAGE_URI / IMAGE_MODELNAME.
// No keys anywhere: your az login locally, the app's managed identity in Azure.
var auth = new BearerTokenPolicy(new DefaultAzureCredential(), "https://ai.azure.com/.default");

builder.Services.AddSingleton(new ChatClient(
    model: Setting("STORY_MODELNAME"),
    authenticationPolicy: auth,
    options: new OpenAIClientOptions { Endpoint = new Uri($"{Setting("STORY_URI")}openai/v1/") }));

builder.Services.AddSingleton(new ImageClient(
    model: Setting("IMAGE_MODELNAME"),
    authenticationPolicy: auth,
    options: new OpenAIClientOptions { Endpoint = new Uri($"{Setting("IMAGE_URI")}openai/v1/") }));

builder.Services.AddSingleton<StoryTeller>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

string Setting(string name) =>
    builder.Configuration[name] ?? throw new InvalidOperationException($"{name} is missing. Run the app with `aspire run`.");
