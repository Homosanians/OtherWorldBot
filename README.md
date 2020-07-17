# Disgrace Discord Bot

Made for Other World community Discord server.

## Build

```bash
dotnet publish -c Release -f netcoreapp3.1 -r <arch>
```

```bash
docker rm --force dbot
docker rmi homosanians/disgracediscordbot:latest
docker build -t homosanians/disgracediscordbot:latest .
docker run -d -v /c/data:/data --restart unless-stopped --name dbot homosanians/disgracediscordbot:latest
```

## Deploy

### Daemon

No daemon instructions

### Docker

```bash
curl -sSL https://get.docker.com | sudo sh
sudo usermod -aG docker $USER
docker build . homosanians:disgracediscordbot
docker run homosanians:disgracediscordbot --name dbot
```
