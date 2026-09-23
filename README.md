# AI MIS & ERP Copilot

An enterprise-ready foundation for a read-only, natural-language MIS and ERP analytics copilot.

## Sprint 1 complete

- Clean Architecture .NET solution (API, Application, Domain, Infrastructure, Persistence)
- Vue 3 + TypeScript web starter
- User, role, and branch-access domain/database foundations
- Base API, CORS configuration, health/status endpoints, and JWT login

Configure JWT signing with user secrets or environment variables. User credentials, roles, and branch access are read from the application database; store only PBKDF2 password hashes, never plaintext passwords:

```powershell
$env:Authentication__SigningKey = 'replace-with-at-least-32-random-characters'
$env:ConnectionStrings__ApplicationDatabase = 'Server=...;Database=AiMisErpCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;'
```

Generate a compatible password hash locally without sending the password anywhere:

```powershell
$password = Read-Host 'Admin password' -AsSecureString
$plain = [System.Net.NetworkCredential]::new('', $password).Password
$salt = [Security.Cryptography.RandomNumberGenerator]::GetBytes(16)
$key = [Security.Cryptography.Rfc2898DeriveBytes]::Pbkdf2($plain, $salt, 100000, [Security.Cryptography.HashAlgorithmName]::SHA256, 32)
"$([Convert]::ToBase64String($salt)).$([Convert]::ToBase64String($key))"
```

Run `database/Seed/003_DemoUsers.sql` against `AiMisErpCopilotDb`, then call `POST /api/auth/login` with a seeded username and password to receive a JWT.

## Sprint 2 complete

- Provider-neutral, OpenAI-compatible LLM client with a 30-second timeout
- Controlled system prompt and JSON-only structured interpretation
- Approved schema metadata provider and API endpoint
- Initial schema metadata database tables
- Interpretation endpoint that never executes its proposed SQL

## Sprint 4 complete

- Vue query form connected to `POST /api/copilot/query`
- Loading and error states
- KPI cards for row count and numeric totals
- Responsive result table
- Dependency-free bar, line, pie, and donut-friendly visualizations
- Result summary with row count and numeric totals

Visualization frontend structure:

```text
src/AI.MIS.Web/src/
├── pages/VisualizationPage.vue
├── components/visualization/
│   ├── QueryComposer.vue
│   ├── ResultChart.vue
│   ├── ResultSummary.vue
│   └── ResultTable.vue
├── composables/useCopilotQuery.ts
└── types/copilot.ts
```

Authentication frontend structure:

```text
src/AI.MIS.Web/src/
├── pages/LoginPage.vue
├── composables/useAuth.ts
└── types/auth.ts
```

### Seeded development users

```text
admin / Admin@123
mis.analyst / Mis@123
branch.0212 / Branch@123
```

Change these development-only passwords before any deployment outside local development.

## Sprint 5 complete

- Fixed-window rate limiting on `POST /api/copilot/query` (20 requests/minute)
- Structured query audit logging for success, clarification, rejection, and failure outcomes
- Audit table schema for persisting query metadata without result rows
- Database-backed JWT authentication with role and branch claims
- JWT validation middleware and authorization on the query endpoint
- Branch users must include authorized `BranchCode` equality predicates

### Configure an LLM and read-only ERP database

Keep credentials outside source control. Set these environment variables before calling `POST /api/copilot/interpret`:

```powershell
$env:Llm__Endpoint = 'https://your-provider.example/v1/chat/completions'
$env:Llm__ApiKey = 'your-secret-key'
$env:Llm__Model = 'your-model-name'
```

For a local GPT4All API server, no API key is required:

```powershell
$env:Llm__Endpoint = 'http://localhost:4891/v1/chat/completions'
$env:Llm__Model = 'Phi-3 Mini Instruct'
$env:Llm__ApiKey = ''
$env:ConnectionStrings__ErpReadOnlyDatabase = 'Server=...;Database=...;User Id=...;Password=...;Encrypt=True;'
```

Use `GET /api/schema-metadata` to inspect the only schema sent to the LLM. `POST /api/copilot/interpret` accepts `{ "question": "..." }`; it returns an interpretation and optional proposed SQL, but never executes it.
Use `POST /api/copilot/query` with the same request to validate and execute a proposed query. The query must be one approved, read-only `SELECT` statement and execution is limited to 15 seconds by default. Configure a read-only database principal; application validation is defense in depth, not a replacement for database permissions.

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
