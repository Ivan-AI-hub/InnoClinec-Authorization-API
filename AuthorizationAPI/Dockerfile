#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AuthorizationAPI.Web/AuthorizationAPI.Web.csproj", "AuthorizationAPI.Web/"]
COPY ["AuthorizationAPI.Application.Abstractions/AuthorizationAPI.Application.Abstractions.csproj", "AuthorizationAPI.Application.Abstractions/"]
COPY ["AuthorizationAPI.Application/AuthorizationAPI.Application.csproj", "AuthorizationAPI.Application/"]
COPY ["AuthorizationAPI.Domain/AuthorizationAPI.Domain.csproj", "AuthorizationAPI.Domain/"]
COPY ["AuthorizationAPI.Persistence/AuthorizationAPI.Persistence.csproj", "AuthorizationAPI.Persistence/"]
COPY ["AuthorizationAPI.Presentation/AuthorizationAPI.Presentation.csproj", "AuthorizationAPI.Presentation/"]
RUN dotnet restore "AuthorizationAPI.Web/AuthorizationAPI.Web.csproj"
COPY . .
WORKDIR "/src/AuthorizationAPI.Web"
RUN dotnet build "AuthorizationAPI.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthorizationAPI.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthorizationAPI.Web.dll"]