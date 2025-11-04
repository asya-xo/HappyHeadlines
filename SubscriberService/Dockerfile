# ----- runtime image -----
# ----- runtime image -----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ----- build image -----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SubscriberService.csproj", "./"]
RUN dotnet restore "SubscriberService.csproj"
COPY . .
RUN dotnet publish "SubscriberService.csproj" -c Release -o /out

# ----- final -----
FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "SubscriberService.dll"]
