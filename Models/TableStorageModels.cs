using Azure;
using Azure.Data.Tables;

namespace azspx_mgt_web.Models;

public class TableListResponse
{
    public List<string> Tables { get; set; } = new();
}

public class TableEntityResponse
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

public class TableEntitiesResponse
{
    public List<TableEntityResponse> Entities { get; set; } = new();
    public string? ContinuationToken { get; set; }
}

public class UpsertTableEntityRequest
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

public static class TableEntityExtensions 
{
    public static TableEntityResponse ToResponse(this TableEntity entity)
    {
        var response = new TableEntityResponse
        {
            PartitionKey = entity.PartitionKey,
            RowKey = entity.RowKey,
            Properties = new Dictionary<string, object>()
        };

        foreach (var prop in entity)
        {
            if (prop.Key != nameof(TableEntity.PartitionKey) && 
                prop.Key != nameof(TableEntity.RowKey) &&
                prop.Key != nameof(TableEntity.Timestamp) &&
                prop.Key != nameof(TableEntity.ETag))
            {
                response.Properties[prop.Key] = prop.Value;
            }
        }

        return response;
    }

    public static TableEntity ToTableEntity(this UpsertTableEntityRequest request)
    {
        var entity = new TableEntity(request.PartitionKey, request.RowKey);
        foreach (var prop in request.Properties)
        {
            entity[prop.Key] = prop.Value;
        }
        return entity;
    }
}