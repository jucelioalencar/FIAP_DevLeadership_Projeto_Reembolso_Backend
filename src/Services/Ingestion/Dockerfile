FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CorisSeguros.Ingestion.csproj", "."]
RUN dotnet restore "CorisSeguros.Ingestion.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "CorisSeguros.Ingestion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CorisSeguros.Ingestion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CorisSeguros.Ingestion.dll"]



