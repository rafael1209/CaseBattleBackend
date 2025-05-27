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
    "SecretKey": "2aec48f69e8645ebbfdc4e5f96ed3a02",
    "Issuer": "mr.rafaello"
  },
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017/",
    "DatabaseName": "case-battle-db"
  },
  "SPWorlds": {
    "MiniAppToken": "APP_TOKEN",
    "CardId": "CARD_ID",
    "CardToken": "CARD_TOKEN",
    "RedirectUrl": "#MINIAPP",
    "WebhookUrl": "WEBHOOK_URL"
  },
  "Minecraft": {
    "AvatarUrl": "https://avatars.spworlds.ru/face/"
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
      Jwt:SecretKey: "JWT_TOKEN"
      Jwt:Issuer: "mr.rafaello"
      MongoDb:ConnectionString: "mongodb://mongodb:27017/"
      MongoDb:DatabaseName: "case-battle-db"
      SPWorlds:MiniAppToken: "APP_TOKEN"
      SPWorlds:CardId: "CARD_ID"
      SPWorlds:CardToken: "CARD_TOKEN"
      SPWorlds:RedirectUrl: "REDIRECT_URL"
      SPWorlds:WebhookUrl: "WEBHOOK_URL"
      Minecraft:AvatarUrl: "https://avatars.spworlds.ru/face/"
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
