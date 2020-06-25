CREATE TABLE [dbo].[Battery]
(
	[Id]           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Timestamp]    DATETIMEOFFSET (7) NOT NULL,
    [BatteryLevelPercentage] INT             NOT NULL, -- =batteryLevel
    [AppId]                  NVARCHAR (50)   NOT NULL, -- =app_id
    [DeviceId]               NVARCHAR (50)   NOT NULL, -- =Token
)
