FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/PostPilot.Api/PostPilot.Api.csproj", "src/PostPilot.Api/"]
COPY ["src/PostPilot.Domain/PostPilot.Domain.csproj", "src/PostPilot.Domain/"]
COPY ["src/PostPilot.Infrastructure/PostPilot.Infrastructure.csproj", "src/PostPilot.Infrastructure/"]
RUN dotnet restore "src/PostPilot.Api/PostPilot.Api.csproj"

COPY . .
RUN dotnet publish "src/PostPilot.Api/PostPilot.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM build AS migrations
RUN dotnet tool install --global dotnet-ef --version 10.0.8
ENV PATH="/root/.dotnet/tools:${PATH}"
ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "src/PostPilot.Infrastructure/PostPilot.Infrastructure.csproj", "--startup-project", "src/PostPilot.Api/PostPilot.Api.csproj"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PostPilot.Api.dll"]
