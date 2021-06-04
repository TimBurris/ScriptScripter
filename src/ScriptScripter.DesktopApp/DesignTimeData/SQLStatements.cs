using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.DesktopApp.DesignTimeData
{
    public static class SqlStatements
    {
        private static string[] _statements;

        public static string[] Items
        {
            get
            {
                if (_statements == null)
                    _statements = new string[]
                    {
                    @"
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sysmail_account]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[sysmail_account](
	[account_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [sysname] NOT NULL,
	[description] [nvarchar](256) NULL,
	[email_address] [nvarchar](128) NOT NULL,
	[display_name] [nvarchar](128) NULL,
	[replyto_address] [nvarchar](128) NULL,
	[last_mod_datetime] [datetime] NOT NULL,
	[last_mod_user] [sysname] NOT NULL,
 CONSTRAINT [SYSMAIL_ACCOUNT_IDMustBeUnique] PRIMARY KEY CLUSTERED 
(
	[account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [SYSMAIL_ACCOUNT_NameMustBeUnique] UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__sysmail_a__last___2F9A1060]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[sysmail_account] ADD  DEFAULT (getdate()) FOR [last_mod_datetime]
END

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF__sysmail_a__last___308E3499]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[sysmail_account] ADD  DEFAULT (suser_sname()) FOR [last_mod_user]
END

GO",


                    //********************************************************************************************************************
                    //********************************************************************************************************************
                    //********************************************************************************************************************
                    @"ALTER PROCEDURE [dbo].[sp_add_alert]
  @name                         sysname,
  @message_id                   INT              = 0,
  @severity                     INT              = 0,
  @enabled                      TINYINT          = 1,
  @delay_between_responses      INT              = 0,
  @notification_message         NVARCHAR(512)    = NULL,
  @include_event_description_in TINYINT          = 5,    -- 0 = None, 1 = Email, 2 = Pager, 4 = NetSend, 7 = All
  @database_name                sysname          = NULL,
  @event_description_keyword    NVARCHAR(100)    = NULL,
  @job_id                       UNIQUEIDENTIFIER = NULL, -- If provided must NOT also provide job_name
  @job_name                     sysname          = NULL, -- If provided must NOT also provide job_id
  @raise_snmp_trap              TINYINT          = 0,
  @performance_condition        NVARCHAR(512)    = NULL, -- New for 7.0
  @category_name                sysname          = NULL, -- New for 7.0
  @wmi_namespace                sysname             = NULL, -- New for 9.0
  @wmi_query                    NVARCHAR(512)     = NULL  -- New for 9.0
AS
BEGIN
  DECLARE @verify_alert         INT
  
  --Always verify alerts before adding
  SELECT @verify_alert = 1

  EXECUTE msdb.dbo.sp_add_alert_internal @name,
                                         @message_id,
                                         @severity,
                                         @enabled,
                                         @delay_between_responses,
                                         @notification_message,
                                         @include_event_description_in,
                                         @database_name,
                                         @event_description_keyword,
                                         @job_id,
                                         @job_name,
                                         @raise_snmp_trap,
                                         @performance_condition,
                                         @category_name,
                                         @wmi_namespace,
                                         @wmi_query,
                                         @verify_alert
END
",
                    //********************************************************************************************************************
                    //********************************************************************************************************************
                    //********************************************************************************************************************
@"INSERT INTO [dbo].[sysmail_account]
           ([name]
           ,[description]
           ,[email_address]
           ,[display_name]
           ,[replyto_address]
           ,[last_mod_datetime]
           ,[last_mod_user])
     VALUES
           (<name, sysname,>
           ,<description, nvarchar(256),>
           ,<email_address, nvarchar(128),>
           ,<display_name, nvarchar(128),>
           ,<replyto_address, nvarchar(128),>
           ,<last_mod_datetime, datetime,>
           ,<last_mod_user, sysname,>)
GO

",
                    //********************************************************************************************************************
                    //********************************************************************************************************************
                    //********************************************************************************************************************
@"ALTER TABLE dbo.AwesomeTable ADD [VeryImportant] BIT
GO
UPDATE dbo.AwesomeTable SET [VeryImportant] = 1
GO 
ALTER TABLE dbo.AwesomeTable ALTER COLUMN [VeryImportant] BIT NOT NULL
"
                    };
                return _statements;
            }
        }
    }
}
