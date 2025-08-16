FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Luna.Users.gRPC/Luna.Users.gRPC.csproj", "Luna.Users.gRPC/"]
COPY ["Luna.Users.Models.Blank/Luna.Users.Models.Blank.csproj", "Luna.Users.Models.Blank/"]
COPY ["Luna.Users.Models.View/Luna.Users.Models.View.csproj", "Luna.Users.Models.View/"]
COPY ["Luna.Tools/Luna.Tools.csproj", "Luna.Tools/"]
COPY ["Luna.Users.Services/Luna.Users.Services.csproj", "Luna.Users.Services/"]
COPY ["Luna.Users.Models.Database/Luna.Users.Models.Database.csproj", "Luna.Users.Models.Database/"]
COPY ["Luna.Users.Models.Domain/Luna.Users.Models.Domain.csproj", "Luna.Users.Models.Domain/"]
COPY ["Luna.Users.Repositories/Luna.Users.Repositories.csproj", "Luna.Users.Repositories/"]
RUN dotnet restore "Luna.Users.gRPC/Luna.Users.gRPC.csproj"
COPY . .
WORKDIR "/src/Luna.Users.gRPC"
RUN dotnet build "./Luna.Users.gRPC.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Luna.Users.gRPC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Luna.Users.gRPC.dll"]
