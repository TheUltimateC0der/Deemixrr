# Deemixrr

Deemixrr manages your artists and playlists completely automated. You add your favorite artists and playlists, and Deemixrr does the rest for you.

[![Discord: https://discord.gg/qJQPqR8](https://img.shields.io/discord/751788862644158504?color=blue&label=Discord&style=for-the-badge)](https://discord.gg/qJQPqR8)
[![GitHub: Last commit date (develop)](https://img.shields.io/github/last-commit/TheUltimateC0der/Deemixrr/develop.svg?style=for-the-badge&colorB=177DC1)](https://github.com/TheUltimateC0der/Deemixrr/commits/develop)
<br/>
[![Docker Automated build: theultimatecoder/deemixrr](https://img.shields.io/docker/cloud/automated/theultimatecoder/deemixrr?color=blue&style=for-the-badge)](https://hub.docker.com/r/theultimatecoder/deemixrr)
[![Docker Build Status: theultimatecoder/deemixrr](https://img.shields.io/docker/cloud/build/theultimatecoder/deemixrr?color=blue&style=for-the-badge)](https://hub.docker.com/r/theultimatecoder/deemixrr)
[![Docker Pulls: theultimatecoder/deemixrr](https://img.shields.io/docker/pulls/theultimatecoder/deemixrr?color=blue&style=for-the-badge)](https://hub.docker.com/r/theultimatecoder/deemixrr)

## What Deemixrr does for you

- Manage your Artists
- Manage your Playlists
- Automatically updates your playlists
- Automatically updates your artists
- Calculates the size of your media
- Add artists by Deezer Id
- Add artists by scraping them from a playlist
- Add artists by adding multiple Deezer Ids seperated by a ,
- Select an individual storing location for each artist
- Add playlists by Deezer Id
- Add playlists by adding multiple Deezer Ids seperated by a ,
- Select an individual storing location for each playlist

## How to spin-up your own instance

#### Docker-Compose

If you already experienced with deemix, you can use this as a starting point. For more advanced setup, please have a look here: https://github.com/TheUltimateC0der/Deemixrr/wiki/Host-your-own

```yaml
version: '3'
services:
    deemixrr:
        image: theultimatecoder/deemixrr:latest
        environment:
            # Connectionstring for the database
            - ConnectionStrings__DefaultConnection=server=mssql;uid=sa;pwd=H^yi4HtSY$rgd@ptd9PD6YN#dJni6HsNnG^kouXB62zcd4jQKAyw3hp3HcCA7Zp2qco6R&!oC%YzCV#!B5r@tWZerb6KB3NywiCzbeVy#Z6m#q6$Dq4WgFb2!o%vLV^T;database=Deemixrr;pooling=true
            # Hangfire dashboard
            - Hangfire__DashboardPath=/autoloaderjobs
            - Hangfire__Password=p2S3cVY6Yojkby9PYG3AbGPqVzbo8KLS
            - Hangfire__Username=Deemixrr
            - Hangfire__Workers=2
            # Configure the cron expression for your job
            - JobConfiguration__GetUpdatesRecurringJob=15 * * * *
            - JobConfiguration__SizeCalculatorRecurringJob=* 6 * * *
            - Kestrel__EndPoints__Http__Url=http://0.0.0.0:5555
            # Use the id command in your shell to determine the ids
            - PGID=1234
            - PUID=1234
        ports:
            #remove this if you use something like nginx reverse-proxy
            - 5555:5555
        depends_on:
            - mssql
        volumes:
            # Mount the deemix config files
            - /opt/deemixrr/deemix:/config/.config/deemix
            # Mount your media folder
            - /mnt/unionfs:/mnt/unionfs
    mssql:
        image: microsoft/mssql-server-linux:latest
        environment:
            - SA_PASSWORD=H^yi4HtSY$rgd@ptd9PD6YN#dJni6HsNnG^kouXB62zcd4jQKAyw3hp3HcCA7Zp2qco6R&!oC%YzCV#!B5r@tWZerb6KB3NywiCzbeVy#Z6m#q6$Dq4WgFb2!o%vLV^T
            - ACCEPT_EULA=Y
        volumes:
            # Persist the db files
            - /opt/deemixrr/mssql:/var/opt/mssql
```




## Screenshots

#### Dashboard
![Dashboard](/screenshots/dashboard.png "Dashboard")

#### Create folder
![Create folder](/screenshots/folder-create.png "Create folder")

#### Create artist
![Create artist](/screenshots/artist-create.png "Create artist")

#### Create playlist
![Create playlist](/screenshots/playlist-create.png "Create playlist")
