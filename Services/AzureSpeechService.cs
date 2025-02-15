using System.Text.Json;
using azspx_mgt_web.Models;
using Microsoft.Extensions.Configuration;

namespace azspx_mgt_web.Services;

public class AzureSpeechService : IAzureSpeechService
{
    private readonly HttpClient _httpClient;
    private readonly string _subscriptionKey;
    private readonly string _region;
    private readonly ILogger<AzureSpeechService> _logger;
    private readonly string _baseUrl;
    private const string API_VERSION = "2024-11-15";

    public AzureSpeechService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AzureSpeechService> logger)
    {
        _httpClient = httpClient;
        _subscriptionKey = configuration["AzureSpeech:SubscriptionKey"] 
            ?? throw new ArgumentNullException("AzureSpeech:SubscriptionKey");
        _region = configuration["AzureSpeech:Region"] 
            ?? throw new ArgumentNullException("AzureSpeech:Region");
        _logger = logger;
        _baseUrl = $"https://{_region}.api.cognitive.microsoft.com/speechtotext";
        
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
    }

    public async Task<TranscriptionJobResponse> CreateTranscriptionJobAsync(TranscriptionJobRequest request)
    {
        try
        {
            var content = JsonSerializer.Serialize(new
            {
                contentUrls = new[] { request.AudioUrl },
                properties = new TranscriptionProperties
                {
                    Diarization = request.EnableDiarization ? new DiarizationProperties { Enabled = true } : null,
                    WordLevelTimestampsEnabled = request.EnableWordLevelTimestamps,
                    DisplayFormWordLevelTimestampsEnabled = request.EnableDisplayFormWordLevelTimestamps,
                    PunctuationMode = "DictatedAndAutomatic",
                    ProfanityFilterMode = "Masked",
                    TimeToLiveHours = request.TimeToLiveHours ?? 48
                },
                locale = request.Locale,
                displayName = request.DisplayName,
                model = !string.IsNullOrEmpty(request.ModelId) 
                    ? new EntityReference { Self = request.ModelId }
                    : null
            });

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/transcriptions:submit?api-version={API_VERSION}",
                new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TranscriptionJobResponse>();
            return result ?? throw new Exception("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transcription job");
            throw;
        }
    }

    public async Task<TranscriptionJob> GetTranscriptionJobAsync(string jobId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/transcriptions/{jobId}?api-version={API_VERSION}"
            );
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TranscriptionJob>();
            return result ?? throw new Exception("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transcription job status");
            throw;
        }
    }

    public async Task<List<TranscriptionJob>> ListTranscriptionJobsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/transcriptions?api-version={API_VERSION}"
            );
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaginatedTranscriptions>();
            if (result?.Values == null) return new List<TranscriptionJob>();

            // Extract ID from Self URL for each job
            foreach (var job in result.Values)
            {
                if (job.Links?.Files != null)
                {
                    // The Files URL contains the job ID in the format: .../transcriptions/{jobId}/files
                    var match = System.Text.RegularExpressions.Regex.Match(
                        job.Links.Files,
                        @"/transcriptions/([^/]+)/files"
                    );
                    if (match.Success)
                    {
                        job.Id = match.Groups[1].Value;
                    }
                }
            }

            return result.Values;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing transcription jobs");
            throw;
        }
    }

    public async Task DeleteTranscriptionJobAsync(string jobId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(
                $"{_baseUrl}/transcriptions/{jobId}?api-version={API_VERSION}"
            );
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transcription job");
            throw;
        }
    }

    public async Task<bool> DeleteTranscriptionOutputAsync(string jobId)
    {
        try
        {
            var job = await GetTranscriptionJobAsync(jobId);
            if (job.Links?.Files == null)
            {
                return false;
            }

            var response = await _httpClient.DeleteAsync(job.Links.Files);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transcription output");
            return false;
        }
    }

    private class PaginatedTranscriptions
    {
        public List<TranscriptionJob> Values { get; set; } = new();
    }
}