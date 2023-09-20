FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Send.csproj", "./"]
RUN dotnet restore "Send.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Send.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Send.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Send.dll"]
