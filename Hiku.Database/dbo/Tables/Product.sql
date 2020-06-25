CREATE TABLE [dbo].[Product]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[TimestampAdded]    DATETIMEOFFSET (7) NOT NULL,
    [Barcode]           NVARCHAR (50)      NOT NULL, -- =ean
)
