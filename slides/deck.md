---
marp: true
theme: default
paginate: true
transition: fade
size: 16:9
lang: en
title: "Aspire + Microsoft Foundry: two models, one AppHost, zero keys"
description: "A Blazor app that writes a story with gpt-6-luna and illustrates it with an image model, both on Microsoft Foundry, wired with Aspire. Global AI Sarasota Kickoff 2026."
header: '<img src="assets/sarasota.png" alt="Global AI Sarasota">'
footer: "Emiliano Montesdeoca · Aspire + Foundry, zero keys"
style: |
  @font-face {
    font-family: "Space Grotesk";
    src: url("assets/SpaceGrotesk-VariableFont_wght.ttf") format("truetype");
    font-style: normal;
    font-weight: 300 700;
    font-display: swap;
  }
  :root {
    --gain-navy: #071c3b;
    --gain-night: #051931;
    --gain-orange: #d5522b;
    --gain-orange-text: #a43a1f;
    --gain-orange-light: #f4a181;
    --gain-blue: #8cadc6;
    --gain-ink: #17283e;
    --gain-surface: #f3f6f9;
    --gain-muted: #566a80;
  }
  section {
    font-family: "Space Grotesk", "Segoe UI", "Inter", system-ui, sans-serif;
    font-size: 30px;
    line-height: 1.4;
    padding: 6px 72px 76px;
    background: #ffffff;
    color: var(--gain-ink);
  }
  section h1 { font-size: 1.8em; line-height: 1.14; color: var(--gain-navy); letter-spacing: -0.025em; }
  section.lead h1.cover-title { font-size: 2.5em; line-height: 1.05; }
  section h2 { font-size: 1.42em; line-height: 1.2; color: var(--gain-navy); margin-bottom: 0.6em; }
  section strong { color: var(--gain-navy); }
  section a { color: var(--gain-orange-text); text-decoration-color: var(--gain-orange); text-underline-offset: 0.1em; }
  section li { margin: 0.4em 0; }
  section code { font-size: 0.82em; background: var(--gain-surface); padding: 0.1em 0.25em; border-radius: 4px; }
  section pre { font-size: 0.75em; line-height: 1.4; background: var(--gain-surface); border-left: 4px solid var(--gain-orange); padding: 0.65em 0.85em; }
  section pre code { font-size: 1em; background: transparent; padding: 0; }
  section header { top: 30px; right: 72px; left: auto; width: 220px; text-align: right; }
  section header p { margin: 0; }
  section header img {
    display: block;
    width: auto;
    height: 140px;
    max-width: 100%;
    margin-left: auto;
    object-fit: contain;
    border-radius: 8px;
  }
  section.bio header img { height: 72px; }
  section.compact-logo header img { height: 55px; }
  section footer { left: 72px; bottom: 18px; font-size: 14px; color: var(--gain-muted); }
  section::after { right: 72px; bottom: 18px; font-size: 14px; color: var(--gain-muted); }
  section.lead { background: linear-gradient(135deg, var(--gain-navy) 0%, var(--gain-night) 100%); color: #fff; }
  section.lead h1, section.lead strong, section.lead a { color: #fff; }
  section.lead h2 { color: var(--gain-orange-light); }
  section.lead p { color: #e4edf5; }
  section.lead footer, section.lead::after { color: var(--gain-blue); }
  section.demo h2::before { content: "DEMO / "; color: var(--gain-orange-text); font-size: 0.55em; letter-spacing: 0.08em; }
  section.demo pre { font-size: 0.68em; }
  section.transform-before h2::before {
    content: "BEFORE / ";
    color: var(--gain-orange-text);
    font-size: 0.55em;
    letter-spacing: 0.08em;
  }
  section.architecture .architecture-columns {
    display: grid;
    grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
    align-items: start;
    gap: 1em;
  }
  section.architecture .architecture-columns h3 {
    margin: 0 0 0.35em;
    font-size: 0.9em;
    line-height: 1.2;
    color: var(--gain-navy);
  }
  section.architecture pre {
    margin: 0;
    font-size: 0.55em;
    line-height: 1.22;
    padding: 0.5em 0.55em;
  }
  section.architecture .small {
    margin: 0.5em 0 0;
    font-size: 0.68em;
    line-height: 1.3;
  }
  section.bio .bio-layout {
    display: grid;
    grid-template-columns: minmax(0, 1.15fr) minmax(0, 0.85fr);
    align-items: center;
    gap: 1.2em;
  }
  section.bio h2 { margin-top: 0.45em; margin-bottom: 0.25em; }
  section.bio .bio-copy { align-self: start; font-size: 0.82em; line-height: 1.3; }
  section.bio .bio-copy ul { margin: 0.5em 0 0.8em; padding-left: 1.1em; }
  section.bio .bio-copy li { margin: 0.45em 0; line-height: 1.28; }
  section.bio .bio-copy strong { color: var(--gain-orange-text); }
  section.bio .bio-badges {
    display: flex;
    align-items: center;
    gap: 0.6em;
    margin-top: 0.6em;
  }
  section.bio .bio-badges img {
    display: block;
    width: auto;
    height: 96px;
    object-fit: contain;
  }
  section.bio .bio-badges img.mvp {
    height: 50px;
  }
  section.bio .bio-photo {
    display: block;
    width: 100%;
    height: 420px;
    border-radius: 10px;
    object-fit: cover;
    object-position: center 32%;
  }
  section table { font-size: 0.78em; width: 100%; }
  section th { background: var(--gain-navy); color: #fff; }
  section blockquote { border-left: 5px solid var(--gain-orange); padding-left: 0.7em; color: #42566f; }
  .metric { font-size: 1.5em; font-weight: 700; line-height: 1.2; color: var(--gain-navy); }
  .cols { display: grid; grid-template-columns: 1fr 1fr; gap: 1.6em; }
  .small { font-size: 0.75em; color: var(--gain-muted); }
---

<!-- _class: lead -->
<!-- _paginate: false -->

<h1 class="cover-title">Aspire + Microsoft Foundry</h1>

## Two models, one AppHost, zero keys

**Emiliano Montesdeoca del Puerto**
Global AI Sarasota Kickoff 2026 · November 13 · Online

<!--
TIEMPO: 0:00 – 0:20

11 diapositivas: 10 de contenido y una de cierre.
Objetivo: 20 minutos de contenido y 5 de preguntas. La sesión es de 25 minutos con las preguntas
incluidas. Las tres demos son el centro de la charla.

Una frase: una web Blazor que pide una historia a gpt-6-luna y un dibujo a un modelo de imágenes,
los dos en Foundry, todo descrito en un AppHost de Aspire y sin una sola clave.

Es el lanzamiento de una comunidad nueva: dar la enhorabuena a la organización en una frase.
-->

---

<!-- _class: bio -->

## Hi, I'm Emi

<div class="bio-layout">
  <div class="bio-copy">
    <ul>
      <li>Born in Uruguay, Canarian at heart</li>
      <li>Cloud Solutions Team Lead · Intelequia</li>
      <li>Microsoft MVP in Developer Technologies since 2022</li>
      <li>Blogger / wannabe influencer</li>
    </ul>
    <div class="bio-badges">
      <img src="assets/most-active-speaker-2025.svg" alt="Most Active Speaker 2025">
      <img src="assets/most-active-speaker-2024.svg" alt="Most Active Speaker 2024">
      <img src="assets/most-active-speaker-2023.svg" alt="Most Active Speaker 2023">
      <img class="mvp" src="assets/microsoft-mvp.png" alt="Microsoft MVP">
    </div>
  </div>
  <img class="bio-photo" src="https://avatars.githubusercontent.com/u/13499480?v=4" alt="Emi's profile photo">
</div>

<!--
TIEMPO: 0:20 – 1:00

Breve. Lo único que quiero que recuerden: ya no programo cada día; lidero un equipo, y lo que más me
importa es que cualquiera del equipo pueda clonar un repo de IA y ejecutarlo el primer día.
-->

---

<!-- _class: transform-before -->

## The problem

Every AI app needs the same setup before useful code:

1. Create Foundry and deploy both models in the portal.
2. Copy endpoints and deployment names into config; keys end up in `appsettings.json`, Slack or someone's `.env`.
3. Write down the steps so the next person repeats them.

**Works once. For one person. The next clone starts from zero.**

<!--
TIEMPO: 1:00 – 2:30

Pregunta al chat: ¿quién ha pegado alguna vez una API key en appsettings "solo para probar"?
Esperar dos segundos. Todos.

Esto lo hemos hecho todos. Como Team Lead, lo que me preocupa no es el portal: es que los pasos
vivan en la cabeza de una persona, que las claves se muevan por el chat y que el siguiente repita
todo. Aspire deja esa infraestructura descrita en el AppHost.
-->

---

<!-- _class: compact-logo -->

## What Aspire is

<div class="cols">
<div>

**A code-first way to describe your app and everything around it.**

- **AppHost**: a C# file with your projects, databases, AI models…
- **`aspire run`**: starts everything, wires it together
- **Dashboard**: resources, logs, traces
- **`aspire deploy`**: the same model, to Azure

</div>
<div>

```csharp
var builder = DistributedApplication
    .CreateBuilder(args);

var db = builder.AddPostgres("pg");

builder.AddProject<Projects.Web>("web")
       .WithReference(db)
       .WaitFor(db)
       .WithExternalHttpEndpoints();

builder.Build().Run();
```

</div>
</div>

<!--
TIEMPO: 2:30 – 4:00

Para quien no lo conoce: el AppHost es un programa de C#. No es YAML. Cada AddX es un recurso;
WithReference es "este proyecto necesita aquello" y Aspire le pasa la conexión como variables
de entorno. WaitFor espera a que la base de datos esté lista y WithExternalHttpEndpoints marca los
endpoints HTTP de la web como externos. Hoy en vez de Postgres pondremos Foundry.
El ejemplo de Postgres es solo para la idea.
-->

---

## What happens on `aspire run`

1. Reads the AppHost: **what** exists and **who needs what**
2. Creates the Azure resources that don't exist yet (first run only)
3. Gives you **roles**, not keys, on those resources
4. Starts your projects with the connection info as **environment variables**
5. Opens the **dashboard**: resources, console, structured logs, traces

<!--
TIEMPO: 4:00 – 5:00

Punto 2: la primera vez tarda unos minutos porque crea cosas en Azure de verdad. Por eso hoy
lo traigo ya creado. Punto 3 es la clave de la charla: la cuenta de Foundry se crea con la
autenticación local desactivada y Aspire asigna los roles. No existe ninguna clave que filtrar.
-->

---

<!-- _class: architecture -->

## One AppHost, two sides

<div class="architecture-columns">
<div>

### The orchestrating side

```csharp
builder.AddAzureContainerAppEnvironment("env");

var foundry = builder.AddFoundry("foundry");

var story = foundry.AddDeployment(
    "story",
    "gpt-6-luna",
    "2026-09-22",
    "OpenAI");

builder.AddProject<Projects.Web>("web")
    .WithReference(story).WaitFor(story)
    .WithExternalHttpEndpoints();
```

</div>
<div>

### The client side: no keys

```csharp
var auth = new BearerTokenPolicy(
    new DefaultAzureCredential(),
    "https://ai.azure.com/.default");

builder.Services.AddSingleton(new ChatClient(
    model: config["STORY_MODELNAME"],
    authenticationPolicy: auth,
    options: new()
    {
        Endpoint = new(
            $"{config["STORY_URI"]}openai/v1/")
    }));
```


</div>
</div>

<!--
TIEMPO: 5:00 – 7:30

A la izquierda, el AppHost define el entorno, Foundry, los deployments y las referencias a la web.
A la derecha, la web consume las variables que Aspire inyecta y usa DefaultAzureCredential: no hay
API keys. Localmente se autentica con az login; en Azure, con la identidad gestionada.
Volver al BEFORE: los pasos del portal y las claves se sustituyen por código y WithReference.
-->

---

<!-- _class: demo -->

## 1 · Run locally → ```aspire run```

1. Dashboard: `foundry`, `story`, `image`, `web`
2. `web` → **Environment**: `STORY_URI`, `IMAGE_MODELNAME`… no key
3. Open the web: *"A lighthouse keeper who is afraid of the dark"*
4. The story… and its picture

<!--
TIEMPO: 7:30 – 14:55

Antes de empezar: Foundry y los dos despliegues YA creados (aspire run de la mañana), az login
hecho, la web calentada con una historia (la primera llamada tarda más).
Guion:
- Dashboard → Resources. Señalar foundry, story, image (en Azure) y web (en mi máquina).
- web → Environment. Buscar "KEY": no hay. Solo URI y nombres de despliegue.
- Abrir la web. Pedir al chat una idea; si nadie dice nada, la del faro.
- Mientras dibuja (unos segundos): "esto es el modelo de imagen trabajando".
Si falla: un intento de un minuto, luego vídeo de respaldo.
-->

---

<!-- _class: demo -->

## 2 · Observability

- Dashboard → **Traces** → the `POST` of the story
- `chat gpt-6-luna` → then `images/generations`
- Tokens, duration, which model, in order

<p class="metric">Observability from the first run. Zero setup.</p>

<!--
TIEMPO: 14:55 – 16:25

Abrir la traza de la petición. Primero el span del chat (luna), luego la llamada HTTP a
images/generations. Enseñar cuál tarda más (la imagen). Esto sale de ServiceDefaults y del
switch de telemetría del SDK de OpenAI; no he escrito nada de telemetría a mano.
-->

---

<!-- _class: demo -->

## 3 · Now in Azure → ```aspire deploy```


1. Builds the web image, pushes it to a registry
2. Creates the Container Apps environment + the app
3. Managed identity + roles on Foundry
4. Open the public URL → same story, same picture

<!--
TIEMPO: 16:25 – 19:25

NO desplegar en directo desde cero: tarda varios minutos. Plan: lanzar `aspire deploy` y enseñar
la salida mientras habla, o enseñar el despliegue de la mañana (consola + URL pública).
Pedir otra historia en la URL pública. Punto clave: el mismo código, ninguna clave; ahora la
identidad es la de la app en Azure.
Si falla: vídeo de respaldo del deploy.
-->

---

## Summary

- **AppHost:** `AddFoundry`, `AddDeployment` and `WithReference`.
- **Web:** `DefaultAzureCredential`; no keys.
- **Run locally:** `aspire run`.
- **Deploy to Azure:** `aspire deploy`.

**Repo:** [foundry-aspire-blazor-global-ai-sarasota-2026](https://github.com/emimontesdeoca/foundry-aspire-blazor-global-ai-sarasota-2026)

<!--
TIEMPO: 19:25 – 20:00

La slide para la foto. Terminar el contenido en el minuto 20 y pasar a preguntas.
-->

---

<!-- _class: lead -->
<!-- _paginate: false -->

# Thank you

## Questions?

**Emiliano Montesdeoca**
linkedin.com/in/emimontesdeoca · @emimontesdeocaa

<!--
TIEMPO: 20:00 – 25:00

Cinco minutos de preguntas. Dejar esta slide puesta.

Preguntas previsibles:
- ¿Necesito Visual Studio? No: .NET 10 SDK + CLI de Aspire, vale VS Code o la terminal.
- ¿Funciona con Python o TypeScript? Sí: el AppHost puede ser TypeScript y los clientes Python/JS
  leen las mismas variables (STORY_URI, STORY_MODELNAME).
- ¿Y si mi empresa no me deja asignar roles? Tu usuario necesita crear recursos y asignar roles; si
  no, que alguien cree Foundry y lo usas con `AsExisting`.
- ¿Por qué no Aspire.OpenAI? Porque no soporta identidad gestionada; para Foundry en la nube la doc
  de Aspire usa el SDK de OpenAI directamente.
- ¿Cuánto cuesta? Los modelos se pagan por uso (tokens y por imagen); Container Apps escala a cero.
  Borra el grupo de recursos al terminar.

Si no sabes una respuesta: "I'll answer you on LinkedIn". Y hacerlo.
-->
