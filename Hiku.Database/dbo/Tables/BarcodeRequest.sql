CREATE TABLE [dbo].[BarcodeRequest]
(
	[Id]    UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[State] NVARCHAR (50)    NOT NULL, -- FlashFinished|...
	[TimestampFlashFinished]    DATETIMEOFFSET (7) NOT NULL,
    [Barcode]           NVARCHAR (50)      NOT NULL, -- =ean
    [AppId]             NVARCHAR (50)      NOT NULL, -- =app_id
    [DeviceId]          NVARCHAR (50)      NOT NULL, -- =Token
)
