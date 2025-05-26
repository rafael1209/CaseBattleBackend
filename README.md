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
  }
}
```

`docker-compose.yml`
```yml
//soon
```
