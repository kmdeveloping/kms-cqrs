FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ExampleApi/ExampleApi.csproj", "ExampleApi/"]
RUN dotnet restore "ExampleApi/ExampleApi.csproj"
COPY . .
WORKDIR "/src/ExampleApi"
RUN dotnet build "ExampleApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExampleApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExampleApi.dll"]
