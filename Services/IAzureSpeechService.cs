using azspx_mgt_web.Models;

namespace azspx_mgt_web.Services;

public interface IAzureSpeechService
{
    Task<TranscriptionJobResponse> CreateTranscriptionJobAsync(TranscriptionJobRequest request);
    Task<TranscriptionJob> GetTranscriptionJobAsync(string jobId);
    Task<List<TranscriptionJob>> ListTranscriptionJobsAsync();
    Task DeleteTranscriptionJobAsync(string jobId);
    Task<bool> DeleteTranscriptionOutputAsync(string jobId);
}