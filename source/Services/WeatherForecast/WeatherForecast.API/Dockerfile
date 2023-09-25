#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/WeatherForecast/WeatherForecast.API/WeatherForecast.API.csproj", "Services/WeatherForecast/WeatherForecast.API/"]
COPY ["Services/WeatherForecast/WeatherForecast.Infrastucture/WeatherForecast.Infrastucture.csproj", "Services/WeatherForecast/WeatherForecast.Infrastucture/"]
COPY ["Services/WeatherForecast/WeatherForecast.Domain/WeatherForecast.Domain.csproj", "Services/WeatherForecast/WeatherForecast.Domain/"]
COPY ["Services/WeatherForecast/WeatherForecast.Application/WeatherForecast.Application.csproj", "Services/WeatherForecast/WeatherForecast.Application/"]
RUN dotnet restore "Services/WeatherForecast/WeatherForecast.API/WeatherForecast.API.csproj"
COPY . .
WORKDIR "/src/Services/WeatherForecast/WeatherForecast.API"
RUN dotnet build "WeatherForecast.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "WeatherForecast.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherForecast.API.dll"]
