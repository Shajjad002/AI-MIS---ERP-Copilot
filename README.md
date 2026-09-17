# AI MIS & ERP Copilot

An enterprise-ready foundation for a read-only, natural-language MIS and ERP analytics copilot.

## Sprint 1 complete

- Clean Architecture .NET solution (API, Application, Domain, Infrastructure, Persistence)
- Vue 3 + TypeScript web starter
- User, role, and branch-access domain/database foundations
- Base API, CORS configuration, health/status endpoints, and authentication endpoint placeholder

## Sprint 2 complete

- Provider-neutral, OpenAI-compatible LLM client with a 30-second timeout
- Controlled system prompt and JSON-only structured interpretation
- Approved schema metadata provider and API endpoint
- Initial schema metadata database tables
- Interpretation endpoint that never executes its proposed SQL

### Configure an LLM

Keep credentials outside source control. Set these environment variables before calling `POST /api/copilot/interpret`:

```powershell
$env:Llm__Endpoint = 'https://your-provider.example/v1/chat/completions'
$env:Llm__ApiKey = 'your-secret-key'
$env:Llm__Model = 'your-model-name'
```

Use `GET /api/schema-metadata` to inspect the only schema sent to the LLM. `POST /api/copilot/interpret` accepts `{ "question": "..." }`; it returns an interpretation and optional proposed SQL, but never executes it.

## Run the API

```powershell
dotnet run --project src/AI.MIS.Api
```

## Run the web client

```powershell
cd src/AI.MIS.Web
npm.cmd install
node node_modules/vite/bin/vite.js
```

The repository directory includes an `&`, which Windows' npm script wrapper treats as a command separator. Use the direct `node` command above while the project remains at this path.

`ErpReadOnlyDatabase` is deliberately a placeholder. Do not set it to a write-enabled ERP connection.
