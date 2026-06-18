# PostPilot Backend

PostPilot is an admin-only ASP.NET Core Web API for managing product posts and stories for Facebook Page and Instagram.

## Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL / Supabase PostgreSQL
- JWT authentication

## Project Structure

```text
src/
  PostPilot.Api/
    Features/
    Shared/
  PostPilot.Domain/
    Common/
    Entities/
  PostPilot.Infrastructure/
    Auth/
    Database/
    EntityConfigurations/
    Storage/
    Startup/
tests/
  PostPilot.UnitTests/
  PostPilot.IntegrationTests/
```

## Local Setup

Set these environment variables or use user secrets:

```text
POSTPILOT_DATABASE_CONNECTION
POSTPILOT_JWT_SIGNING_KEY
POSTPILOT_JWT_ISSUER
POSTPILOT_JWT_AUDIENCE
POSTPILOT_JWT_EXPIRATION_MINUTES
```

Create user records directly in the database. The API does not seed users at startup.

Passwords must be stored as hashes using the same format as `Pbkdf2PasswordHasher`.

Run locally:

```powershell
dotnet run --project src\PostPilot.Api\PostPilot.Api.csproj
```

OpenAPI is available in development at `/openapi/v1.json`. Health checks are available at `/health`.

## Implemented Endpoints

- `POST /api/auth/login`
- `GET /api/profiles`
- `POST /api/profiles`
- `GET /api/profiles/{profileId}/categories`
- `POST /api/profiles/{profileId}/categories`
- `PUT /api/profiles/{profileId}/categories/{categoryId}`
- `DELETE /api/profiles/{profileId}/categories/{categoryId}`
- `POST /api/profiles/{profileId}/media`
- `GET /api/profiles/{profileId}/history`

Media upload currently uses local `wwwroot/uploads` storage for development and returns a public URL from the API host. Cloudinary or Supabase Storage can replace the local provider before production publishing.

History supports filtering by platform, status, category, date range, keyword, and existing paging query params.

Later dashboard and real Meta publish routes are still planned.

## Verification

```powershell
dotnet build
dotnet test
```