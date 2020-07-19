# Other World Bot

Made for Other World community Discord server.

## Build

```bash
dotnet publish -c Release -f netcoreapp3.1 -r <arch>
```

```bash
docker rm --force owbot
docker rmi homosanians/otherworldbot:latest
docker build -t homosanians/otherworldbot:latest .
docker run -d -v /opt/owbot_data:/data --restart unless-stopped --name owbot homosanians/otherworldbot:latest
```

## Deploy

### Daemon

No daemon instructions

### Docker

```bash
curl -sSL https://get.docker.com | sudo sh
sudo usermod -aG docker $USER
docker run -d -v /opt/owbot_data:/data --restart unless-stopped --name owbot homosanians/otherworldbot:latest
```
