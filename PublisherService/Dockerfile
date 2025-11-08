# Use lightweight ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# Use lightweight SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["PublisherService/PublisherService.csproj", "PublisherService/"]
RUN dotnet restore "PublisherService/PublisherService.csproj"
COPY . .
WORKDIR "/src/PublisherService"
RUN dotnet build "PublisherService.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "PublisherService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PublisherService.dll"]
