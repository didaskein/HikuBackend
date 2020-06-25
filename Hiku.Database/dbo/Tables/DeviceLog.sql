CREATE TABLE [dbo].[DeviceLog]
(
	[Id]           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Timestamp]    DATETIMEOFFSET (7) NOT NULL, 
    [AppId]        NVARCHAR (50)   NOT NULL, -- =app_id
    [DeviceId]     NVARCHAR (50)   NOT NULL, -- =serialNumber
	[Data]         NVARCHAR(MAX)  NOT NULL,  -- json(logData)
)
