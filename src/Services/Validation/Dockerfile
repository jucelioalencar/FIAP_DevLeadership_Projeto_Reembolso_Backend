FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CorisSeguros.Validation.csproj", "."]
RUN dotnet restore "CorisSeguros.Validation.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "CorisSeguros.Validation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CorisSeguros.Validation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CorisSeguros.Validation.dll"]



