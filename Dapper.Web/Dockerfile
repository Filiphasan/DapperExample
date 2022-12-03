FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Dapper.Web/Dapper.Web.csproj", "Dapper.Web/"]
RUN dotnet restore "Dapper.Web/Dapper.Web.csproj"
COPY . .
WORKDIR "/src/Dapper.Web"
RUN dotnet build "Dapper.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dapper.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dapper.Web.dll"]
