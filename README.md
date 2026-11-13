# Aspire + Microsoft Foundry: two models, one AppHost, zero keys

> A Blazor story generator on Microsoft Foundry, wired with Aspire
> Talk for the **Global AI Sarasota Kickoff 2026** · online · November 13, 2026 · 25 min (20 + 5 Q&A)
> Speaker: **Emiliano Montesdeoca del Puerto**, Cloud Solutions Team Lead @ Intelequia

This repo has everything from the session: the slides (MARP), a minimal demo where a Blazor app writes a
short story with one Foundry model and illustrates it with another, plus the speaker notes embedded in the slides.

```
foundry-aspire-blazor-global-ai-sarasota-2026/
├── README.md             ← you are here: what, why, how to run, how to present
├── slides/
│   ├── deck.md           ← MARP deck, 11 slides, speaker notes with timings
│   └── assets/           ← Sarasota logo, speaker/MVP badges and Space Grotesk font (OFL)
└── src/
    ├── StoryTime.slnx            ← solution (.NET 10)
    ├── Directory.Build.props     ← TFM, nullable, NoWarn for experimental APIs
    ├── AppHost/                  ← THE INFRASTRUCTURE: Foundry, two models, the web (~20 lines)
    ├── ServiceDefaults/          ← OpenTelemetry + health checks (minimal)
    └── Web/                      ← Blazor app: one page, one service (StoryTeller.cs)
```

---

## 1. The talk in one paragraph

Before the first line of real code, every AI app needs the same chores: create a Foundry resource,
deploy the models, copy endpoints and keys into config, and explain all of it to the next person who
clones the repo. This talk replaces those chores with a single **Aspire AppHost**: it declares
Microsoft Foundry, a text model (`gpt-6-luna`) and an image model (`gpt-image-2.5-flare`), and hands
them to a Blazor app with **roles instead of keys**. `aspire run` works against the real models from
your laptop; `aspire deploy` ships the same thing to Azure Container Apps.

The idea to take home: **the AppHost describes your app and everything it needs, so a new teammate
goes from clone to running in one command, and there is no key to leak.**

---

## 2. The demo: one page, two models

On purpose, very simple: **one Blazor page, one service, one AppHost**.

| Resource | What it is | Where it runs |
|---|---|---|
| `foundry` | Microsoft Foundry account (no keys: local auth disabled, roles created by Aspire) | Azure |
| `story` | Deployment of `gpt-6-luna` (version `2026-09-22`): writes the story and describes the picture | Azure |
| `image` | Deployment of `gpt-image-2.5-flare` (version `2026-09-08`): draws the picture | Azure |
| `web` | Blazor app (interactive server). You type an idea, it shows the story and its illustration | Your machine with `aspire run`; Azure Container Apps with `aspire deploy` |
| `env` | Azure Container Apps environment | Only on `aspire deploy` |

**Why two models.** An image model doesn't chat: it takes a description and returns an image. So
`gpt-6-luna` writes the story and ends with a `PICTURE:` line describing one illustration; the image
model draws that line. See `src/Web/StoryTeller.cs`.

Nothing is stored: the image comes back as base64 and is shown inline. No database, no storage.

The web doesn't know Aspire exists. It reads environment variables:

| Variable | Set by | Example |
|---|---|---|
| `STORY_URI`, `IMAGE_URI` | Aspire (`WithReference`) | `https://<foundry>.services.ai.azure.com/` |
| `STORY_MODELNAME`, `IMAGE_MODELNAME` | Aspire | `story`, `image` (deployment names, not model names) |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | Aspire (locally) | the dashboard |

Authentication is `DefaultAzureCredential`: your `az login` locally, the app's managed identity in Azure.

The two demos:

| # | What it shows | Command |
|---|---|---|
| 1 | Foundry + both models created from the AppHost, the web running locally, a story with its picture, no key in the environment, and the trace `chat gpt-6-luna → images/generations` | `aspire run` |
| 2 | The same app in Azure Container Apps, with managed identity | `aspire deploy` |

---

## 3. Requirements

