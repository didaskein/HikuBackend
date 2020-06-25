CREATE TABLE [dbo].[Device]
(
	[Id]     NVARCHAR(50)         NOT NULL PRIMARY KEY, -- =Token / DeviceId
    [AppId]  NVARCHAR (50)        NOT NULL,  -- =app_id
	[AgentUrl]   NVARCHAR (1024)  NOT NULL,  -- =Token
)
