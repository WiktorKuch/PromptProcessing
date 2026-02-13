FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PromptApi.csproj", "."]
RUN dotnet restore "PromptApi.csproj"
COPY . .
RUN dotnet build "PromptApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PromptApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PromptApi.dll"]
