using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using System.Linq;

namespace azspx_mgt_web.Services;

public class AzureTableStorageService : IAzureTableStorageService
{
    private readonly TableServiceClient _tableServiceClient;
    
    public AzureTableStorageService(IConfiguration configuration)
    {
        var accountUrl = configuration["AzureTableStorage:AccountUrl"] 
            ?? $"https://{configuration["AzureTableStorage:AccountName"]}.table.core.windows.net/";

        if (string.IsNullOrEmpty(accountUrl))
        {
            throw new ArgumentException("Azure Table Storage Account URL or Account Name must be configured");
        }

        // DefaultAzureCredential tries the following methods in order:
        // 1. Environment variables (AZURE_CLIENT_ID, AZURE_TENANT_ID, AZURE_CLIENT_SECRET or AZURE_CLIENT_CERTIFICATE_PATH)
        // 2. Managed Identity
        // 3. Azure CLI credentials
        // 4. Azure PowerShell credentials
        // 5. Visual Studio credentials
        // 6. Visual Studio Code credentials
        var credential = new DefaultAzureCredential(
            new DefaultAzureCredentialOptions 
            { 
                ExcludeVisualStudioCredential = false,
                ExcludeVisualStudioCodeCredential = false,
                ExcludeAzureCliCredential = false,
                ExcludeEnvironmentCredential = false,
                ExcludeManagedIdentityCredential = false
            });

        _tableServiceClient = new TableServiceClient(
            new Uri(accountUrl),
            credential);
    }

    public async Task<List<string>> ListTablesAsync()
    {
        var tables = new List<string>();
        await foreach (var table in _tableServiceClient.QueryAsync())
        {
            tables.Add(table.Name);
        }
        return tables;
    }

    public async Task<TableClient> GetTableClientAsync(string tableName)
    {
        var tableClient = _tableServiceClient.GetTableClient(tableName);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }

    public async Task CreateTableAsync(string tableName)
    {
        await _tableServiceClient.CreateTableIfNotExistsAsync(tableName);
    }

    public async Task DeleteTableAsync(string tableName)
    {
        await _tableServiceClient.DeleteTableAsync(tableName);
    }

    public async Task<Page<TableEntity>> ListEntitiesAsync(
        string tableName, 
        string? filter = null, 
        int? pageSize = null, 
        string? continuationToken = null)
    {
        var tableClient = await GetTableClientAsync(tableName);
        
        AsyncPageable<TableEntity> queryResults = tableClient.QueryAsync<TableEntity>(
            filter: filter,
            maxPerPage: pageSize);

        var pages = queryResults.AsPages(continuationToken);
        await using var enumerator = pages.GetAsyncEnumerator();
        if (await enumerator.MoveNextAsync())
        {
            return enumerator.Current;
        }
        
        // If no results found, return the first page with empty results
        var emptyPage = await queryResults.AsPages().FirstAsync();
        return emptyPage;
    }

    public async Task<TableEntity?> GetEntityAsync(string tableName, string partitionKey, string rowKey)
    {
        var tableClient = await GetTableClientAsync(tableName);
        try
        {
            return await tableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task UpsertEntityAsync(string tableName, TableEntity entity)
    {
        var tableClient = await GetTableClientAsync(tableName);
        await tableClient.UpsertEntityAsync(entity);
    }

    public async Task DeleteEntityAsync(string tableName, string partitionKey, string rowKey)
    {
        var tableClient = await GetTableClientAsync(tableName);
        await tableClient.DeleteEntityAsync(partitionKey, rowKey);
    }

    public async Task<IReadOnlyList<TableEntity>> QueryEntitiesAsync(string tableName, string? filter = null)
    {
        var tableClient = await GetTableClientAsync(tableName);
        var entities = new List<TableEntity>();

        AsyncPageable<TableEntity> queryResults = tableClient.QueryAsync<TableEntity>(filter);
        await foreach (var entity in queryResults)
        {
            entities.Add(entity);
        }

        return entities;
    }
}