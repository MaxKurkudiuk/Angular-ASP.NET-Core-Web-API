



## Configuration

This repository keeps connection strings empty in `AuthECAPI/appsettings.json` on purpose.
Set real values locally with **User Secrets**.

### 1) Initialize secrets (if needed)

```bash
dotnet user-secrets init --project AuthECAPI/AuthECAPI.csproj
```

### 2) Set connection strings

```bash
dotnet user-secrets set "ConnectionStrings:DevConnection" "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;" --project AuthECAPI/AuthECAPI.csproj

dotnet user-secrets set "AppSettings:JWTSeecret" "GiveASecretKeyHavingAtLeast32Characters" --project AuthECAPI/AuthECAPI.csproj
```

You could use https://jwtsecretkeygenerator.com/ for generating a secret key

> `UserSecretsId` in the project file is safe to commit. Secret values are stored outside the repository.

### 3) Get connection strings

```bash
dotnet user-secrets list --project AuthECAPI/AuthECAPI.csproj
```

or just inside the project:

```bash
dotnet user-secrets list
```