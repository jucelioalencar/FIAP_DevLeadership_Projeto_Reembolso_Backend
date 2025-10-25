FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7005

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CorisSeguros.Notification.csproj", "."]
RUN dotnet restore "CorisSeguros.Notification.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "CorisSeguros.Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CorisSeguros.Notification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CorisSeguros.Notification.dll"]



