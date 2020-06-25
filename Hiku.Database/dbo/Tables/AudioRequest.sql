CREATE TABLE [dbo].[AudioRequest]
(
	[Id]    UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,  --AudioToken
	[State] NVARCHAR (50)    NOT NULL, --RecordStarted|RecordFinished|SpeechToTextFinished
	[TimestampRecordStarted]    DATETIMEOFFSET (7) NOT NULL,
	[TimestampRecordFinished]   DATETIMEOFFSET (7) NULL,
    [TimestampSpeechToTextFinished]  DATETIMEOFFSET (7) NULL,
	[AudioFilePath]     NVARCHAR (1024)    NULL,
	[Sentence]          NVARCHAR (MAX)     NULL,
    [AppId]             NVARCHAR (50)      NOT NULL, -- =app_id
    [DeviceId]          NVARCHAR (50)      NOT NULL, -- =Token
)
