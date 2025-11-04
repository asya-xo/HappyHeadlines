# ----- runtime image -----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ----- build image -----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SubscriberQueue.csproj", "./"]
RUN dotnet restore "SubscriberQueue.csproj"
COPY . .
RUN dotnet publish "SubscriberQueue.csproj" -c Release -o /out

# ----- final -----
FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "SubscriberQueue.dll"]
