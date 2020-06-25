CREATE TABLE [dbo].[User]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Name] NVARCHAR (255) NOT NULL,
	[AppId]    NVARCHAR (50)      NOT NULL, -- =app_id
    [DeviceId] NVARCHAR (50)      NOT NULL, -- =Token
    [TimestampAdded]    DATETIMEOFFSET (7) NOT NULL, 
	[TimestampUpdated]  DATETIMEOFFSET (7) NOT NULL, 
)
