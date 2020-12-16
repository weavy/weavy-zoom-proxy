CREATE TABLE [dbo].[ZoomEvents](
	[Id] [int] IDENTITY(1,1) NOT NULL,	
	[CreatedAt] [datetime2](7) NOT NULL,	
	[MeetingId] [nvarchar](128) NOT NULL,
	[Uuid] [nvarchar](128) NOT NULL,
	[EventType] [nvarchar](32) NOT NULL,	
	[MetaData] [nvarchar](512) NULL,	
 CONSTRAINT [PK_Meetings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO