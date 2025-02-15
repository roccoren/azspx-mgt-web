# Azure Speech Manager

A web application for managing Azure Speech Service transcription jobs, built with ASP.NET Core and Vue.js.

## Features

- View and manage Azure Speech Service transcription jobs
- Real-time status updates
- Download transcription files and reports
- Secure authentication
- Responsive UI

## Prerequisites

- .NET 9.0 SDK
- Node.js and npm
- Azure Speech Service subscription

## Configuration

1. Update `appsettings.json` with your Azure Speech Service settings:
```json
{
  "AzureSpeech": {
    "SubscriptionKey": "your-subscription-key",
    "Region": "your-region"
  }
}
```

2. Set up JWT authentication by updating the JWT settings in `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-secure-key-at-least-32-characters",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

## Development Setup

1. Clone the repository
2. Install .NET dependencies:
```bash
dotnet restore
```

3. Install Vue.js dependencies:
```bash
cd ClientApp
npm install
```

4. Start the development servers:

In one terminal:
```bash
dotnet run
```

In another terminal:
```bash
cd ClientApp
npm run dev
```

The application will be available at:
- API: http://localhost:5000
- Frontend: http://localhost:5173

## Default Login

- Username: admin
- Password: admin123

**Note:** Change these credentials in production.

## Production Deployment

1. Build the Vue.js frontend:
```bash
cd ClientApp
npm run build
```

2. Publish the ASP.NET Core application:
```bash
dotnet publish -c Release
```

## Security Notes

1. Replace the development JWT key with a secure key in production
2. Implement proper user authentication and password hashing
3. Configure proper CORS settings for your production environment
4. Use HTTPS in production
5. Update the default admin credentials

## License

MIT