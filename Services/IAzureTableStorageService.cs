using Azure;
using Azure.Data.Tables;

namespace azspx_mgt_web.Services;

public interface IAzureTableStorageService
{
    Task<List<string>> ListTablesAsync();
    Task<TableClient> GetTableClientAsync(string tableName);
    Task CreateTableAsync(string tableName);
    Task DeleteTableAsync(string tableName);
    
    Task<Page<TableEntity>> ListEntitiesAsync(string tableName, string? filter = null, int? pageSize = null, string? continuationToken = null);
    Task<TableEntity?> GetEntityAsync(string tableName, string partitionKey, string rowKey);
    Task UpsertEntityAsync(string tableName, TableEntity entity);
    Task DeleteEntityAsync(string tableName, string partitionKey, string rowKey);
    
    Task<IReadOnlyList<TableEntity>> QueryEntitiesAsync(string tableName, string? filter = null);
}