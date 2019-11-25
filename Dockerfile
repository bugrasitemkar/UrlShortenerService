FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 48719
EXPOSE 27017
EXPOSE 5000
EXPOSE 5001


FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["ShortUrl.API/ShortUrl.API.csproj", "ShortUrl.API/"]
RUN dotnet restore "ShortUrl.API/ShortUrl.API.csproj"
COPY . .
WORKDIR "/src/ShortUrl.API"
RUN dotnet build "ShortUrl.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShortUrl.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShortUrl.API.dll"]
