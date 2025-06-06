# CaseBattleBackend

`appsettings.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "SecretKey": "#####################",
    "Issuer": "#####################"
  },
  "MongoDb": {
    "ConnectionString": "#####################",
    "DatabaseName": "#####################"
  },
  "SPWorlds": {
    "MiniAppToken": "#####################",
    "CardId": "#####################",
    "CardToken": "#####################",
    "RedirectUrl": "#####################", //"#MINIAPP",
    "WebhookUrl": "#####################"
  },
  "Minecraft": {
    "AvatarUrl": "#####################",
    "ItemUrl": "#####################"
  },
  "Google": {
    "FolderId": "#####################",
    "Credentials": "#####################",
    "BaseFileUrl": "#####################"
  }
}
```

`docker-compose.yml`
```yml
version: '3.8'

services:
  case-battle-backend:
    image: ghcr.io/rafael1209/casebattlebackend:master
    container_name: case-battle-backend
    expose:
      - "8080"    
    environment:
      Jwt:SecretKey: "#####################"
      Jwt:Issuer: "#####################"
      MongoDb:ConnectionString: "#####################"
      MongoDb:DatabaseName: "#####################"
      SPWorlds:MiniAppToken: "#####################"
      SPWorlds:CardId: "#####################"
      SPWorlds:CardToken: "#####################"
      SPWorlds:RedirectUrl: "#####################"
      SPWorlds:WebhookUrl: "#####################"
      Minecraft:AvatarUrl: "#####################"
      Minecraft:ItemUrl: "#####################"
      Google:FolderId: "#####################"
      Google:Credentials: "#####################"
      Google:BaseFileUrl: "#####################"
    volumes:
      - ./case-battle/google-credentials.json:/app/google-credentials.json:ro
    networks:
      - app-network
      - mongo-network
  
  mongodb:
    image: mongo:latest
    container_name: mongodb
    restart: always
    ports:
      - "27017:27017"
    volumes:
      - ./mongo-data:/data/db
    networks:
      - mongo-network

  nginx:
    image: nginx:alpine
    container_name: nginx
    restart: always
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx-conf:/etc/nginx/conf.d
    depends_on:
      - chasman-front
    networks:
      - app-network
      
networks:
  app-network:
    driver: bridge
  mongo-network:
    driver: bridge
```
