FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["EcoSphere.csproj", "."]
RUN dotnet restore "EcoSphere.csproj"

COPY . .
RUN dotnet publish "EcoSphere.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# --- ICU kütüphanesi ekle (culture desteği için) ---
RUN apk add --no-cache icu-libs

# --- .NET'e kültür desteğini aktif et ---
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .
EXPOSE 4000
ENTRYPOINT ["dotnet", "EcoSphere.dll"]
