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

## Docker Development (Windows 11)

Docker Compose is the primary local development setup. It starts PostgreSQL in the `postpilot-postgres` container and the API in the `postpilot-api` container. PostgreSQL data is stored in the named `postpilot_postgres_data` volume, so it remains after container restarts and `docker compose down`.

1. Create a local environment file and replace the two placeholder secrets:

```powershell
Copy-Item .env.example .env
notepad .env
```

Set a local database password in `POSTPILOT_DB_PASSWORD` and a unique 32-character-or-longer value in `POSTPILOT_JWT_SIGNING_KEY`. Do not commit `.env`.

2. Create the database container, run EF Core migrations, and start the API:

```powershell
docker compose up -d postpilot-db
docker compose --profile tools run --rm postpilot-migrator
docker compose up -d --build postpilot-api
```

The API is available at `http://localhost:5270`, Swagger UI at `http://localhost:5270/swagger`, Scalar at `http://localhost:5270/scalar/v1`, and health checks at `http://localhost:5270/health`. PostgreSQL is available from Windows at `localhost:5432` by default.

Run every migration after adding a new EF Core migration:

```powershell
docker compose --profile tools run --rm postpilot-migrator
```

Useful Docker commands:

```powershell
# Stop containers while retaining the PostgreSQL volume.
docker compose down

# Restart running services.
docker compose restart

# Follow logs for the API or database.
docker compose logs -f postpilot-api
docker compose logs -f postpilot-db

# Open a psql shell using the credentials configured in the database container.
docker compose exec postpilot-db sh -lc 'psql -U "$POSTGRES_USER" -d "$POSTGRES_DB"'

# Rebuild after Dockerfile or dependency changes, then recreate the API.
docker compose build --no-cache postpilot-api
docker compose up -d --force-recreate postpilot-api
```

To intentionally remove all local Docker database data, run `docker compose down -v`.

## Host Local Setup

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

For local test accounts, store the credentials in user secrets and run the opt-in development seeder:

```powershell
dotnet user-secrets set "POSTPILOT_TEST_USER_EMAIL" "user@example.com" --project src\PostPilot.Api\PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_TEST_USER_PASSWORD" "replace-with-a-test-password" --project src\PostPilot.Api\PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_TEST_ADMIN_EMAIL" "admin@example.com" --project src\PostPilot.Api\PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_TEST_ADMIN_PASSWORD" "replace-with-a-test-password" --project src\PostPilot.Api\PostPilot.Api.csproj
dotnet run --project src\PostPilot.Api\PostPilot.Api.csproj -- --seed-test-users
```

The command applies pending EF Core migrations and then creates or updates the two test accounts. It is blocked outside the Development environment.

Run locally:

```powershell
dotnet run
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

## Platform OAuth setup

Copy the OAuth variables from `.env.example` into `.env`, then create Developer Apps for Meta, X, eBay, Etsy, Lazada, Shopee, and TikTok Shop. Register these callback URLs:

- Meta: `http://localhost:5270/api/oauth/facebook/callback`
- X: `http://localhost:5270/api/oauth/x/callback`
- eBay: configure the callback as the RuName Accept URL, then put the RuName in `POSTPILOT_EBAY_REDIRECT_URI`
- Etsy: `{POSTPILOT_OAUTH_CALLBACK_BASE_URL}/api/oauth/etsy/callback` (Etsy requires HTTPS, so local development needs an HTTPS tunnel)
- Lazada: `{POSTPILOT_OAUTH_CALLBACK_BASE_URL}/api/oauth/lazada/callback`
- Shopee: `{POSTPILOT_OAUTH_CALLBACK_BASE_URL}/api/oauth/shopee/callback`
- TikTok Shop: `{POSTPILOT_OAUTH_CALLBACK_BASE_URL}/api/oauth/tiktokshop/callback`

Run `docker compose --profile tools run --rm postpilot-migrator` after pulling OAuth schema changes. Access and refresh tokens are encrypted by ASP.NET Core Data Protection; its keys persist in the `postpilot_data_protection_keys` Docker volume. All eight platform cards support OAuth; Facebook and Instagram use the same Meta authorization flow.

When running with `dotnet run`, keep provider credentials in .NET User Secrets:

```powershell
dotnet user-secrets set "POSTPILOT_META_CLIENT_ID" "your-meta-app-id" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_META_CLIENT_SECRET" "your-meta-app-secret" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_X_CLIENT_ID" "your-x-client-id" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_X_CLIENT_SECRET" "your-x-client-secret" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_EBAY_CLIENT_ID" "your-ebay-client-id" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_EBAY_CLIENT_SECRET" "your-ebay-client-secret" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_EBAY_REDIRECT_URI" "your-ebay-runame" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_ETSY_CLIENT_ID" "your-etsy-keystring" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_ETSY_SHARED_SECRET" "your-etsy-shared-secret" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_LAZADA_APP_KEY" "your-lazada-app-key" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_LAZADA_APP_SECRET" "your-lazada-app-secret" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_SHOPEE_PARTNER_ID" "your-shopee-partner-id" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_SHOPEE_PARTNER_KEY" "your-shopee-partner-key" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_TIKTOK_SHOP_APP_KEY" "your-tiktok-shop-app-key" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_TIKTOK_SHOP_APP_SECRET" "your-tiktok-shop-app-secret" --project src/PostPilot.Api/PostPilot.Api.csproj
dotnet user-secrets set "POSTPILOT_TIKTOK_SHOP_AUTHORIZATION_URL" "the-seller-authorization-url-from-partner-center" --project src/PostPilot.Api/PostPilot.Api.csproj
```

Dashboard currently returns real counts for draft, queued, posted, failed, pending queue status, and recent posts. Engagement metrics stay at zero until a real Meta analytics integration is added.

## Verification

```powershell
dotnet build
dotnet test build/PostPilot.slnx
```
