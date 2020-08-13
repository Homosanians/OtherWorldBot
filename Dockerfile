ARG GIT_COMMIT=unspecified

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

VOLUME ["/data"]

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release /p:AssemblyVersion=$GIT_COMMIT -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

COPY docker_startup.sh /bin/docker_startup.sh

ENTRYPOINT ["/bin/sh", "/bin/docker_startup.sh"]