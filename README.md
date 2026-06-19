# Angular + ASP.NET Core — JWT Authentication & Authorization

A full-stack authentication system with **Angular 21** frontend and **ASP.NET Core 10** backend, demonstrating JWT-based auth with role/claim-based route guarding, conditional UI rendering, and HTTP interceptors.

## Tech Stack

### Backend (`AuthECAPI`)
- **ASP.NET Core 10** — Minimal APIs + Controllers
- **Clean Architecture** — Core / Application / Infrastructure / Extensions / Controllers
- **ASP.NET Core Identity** — User management & authentication
- **Entity Framework Core** — SQL Server (InMemory for tests)
- **JWT Bearer Authentication** — Token-based auth
- **OpenAPI / Scalar** — API documentation

### Frontend (`AuthECClient`)
- **Angular 21** — Standalone components, `bootstrapApplication`
- **Bootstrap 5.3** — Layout & styling
- **ngx-toastr** — Toast notifications
- **Reactive Forms** — Login & registration with custom validators
- **HttpClient** with functional interceptors
- **View Transitions** — CSS route transitions

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) >= 18
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- SQL Server (or use InMemory for testing)

## Getting Started

### 1. Backend Setup

```bash
cd AuthECAPI

# Initialize user secrets (if not already done)
dotnet user-secrets init --project AuthECAPI/AuthECAPI.csproj

# Set connection string
dotnet user-secrets set "ConnectionStrings:DevConnection" "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;" --project AuthECAPI/AuthECAPI.csproj

# Set JWT secret (min 32 characters)
dotnet user-secrets set "AppSettings:JWTSeecret" "GiveASecretKeyHavingAtLeast32Characters" --project AuthECAPI/AuthECAPI.csproj

# Run the API
dotnet run --project AuthECAPI/AuthECAPI.csproj
```

The API starts at `http://localhost:5292`. Swagger UI is available at `/swagger`.

### 2. Frontend Setup

```bash
cd AuthECClient

# Install dependencies
npm install

# Start development server
ng serve
```

Navigate to `http://localhost:4200/`. The app auto-reloads on file changes.

### 3. Docker

Run the entire stack (API, frontend, and SQL Server) with a single command:

```bash
# Set required environment variables
$env:MSSQL_SA_PASSWORD="YourStrong!Passw0rd"
$env:JWT_SECRET="GiveASecretKeyHavingAtLeast32Characters"

# Start all services
docker compose up -d
```

- **Frontend** — `http://localhost:4200`
- **API** — `http://localhost:5292`
- **SQL Server** — `localhost:1433` (host)

The SQL Server data persists in a named volume (`sqlserver-data`). To stop:

```bash
docker compose down
```

To also remove the volume:

```bash
docker compose down -v
``` 

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/signup` | Create a new user | No |
| POST | `/api/signin` | Sign in, returns JWT token | No |
| GET | `/api/userprofile` | Get current user profile | Yes |
| GET | `/api/AdminOnly` | Admin-only resource | Yes (Admin) |
| GET | `/api/AdminOrTeacher` | Admin or Teacher resource | Yes (Admin/Teacher) |
| GET | `/api/LibraryMembersOnly` | Library members resource | Yes (Policy) |
| GET | `/api/ApplyForMaternityLeave` | Maternity leave endpoint | Yes (Female + Teacher) |
| GET | `/api/Under10sAndFemale` | Under 10 and female endpoint | Yes (Policy) |

## Project Structure

```
├── AuthECAPI/                          # ASP.NET Core Web API (Clean Architecture)
│   └── AuthECAPI/
│       ├── Core/                       # Domain layer — entities, enums, interfaces
│       │   ├── Entities/
│       │   │   └── AppUser.cs
│       │   ├── Enums/
│       │   │   └── Roles.cs
│       │   └── Interfaces/
│       │       ├── IAuthService.cs
│       │       └── ITokenService.cs
│       ├── Application/                # Business logic — DTOs, services
│       │   ├── Models/
│       │   │   ├── AppSettings.cs
│       │   │   ├── LoginModel.cs
│       │   │   └── UserRegistrationModel.cs
│       │   └── Services/
│       │       ├── AuthService.cs
│       │       └── TokenService.cs
│       ├── Infrastructure/             # Data access & external concerns
│       │   ├── Data/
│       │   │   ├── AppDbContext.cs
│       │   │   └── SeedData.cs
│       │   └── Extensions/
│       │       ├── AppConfigExtensions.cs
│       │       ├── EFCoreExtensions.cs
│       │       └── IdentityExtensions.cs
│       ├── Extensions/                 # Presentation-level extensions
│       │   └── OpenApiExtensions.cs
│       ├── Controllers/                # API endpoints (Minimal APIs)
│       │   ├── AccountEndpoints.cs
│       │   ├── AuthorizationDemoEndpoints.cs
│       │   ├── IdentityUserEndpoints.cs
│       │   └── OrderEndpoints.cs
│       └── Program.cs
│
└── AuthECClient/                       # Angular Frontend
    └── src/app/
        ├── core/
        │   ├── guards/                 # authGuard, isNotLoggedInGuard
        │   ├── interceptors/           # JWT interceptor
        │   └── services/               # auth.service, user.service
        ├── layouts/
        │   └── main-layout/            # Post-login sidebar layout
        └── shared/
            ├── components/
            │   ├── user/               # Login & Registration forms
            │   ├── dashboard/          # Welcome page
            │   ├── forbidden/          # 403 page
            │   └── authorizeDemo/      # Claim-based demo pages
            ├── constants/              # localStorage keys
            ├── directives/             # HideIfClaimsNotMetDirective
            ├── pipes/                  # FirstKey pipe
            └── utils/                  # Claim requirement functions
```

## Features

### Authentication
- User registration with full name, email, password (uppercase, lowercase, special char, min-length validation)
- Login with email/password
- JWT stored in `localStorage`
- Auth guard redirects unauthenticated users to `/signin`
- `isNotLoggedInGuard` redirects authenticated users away from login/register

### Authorization
- **Claim-based route guarding** — each route specifies a `claimReq` function evaluating JWT claims
- **Conditional UI** — `HideIfClaimsNotMetDirective` hides nav items/buttons based on user claims
- **HTTP interceptor** — handles 401 (clears token, redirects to login) and 403 (shows toast)

### Demo Routes

| Route | Required Claims |
|-------|-----------------|
| `/admin-only` | `role == "Admin"` |
| `/admin-or-teacher` | `role == "Admin" \|\| role == "Teacher"` |
| `/apply-for-maternity-leave` | `gender == "Female" && role == "Teacher"` |
| `/library-members-only` | Has a `libraryID` claim |
| `/under10-and-female` | `gender == "Female" && age <= 10` |

## Testing

```bash
# Run API tests
cd AuthECAPI
dotnet test

# Run frontend tests
cd AuthECClient
ng test
```

## License

This project is for educational purposes.
