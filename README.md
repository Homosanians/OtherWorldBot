# Other World Bot

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/37156bf972d3435baae36087497d1bb5)](https://www.codacy.com?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Homosanians/OtherWorldBot&amp;utm_campaign=Badge_Grade)

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

### Docker

```bash
curl -sSL https://get.docker.com | sudo sh
sudo usermod -aG docker $USER
docker run -d -v /opt/owbot_data:/data --restart unless-stopped --name owbot homosanians/otherworldbot:latest
```
