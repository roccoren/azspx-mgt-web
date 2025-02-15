export interface EntityReference {
  self: string;
}

export interface DiarizationProperties {
  enabled: boolean;
  maxSpeakers?: number;
}

export interface TranscriptionProperties {
  wordLevelTimestampsEnabled: boolean;
  displayFormWordLevelTimestampsEnabled: boolean;
  channels?: number[];
  punctuationMode: string;
  profanityFilterMode: string;
  destinationContainerUrl?: string;
  timeToLiveHours: number;
  diarization?: DiarizationProperties;
  error?: string;
  durationInTicks: number;
}

export interface TranscriptionLinks {
  files: string;
}

export interface TranscriptionJob {
  id: string;
  displayName: string;
  status: string;
  createdDateTime: string;
  lastActionDateTime?: string;
  error?: string;
  locale: string;
  model?: EntityReference;
  properties: TranscriptionProperties;
  links?: TranscriptionLinks;
}

export interface TranscriptionJobRequest {
  displayName: string;
  locale: string;
  modelId?: string;
  audioUrl: string;
  enableDiarization: boolean;
  enableWordLevelTimestamps: boolean;
  enableDisplayFormWordLevelTimestamps: boolean;
  timeToLiveHours?: number;
}

export interface TranscriptionJobResponse {
  self: string;
  displayName: string;
  status: string;
  locale: string;
  createdDateTime: string;
  lastActionDateTime?: string;
  links?: TranscriptionLinks;
  properties: TranscriptionProperties;
}