using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using azspx_mgt_web.Models;
using azspx_mgt_web.Services;

namespace azspx_mgt_web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TranscriptionController : ControllerBase
{
    private readonly IAzureSpeechService _speechService;
    private readonly ILogger<TranscriptionController> _logger;

    public TranscriptionController(
        IAzureSpeechService speechService,
        ILogger<TranscriptionController> logger)
    {
        _speechService = speechService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<TranscriptionJobResponse>> CreateTranscriptionJob(
        TranscriptionJobRequest request)
    {
        try
        {
            var result = await _speechService.CreateTranscriptionJobAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transcription job");
            return StatusCode(500, new { error = "Failed to create transcription job" });
        }
    }

    [HttpGet("{transcriptionID}")]
    public async Task<ActionResult<TranscriptionJob>> GetTranscriptionJob(string transcriptionID)
    {
        try
        {
            var result = await _speechService.GetTranscriptionJobAsync(transcriptionID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transcription job");
            return StatusCode(500, new { error = "Failed to get transcription job" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<TranscriptionJob>>> ListTranscriptionJobs()
    {
        try
        {
            var result = await _speechService.ListTranscriptionJobsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing transcription jobs");
            return StatusCode(500, new { error = "Failed to list transcription jobs" });
        }
    }

    [HttpDelete("{transcriptionID}")]
    public async Task<IActionResult> DeleteTranscriptionJob(string transcriptionID)
    {
        try
        {
            await _speechService.DeleteTranscriptionJobAsync(transcriptionID);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transcription job");
            return StatusCode(500, new { error = "Failed to delete transcription job" });
        }
    }

    [HttpDelete("{transcriptionID}/output")]
    public async Task<IActionResult> DeleteTranscriptionOutput(string transcriptionID)
    {
        try
        {
            var result = await _speechService.DeleteTranscriptionOutputAsync(transcriptionID);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transcription output");
            return StatusCode(500, new { error = "Failed to delete transcription output" });
        }
    }
}