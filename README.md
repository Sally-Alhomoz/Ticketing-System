# Ticketing System

API-first ticketing/support system with an ASP.NET Core backend and a Vue 3 + Vite frontend.

Projects
- `TicketingSystem.WebAPI` — ASP.NET Core Web API (JWT auth, Swagger, EF Core)
- `TicketingSystem.DataAccess` — EF Core `DbContext` and migrations
- `TicketingSystem.Services` — business logic and services
- `Ticketingsystem.vue` — Vue 3 + Vite frontend

Tech
- .NET 8, C# 12
- EF Core (SQL Server)
- JWT Bearer authentication
- Swagger / Swashbuckle
- Vue 3 + Vite

Status
- Development-ready. Swagger available in Development environment. CORS configured for local Vue dev (`AllowVueDev` policy).

Getting Started

Prerequisites
- .NET 8 SDK
- Visual Studio 2022 (or VS Code + C# extension)
- SQL Server (local or remote)
- Node.js & npm
- (Optional) EF CLI: `dotnet tool install --global dotnet-ef`

1) Backend (API)
- Configure app settings (see Configuration).
- Restore, build and run: dotnet restore dotnet build dotnet run --project TicketingSystem.WebAPI

- For iterative development: dotnet watch run --project TicketingSystem.WebAPI



- In development, Swagger UI is available at:
  - `https://localhost:<port>/swagger`

2) Frontend (Vue)
- Install and run: cd TicketingSytem.vue npm install npm run dev 

- The Web API CORS policy `AllowVueDev` permits `http://localhost:54045`. If your dev server runs on a different port, update the CORS origins in `Program.cs`.

Configuration

Create or update `TicketingSystem.WebAPI/appsettings.json` (or use __Manage User Secrets__ / environment variables):
{  
"ConnectionStrings": 
{ "connectionString": "Server=localhost;Database=TicketingSystemDb;Trusted_Connection=True;" },
"JWT": {
"SecretKey": "YOUR_STRONG_SECRET_KEY", 
"Issuer": "YourIssuer",
"Audience": "YourAudience" } 
}


Notes:
- Program.cs reads `ConnectionStrings:connectionString`.
- Keep `JWT:SecretKey` secret — use __Manage User Secrets__ or environment variables for development.

Database Setup (EF Core migrations)

Migrations live in `TicketingSystem.DataAccess`. From the repository root:

- Add a migration: dotnet ef migrations add InitialCreate --project TicketingSystem.DataAccess
                                                          --startup-project TicketingSystem.WebAPI

- Apply migrations: dotnet ef database update --project TicketingSystem.DataAccess 
                                              --startup-project TicketingSystem.WebAPI

Visual Studio (Package Manager Console)
- Set `TicketingSystem.WebAPI` as startup project (__Set as Startup Project__).
- Open __Package Manager Console__ and run: Update-Database -Project TicketingSystem.DataAccess
                                                            -StartupProject TicketingSystem.WebAPI


Enable XML comments for Swagger
- In Visual Studio: Project Properties ? __Build > XML documentation file__ (enable).
- Or add to `TicketingSystem.WebAPI.csproj`: 
xml <PropertyGroup> <GenerateDocumentationFile>true</GenerateDocumentationFile> </PropertyGroup>


API Documentation (Swagger)

- Swagger is configured in `TicketingSystem.WebAPI` via Swashbuckle.
- Visit: `https://localhost:<port>/swagger` (development).
- Swagger is configured with a Bearer JWT security scheme. Click the Authorize button and provide: Bearer <your_jwt_token>


to call protected endpoints.

CORS
- Policy name: `AllowVueDev`
- Default allowed origin: `http://localhost:54045`
- Update the origin or policy in `Program.cs` if your frontend uses a different host/port.

Common Commands

- Restore & build: dotnet restore dotnet build

- Run API: dotnet run --project TicketingSystem.WebAPI

- Run API with hot reload: dotnet watch run --project TicketingSystem.WebAPI

- Frontend dev server: cd Ticketingsystem.vue npm install npm run dev


Troubleshooting

- 401 Unauthorized:
  - Confirm JWT token issuer/audience/signing key match `appsettings.json`.
  - Use Swagger Authorize to supply `Bearer <token>`.

- Database connection errors:
  - Verify `ConnectionStrings:connectionString` and SQL Server accessibility.
  - Ensure migrations have been applied.

- Migrations issues:
  - Ensure you use the correct `--project` and `--startup-project` when running `dotnet ef`.

Development Tips

- Keep `TicketingSystem.WebAPI` as the startup project (__Set as Startup Project__) for debugging in Visual Studio.
- Use `dotnet ef` with explicit project arguments when working in multi-project solutions.
- Confirm XML comment file name matches the assembly name if Swagger XML comments do not appear (Program.cs checks the XML file at runtime).

Contributing
- Fork/branch from `master`.
- Add tests for new functionality.
- Follow existing DI and service registration patterns in `Program.cs`.

License
- Add a LICENSE file (e.g., MIT) if you intend to open-source the repository.

Contact / Notes
- Repository remote: `https://github.com/Sally-Alhomoz/Ticketing-System`
- Default CORS origin for local Vue dev: `http://localhost:54045` (policy `AllowVueDev`)

Enjoy building and maintaining the system.



