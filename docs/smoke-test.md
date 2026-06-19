# PostPilot MVP Smoke Test

Use this checklist after every backend/frontend feature task before moving to Meta API work.

## 1. Backend verification

```bash
dotnet restore src/PostPilot.Api/PostPilot.Api.csproj
dotnet build src/PostPilot.Api/PostPilot.Api.csproj
dotnet test tests/PostPilot.UnitTests/PostPilot.UnitTests.csproj
dotnet ef database update --project src/PostPilot.Infrastructure/PostPilot.Infrastructure.csproj --startup-project src/PostPilot.Api/PostPilot.Api.csproj
dotnet run --project src/PostPilot.Api/PostPilot.Api.csproj
```

Expected:

- `/health` returns success.
- Swagger/OpenAPI loads in development.
- No secrets are printed in logs.
- `wwwroot/uploads` is used only for local development.

## 2. Frontend verification

```bash
npm install
npm run build
npm run dev
```

Expected:

- TypeScript build passes.
- Vite starts without console compile errors.
- `VITE_POSTPILOT_API_URL` points to the local API when needed.

## 3. Cloudinary verification

Set these variables when testing a public image URL for Meta preparation:

```bash
export POSTPILOT_STORAGE_PROVIDER=Cloudinary
export POSTPILOT_CLOUDINARY_CLOUD_NAME=your-cloud-name
export POSTPILOT_CLOUDINARY_API_KEY=your-api-key
export POSTPILOT_CLOUDINARY_API_SECRET=your-api-secret
export POSTPILOT_CLOUDINARY_FOLDER=postpilot
```

Expected:

- Uploading an image returns `storageProvider: Cloudinary`.
- The returned `publicUrl` is an HTTPS Cloudinary URL.
- The `publicUrl` opens from another browser or device without logging in.

## 4. End-to-end smoke flow

1. Login with the seed admin account.
2. Select an existing profile or create a new one.
3. Open Categories and create a category.
4. Open Create Post.
5. Select the category.
6. Upload a JPG, PNG, WebP, or GIF under 10 MB.
7. Save Draft.
8. Open Drafts and confirm the draft appears.
9. Add the draft to Queue.
10. Open Queue and confirm the queued post appears.
11. Move the queued item up/down when there is more than one item.
12. Click Post Next.
13. Open Post History and confirm a Posted item appears with a mock external ID.
14. Open Dashboard and confirm Draft, Queued, Posted, Failed, Queue Status, and Recent Posts reflect the new data.

## 5. Negative checks

- Uploading an unsupported file type should return a validation error.
- Uploading an image over 10 MB should return a validation error.
- A category, media item, post, queue item, or history item from another profile must not be accessible through the selected profile route.
- Calling protected endpoints without a bearer token should return unauthorized.
- Re-adding the same post to Queue should not create a duplicate pending queue item.

## 6. Known MVP limits

- Publish uses the mock provider only.
- Local uploads are not suitable for real Meta publishing because public URLs must be accessible from the internet.
- Engagement metrics stay at zero until Meta analytics is implemented.
- Queue is manual; no background scheduler is enabled yet.
