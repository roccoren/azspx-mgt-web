using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using azspx_mgt_web.Services;
using azspx_mgt_web.Models;

namespace azspx_mgt_web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TableStorageController : ControllerBase
{
    private readonly IAzureTableStorageService _tableService;

    public TableStorageController(IAzureTableStorageService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet("tables")]
    public async Task<ActionResult<TableListResponse>> ListTables()
    {
        var tables = await _tableService.ListTablesAsync();
        return Ok(new TableListResponse { Tables = tables });
    }

    [HttpPost("tables/{tableName}")]
    public async Task<IActionResult> CreateTable(string tableName)
    {
        await _tableService.CreateTableAsync(tableName);
        return Ok();
    }

    [HttpDelete("tables/{tableName}")]
    public async Task<IActionResult> DeleteTable(string tableName)
    {
        await _tableService.DeleteTableAsync(tableName);
        return Ok();
    }

    [HttpGet("tables/{tableName}/entities")]
    public async Task<ActionResult<TableEntitiesResponse>> ListEntities(
        string tableName,
        [FromQuery] string? filter = null,
        [FromQuery] int? pageSize = null,
        [FromQuery] string? continuationToken = null)
    {
        try
        {
            var page = await _tableService.ListEntitiesAsync(tableName, filter, pageSize, continuationToken);
            
            return Ok(new TableEntitiesResponse
            {
                Entities = page.Values.Select(e => e.ToResponse()).ToList(),
                ContinuationToken = page.ContinuationToken
            });
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return NotFound($"Table '{tableName}' not found");
        }
    }

    [HttpGet("tables/{tableName}/entities/{partitionKey}/{rowKey}")]
    public async Task<ActionResult<TableEntityResponse>> GetEntity(
        string tableName,
        string partitionKey,
        string rowKey)
    {
        var entity = await _tableService.GetEntityAsync(tableName, partitionKey, rowKey);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity.ToResponse());
    }

    [HttpPost("tables/{tableName}/entities")]
    public async Task<IActionResult> UpsertEntity(
        string tableName,
        [FromBody] UpsertTableEntityRequest request)
    {
        var entity = request.ToTableEntity();
        await _tableService.UpsertEntityAsync(tableName, entity);
        return Ok();
    }

    [HttpDelete("tables/{tableName}/entities/{partitionKey}/{rowKey}")]
    public async Task<IActionResult> DeleteEntity(
        string tableName,
        string partitionKey,
        string rowKey)
    {
        try
        {
            await _tableService.DeleteEntityAsync(tableName, partitionKey, rowKey);
            return Ok();
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return NotFound();
        }
    }

    [HttpGet("tables/{tableName}/query")]
    public async Task<ActionResult<TableEntitiesResponse>> QueryEntities(
        string tableName,
        [FromQuery] string? filter = null)
    {
        try
        {
            var entities = await _tableService.QueryEntitiesAsync(tableName, filter);
            return Ok(new TableEntitiesResponse
            {
                Entities = entities.Select(e => e.ToResponse()).ToList()
            });
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return NotFound($"Table '{tableName}' not found");
        }
    }
}