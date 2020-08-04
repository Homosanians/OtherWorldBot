<p align="center"><b>Other World Bot</b></p>

<p align="center"><img src="https://cdn.discordapp.com/avatars/733404944635527208/be9c8683a0171bd111808b0159e3c360.png?size=128"></img></p>

<p align="center"><img src="https://app.codacy.com/project/badge/Grade/37156bf972d3435baae36087497d1bb5"></img></p>

<p align="center">Made for Other World community Discord server.</p>

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
