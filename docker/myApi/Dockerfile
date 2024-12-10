#syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0

EXPOSE 5001

COPY *.csproj ./usr/src/app/

RUN dotnet tool install --global dotnet-ef
# RUN dotnet add package RestSharp

ENV DOTNET_USE_POLLING_FILE_WATCHER 1
ENV PATH $PATH:/root/.dotnet/tools
ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT 1

WORKDIR /usr/src/app
ENTRYPOINT ["/usr/src/app/docker-entrypoint.sh"]
