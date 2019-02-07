FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY KrHomeBot/KrHomeBot.csproj KrHomeBot/
RUN dotnet restore KrHomeBot/KrHomeBot.csproj
COPY . .
WORKDIR /src/KrHomeBot
RUN dotnet build KrHomeBot.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish KrHomeBot.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KrHomeBot.dll"]
