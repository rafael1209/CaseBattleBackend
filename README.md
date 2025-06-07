# CaseBattleBackend

### Google Cloud Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com).

2. Navigate to **APIs & Services** → **Library** and enable **Google Drive API**.

3. Go to **APIs & Services** → **Credentials**.

4. Click **Create Credentials** → **Service Account**.

5. Fill in the service account details and create it.

6. After creation, open the service account and go to **Keys** → **Add Key** → **Create new key** → select **JSON** → **Create**.

7. Save the downloaded file as `google-credentials.json`.

---

### Example `google-credentials.json`

```json
{
  "type": "service_account",
  "project_id": "your-project-id",
  "private_key_id": "your-private-key-id",
  "private_key": "-----BEGIN PRIVATE KEY-----\nYOUR_PRIVATE_KEY\n-----END PRIVATE KEY-----\n",
  "client_email": "your-service-account-email@your-project-id.iam.gserviceaccount.com",
  "client_id": "your-client-id",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/your-service-account-email%40your-project-id.iam.gserviceaccount.com"
}
```

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
    "Credentials": "google-credentials.json",
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
      Google:Credentials: "/app/google-credentials.json"
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
