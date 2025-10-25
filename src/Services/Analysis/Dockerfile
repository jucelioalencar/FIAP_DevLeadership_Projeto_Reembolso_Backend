FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CorisSeguros.Analysis.csproj", "."]
RUN dotnet restore "CorisSeguros.Analysis.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "CorisSeguros.Analysis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CorisSeguros.Analysis.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CorisSeguros.Analysis.dll"]



