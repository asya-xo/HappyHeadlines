# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the csproj file first
COPY ./ArticleService.csproj ./
RUN dotnet restore ArticleService.csproj

# Copy the rest of the files
COPY . .

# Build the ArticleService project only
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ArticleService.dll"]
EXPOSE 8080
