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

Optional Cloudinary media storage configuration:

```text
POSTPILOT_STORAGE_PROVIDER=Cloudinary
POSTPILOT_CLOUDINARY_CLOUD_NAME
POSTPILOT_CLOUDINARY_API_KEY
POSTPILOT_CLOUDINARY_API_SECRET
POSTPILOT_CLOUDINARY_FOLDER=postpilot
```

If `POSTPILOT_STORAGE_PROVIDER` is not `Cloudinary`, or the Cloudinary credentials are missing, media upload falls back to local `wwwroot/uploads` storage for development.

Optional publish provider configuration:

```text
POSTPILOT_PUBLISH_PROVIDER=Meta
POSTPILOT_META_GRAPH_API_VERSION=v20.0
```

If `POSTPILOT_PUBLISH_PROVIDER` is not `Meta`, PostPilot uses the mock publisher. Meta publisher currently supports Facebook Page image publishing only.

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
- `GET /api/profiles/{profileId}/dashboard`
- `GET /api/profiles/{profileId}/categories`
- `POST /api/profiles/{profileId}/categories`
- `PUT /api/profiles/{profileId}/categories/{categoryId}`
- `DELETE /api/profiles/{profileId}/categories/{categoryId}`
- `POST /api/profiles/{profileId}/media`
- `GET /api/profiles/{profileId}/posts`
- `POST /api/profiles/{profileId}/posts`
- `POST /api/profiles/{profileId}/posts/{postId}/publish-now`
- `POST /api/profiles/{profileId}/posts/{postId}/queue`
- `GET /api/profiles/{profileId}/queue`
- `PUT /api/profiles/{profileId}/queue`
- `POST /api/profiles/{profileId}/queue/post-next`
- `GET /api/profiles/{profileId}/history`
- `GET /api/profiles/{profileId}/meta-connection`
- `PUT /api/profiles/{profileId}/meta-connection`

Media upload supports local development storage and Cloudinary. Use Cloudinary before real Meta publishing because Meta must fetch a public image URL from the internet.

Meta connection stores Facebook Page, optional Instagram Business identifiers, and a protected server-side credential. The protected value is never returned by the API.

Publishing uses mock mode by default. Set `POSTPILOT_PUBLISH_PROVIDER=Meta` to publish Facebook Page image posts through the Meta Graph API using the saved Meta connection.

Dashboard currently returns real counts for draft, queued, posted, failed, pending queue status, and recent posts. Engagement metrics stay at zero until a real Meta analytics integration is added.

## Verification

```powershell
dotnet build
dotnet test
```