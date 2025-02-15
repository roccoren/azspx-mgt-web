using System;

namespace azspx_mgt_web.Models;

public class TranscriptionJob
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; }
    public DateTime? LastActionDateTime { get; set; }
    public TranscriptionError? Error { get; set; }
    public string Locale { get; set; } = "en-US";
    public EntityReference? Model { get; set; }
    public TranscriptionProperties Properties { get; set; } = new();
    public TranscriptionLinks? Links { get; set; }
}

public class EntityReference
{
    public string Self { get; set; } = string.Empty;
}

public class TranscriptionProperties
{
    public bool WordLevelTimestampsEnabled { get; set; }
    public bool DisplayFormWordLevelTimestampsEnabled { get; set; }
    public int[]? Channels { get; set; }
    public string PunctuationMode { get; set; } = "DictatedAndAutomatic";
    public string ProfanityFilterMode { get; set; } = "Masked";
    public string? DestinationContainerUrl { get; set; }
    public int TimeToLiveHours { get; set; } = 48;
    public DiarizationProperties? Diarization { get; set; }
    public TranscriptionError? Error { get; set; }
    public long DurationInTicks { get; set; }
}

public class TranscriptionError
{
    public string? Code { get; set; }
    public string? Message { get; set; }
}

public class DiarizationProperties
{
    public bool Enabled { get; set; }
    public int? MaxSpeakers { get; set; }
}

public class TranscriptionJobRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string Locale { get; set; } = "en-US";
    public string? ModelId { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public bool EnableDiarization { get; set; }
    public bool EnableWordLevelTimestamps { get; set; }
    public bool EnableDisplayFormWordLevelTimestamps { get; set; }
    public int? TimeToLiveHours { get; set; }
}

public class TranscriptionJobResponse
{
    public string Self { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; }
    public DateTime? LastActionDateTime { get; set; }
    public TranscriptionLinks? Links { get; set; }
    public TranscriptionProperties Properties { get; set; } = new();
}

public class TranscriptionLinks 
{
    public string Files { get; set; } = string.Empty;
}