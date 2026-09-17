# AI MIS & ERP Copilot

An enterprise-ready foundation for a read-only, natural-language MIS and ERP analytics copilot.

## Sprint 1 complete

- Clean Architecture .NET solution (API, Application, Domain, Infrastructure, Persistence)
- Vue 3 + TypeScript web starter
- User, role, and branch-access domain/database foundations
- Base API, CORS configuration, health/status endpoints, and authentication endpoint placeholder

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