- **.NET 10 SDK** and the **Aspire CLI 13.6** or later (install it from aspire.dev). Check with `aspire --version`.
- **Azure CLI** signed in (`az login`) to the tenant where the resources will be created.
- **Docker** (or Podman) for `aspire deploy`.
- An **Azure subscription** where your user can create resources **and assign roles**
  (Owner, or Contributor + User Access Administrator).
- A **region** with both `gpt-6-luna` and `gpt-image-2.5-flare`, and **quota** for both. The AppHost
  uses `swedencentral`; check the Foundry region availability table before trusting it.

NuGet packages:

| Project | Packages | Why |
|---|---|---|
| `AppHost` | `Aspire.Hosting.Foundry` (formerly `Aspire.Hosting.Azure.AIFoundry`), `Aspire.Hosting.Azure.AppContainers` | Hosting: describe Foundry, the models and where the web runs |
| `Web` | `OpenAI`, `Azure.Identity` | Client: call the models with a token, not a key |
| `ServiceDefaults` | `OpenTelemetry.*` | Traces, metrics and logs to the dashboard |

Why not `Aspire.OpenAI` in the web? Aspire's docs recommend it for Foundry Local; cloud Foundry uses
managed identity, which that integration doesn't support, so the docs use the OpenAI SDK directly.

---

## 4. Configure Azure (once)

Region, resource group and credential source are in `src/AppHost/appsettings.json`. The subscription
goes in user secrets:

```bash
cd src/AppHost
dotnet user-secrets set "Azure:SubscriptionId" "<your-subscription-id>"
```

If something is missing, Aspire asks for it in the dashboard at startup.

---

## 5. Run it locally (demo 1)

```bash
cd src
aspire run
```

The **first time**, Aspire creates the Foundry account and both model deployments in Azure. It takes a
few minutes. After that it reuses them.

In the dashboard (the URL with its token is printed in the console):

1. **Resources**: `foundry`, `story`, `image` and `web`.
2. `web` → **Environment**: `STORY_URI`, `STORY_MODELNAME`, `IMAGE_URI`, `IMAGE_MODELNAME`. No key.
3. Open the `web` URL and type an idea: *"A lighthouse keeper who is afraid of the dark"*.
4. Wait a few seconds: the story appears with its illustration.
5. **Traces**: open the request. You'll see the chat call to `gpt-6-luna` and the HTTP call to `images/generations`.

---

## 6. Deploy to Azure (demo 2)

```bash
cd src
aspire deploy
```

Aspire builds the web image, pushes it to a container registry, creates the Container Apps environment
and the app, and gives the app's managed identity the roles it needs on Foundry. Open the public URL it
prints and ask for another story.

To see the generated infrastructure without deploying: `aspire publish -o infra` (Bicep in `infra/`,
which is git-ignored).

**Clean up:** delete the `rg-global-ai-sarasota-2026` resource group when you're done.

---

## 7. Slides (MARP)

