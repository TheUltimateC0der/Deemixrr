FROM lsiobase/ubuntu:bionic AS base

WORKDIR /app

EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Deemixrr/", "Deemixrr/"]
RUN dotnet restore "Deemixrr/Deemixrr.csproj"
COPY . .
WORKDIR "/src/Deemixrr"
RUN dotnet build "Deemixrr.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Deemixrr.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .

RUN curl https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb --output packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb

RUN apt-get update && \
    apt-get install -y python3 python3-pip apt-transport-https aspnetcore-runtime-3.1

COPY /etc /etc

ENTRYPOINT ["/init"]