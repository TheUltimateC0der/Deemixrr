FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Deemix.AutoLoader/Deemix.AutoLoader.csproj", "Deemix.AutoLoader/"]
RUN dotnet restore "Deemix.AutoLoader/Deemix.AutoLoader.csproj"
COPY . .
WORKDIR "/src/Deemix.AutoLoader"
RUN dotnet build "Deemix.AutoLoader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Deemix.AutoLoader.csproj" -c Release -o /app/publish

FROM base AS final

# Install deemix
RUN apt-get update && apt-get install python3 python3-pip git -y
RUN python3 --version
RUN python3 -m pip install deemix

RUN \
    groupmod -g 1001 users && \
    useradd -u 1000 -U -d /config -s /bin/false abc && \
    usermod -G users abc && \
    mkdir /config && \
    chown abc:abc /config

USER abc

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Deemix.AutoLoader.dll"]