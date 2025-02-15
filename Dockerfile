FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app .

# Create directory for certificates
RUN mkdir -p /app/cert

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "azspx-mgt-web.dll"]