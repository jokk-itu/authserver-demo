FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Clients/BasicClient/Server", "Clients/BasicClient/Server/"]
COPY ["Clients/BasicClient/Client", "Clients/BasicClient/Client/"]
COPY ["Clients/BasicClient/Shared", "Clients/BasicClient/Shared/"]
RUN dotnet restore "Clients/BasicClient/Server/Server.csproj"
WORKDIR "/src/Clients/BasicClient/Server"
RUN dotnet build "Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]