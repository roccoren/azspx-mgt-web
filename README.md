# Azure Speech Manager

A web application for managing Azure Speech Service transcription jobs, built with ASP.NET Core and Vue.js.

## Features

- View and manage Azure Speech Service transcription jobs
- Azure Table Storage integration for job data persistence
- Real-time status updates
- Download transcription files and reports
- Secure authentication with JWT
- Support for Azure Entra ID (Azure AD) authentication
- Responsive UI built with Vue.js and Tailwind CSS

## Prerequisites

- .NET 9.0 SDK
- Node.js and npm
- Azure Speech Service subscription
- Azure Storage Account
- (Optional) Azure Entra ID application registration

## Configuration

You can configure the application using either environment variables or appsettings.json. We recommend using environment variables for production deployments.

### Environment Variables Setup

1. Copy `.env.example` to `.env`:
```bash
cp .env.example .env
```

2. Configure the following sections in your `.env` file:

#### JWT Authentication
```
JWT__Key=your-secure-jwt-key-for-production-min-32-chars
JWT__Issuer=https://localhost:5000
JWT__Audience=https://localhost:5000
JWT__ExpiryInDays=7
```

#### Azure Speech Service
```
AzureSpeech__SubscriptionKey=your-speech-service-subscription-key
AzureSpeech__Region=your-speech-service-region
```

#### Azure Table Storage (Choose one option)

Option 1 - Using Azure Entra ID (Recommended for production):
```
AZURE_TENANT_ID=your-tenant-id
AZURE_CLIENT_ID=your-client-id
AZURE_CLIENT_SECRET=your-client-secret
AzureTableStorage__AccountUrl=https://your-account.table.core.windows.net
```

Option 2 - Using Azure CLI (Development):
```
AzureTableStorage__AccountName=your-storage-account-name
# Make sure you're logged in with: az login
```

#### CORS Configuration
```
Cors__Origins__0=http://localhost:5173
```

### Application Settings

Alternatively, update `appsettings.json` with your settings:

```json
{
  "JWT": {
    "Key": "your-secure-key-at-least-32-characters",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpiryInDays": 7
  },
  "AzureSpeech": {
    "SubscriptionKey": "your-subscription-key",
    "Region": "your-region"
  },
  "AzureTableStorage": {
    "AccountUrl": "https://your-account.table.core.windows.net",
    "AccountName": "your-storage-account-name"
  },
  "Cors": {
    "Origins": ["http://localhost:5173"]
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

## Docker Support

The application includes Docker support. To run using Docker Compose:

```bash
docker-compose up --build
```

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
6. Use Azure Entra ID authentication in production instead of Azure CLI login
7. Store sensitive configuration in Azure Key Vault for production deployments

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request

## License

MIT