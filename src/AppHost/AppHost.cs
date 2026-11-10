// =====================================================================================
// Story time — AppHost
//
// One Blazor app, two Microsoft Foundry models, zero keys. Everything is in this file.
//
//   aspire run     -> creates Foundry + both model deployments in Azure (first time) and runs the web locally
//   aspire deploy  -> same models, and the web goes to Azure Container Apps
// =====================================================================================

using Aspire.Hosting.Foundry;

var builder = DistributedApplication.CreateBuilder(args);

// Where the web runs when you deploy. Locally it is ignored.
builder.AddAzureContainerAppEnvironment("env");

// 1. Microsoft Foundry and two model deployments.
//    Local auth is disabled; Aspire creates the role assignments the web needs.
var foundry = builder.AddFoundry("foundry");

var story = foundry.AddDeployment("story", "gpt-6-luna", "2026-09-22", "OpenAI");
var image = foundry.AddDeployment("image", "gpt-image-2.5-flare", "2026-09-08", "OpenAI");

// 2. The Blazor app. WithReference = connection info as environment variables + roles.
builder.AddProject<Projects.Web>("web")
    .WithReference(story).WaitFor(story)
    .WithReference(image).WaitFor(image)
    .WithExternalHttpEndpoints();

builder.Build().Run();