`slides/deck.md`, for [Marp](https://marp.app). Cover, bio, a two-slide hook on the pieces behind an
application, the problem and old workflow, AppHost architecture, client code, one demo-time divider for
three live demos, summary and Q&A.
Speaker notes (in Spanish) with **timings for 25 minutes**.

The public presentation is [available on GitHub Pages](https://emimontesdeoca.github.io/foundry-aspire-blazor-global-ai-sarasota-2026/).
It is rebuilt from `slides/deck.md` whenever the slides or publishing workflow change on `main`.

```bash
cd slides
npx @marp-team/marp-cli deck.md -o deck.html --allow-local-files   # press P: presenter view
npx @marp-team/marp-cli deck.md --pdf -o deck.pdf --allow-local-files
npx @marp-team/marp-cli -s .                                        # live server while editing
```

The bio photo loads from GitHub; to present offline, download it into `slides/assets/` and change the path.
The deck loads Space Grotesk from `slides/assets/`; its OFL license is included alongside the font.

### Structure and timings (25')

| Minute | Section | Slides |
|---|---|---|
| 0:00–1:10 | Cover, bio and application pieces | 1–4 |
| 1:10–2:30 | The problem and old workflow | 5 |
| 2:30–5:00 | What Aspire is, what `aspire run` does | 6–7 |
| 5:00–7:30 | AppHost orchestration and client code | 8 |
| 7:30–19:25 | **It's demo time**: run locally, observability and Azure | 9 |
| 19:25–20:00 | Summary | 10 |
| 20:00–25:00 | Questions | 11 |

---

## 8. Troubleshooting

| Symptom | What to do |
|---|---|
| `aspire: command not found` | Install the CLI (aspire.dev) and open a new terminal |
| "is for evaluation purposes only" on build | Add the id it prints to `<NoWarn>` in `src/Directory.Build.props` |
| `AddFoundry` doesn't exist | Package `Aspire.Hosting.Foundry` (not the old `Aspire.Hosting.Azure.AIFoundry`) and `using Aspire.Hosting.Foundry;` |
| `AddDeployment` with strings doesn't compile | Use the overload your version has, or a generated `FoundryModel.OpenAI.*` descriptor if one exists for the model |
| `AddAzureContainerAppEnvironment` doesn't exist | Missing `Aspire.Hosting.Azure.AppContainers` in the AppHost |
| Aspire asks for subscription/region | See §4: `Azure:SubscriptionId` in user secrets |
| Permission error while provisioning | Your user must be able to create resources **and** assign roles |
| A model deployment fails | Region without that model, no quota, or the subscription needs access. Try another region from the availability table, or `gpt-image-1-mini` / `gpt-image-2` for images |
| 401 / `AuthenticationFailed` in the web | `az login` in the right tenant; the AppHost uses `CredentialSource: AzureCli` |
| 404 `DeploymentNotFound` | `*_MODELNAME` must be the **deployment** name (`story`, `image`), not the model name |
| `ImageClient` has no constructor with `AuthenticationPolicy` | Update the `OpenAI` package; older versions only take a key |
| `ImageBytes` is empty | The model returned a URL instead of base64: read `image.ImageUri` and show that |
| The picture takes long or times out | Normal for several seconds. Over a minute: check the `image` deployment quota |
| 429 | Raise the deployment capacity: `.WithProperties(d => d.SkuCapacity = 50)` |
| No model spans in Traces | Check `AppContext.SetSwitch("OpenAI.Experimental.EnableOpenTelemetry", true)` in `Program.cs` and the `OpenAI.*` source in `Extensions.cs`; HTTP spans show up anyway |
| `aspire deploy` fails pushing the image | Docker running; `linux/amd64` image (matters on Apple Silicon) |
| Deployed app returns 401 from Foundry | Wait a few minutes for the role assignment to propagate, then restart the revision |

The code follows the Aspire 13.6 and Microsoft Foundry docs as of November 2026 and **has not been
compiled**: run it end to end before the talk. The Foundry integration in Aspire is in preview and
changes between versions.

---

## 9. References

- Aspire: Foundry hosting integration (AppHost) — https://aspire.dev/integrations/cloud/azure/azure-ai-foundry/azure-ai-foundry-host/
- Aspire: connect apps to Foundry (environment variables, OpenAI SDK, no keys) — https://aspire.dev/integrations/cloud/azure/azure-ai-foundry/azure-ai-foundry-connect/
- Aspire: deploy to Azure Container Apps — https://aspire.dev/deployment/azure/container-apps/
- Aspire: configure Azure Container Apps environments — https://aspire.dev/integrations/cloud/azure/configure-container-apps/
- Foundry model catalog: gpt-6-luna — https://ai.azure.com/catalog/models/gpt-6-luna
- Foundry model catalog: gpt-image-2.5-flare — https://ai.azure.com/catalog/models/gpt-image-2.5-flare
- Foundry REST: create an image (supported image models) — https://learn.microsoft.com/en-us/azure/foundry/reference/rest/images-create-image
- Foundry: model availability by region — https://learn.microsoft.com/azure/foundry/foundry-models/concepts/models-sold-directly-by-azure-region-availability
---

Stories and pictures are generated by AI on the spot; nothing is stored. MIT license: take the code and change the prompt.
