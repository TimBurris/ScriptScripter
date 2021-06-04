using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ScriptScripter.Processor.Services.Tests
{

    [TestClass()]
    public class ScriptWarningService_Tests
    {
        private const string _expectedUseDbWarnText = "Line {0}: Appears to switch databases with a 'Use [db]' statement; typically you should let ScriptScripter control the database.";

        private ScriptWarningService _service;

        //act input params
        private string _inputSql;

        [TestInitialize]
        public void Init()
        {
            _service = new ScriptWarningService();
        }

        public IEnumerable<string> Act()
        {
            return _service.CheckSql(_inputSql);
        }

        [TestMethod]
        public void no_warnings_for_empty()
        {
            //arrange
            _inputSql = "";

            //act
            var result = this.Act();

            //assert
            result.Should().BeEmpty();
        }


        [TestMethod]
        public void no_warnings_for_null()
        {
            //arrange
            _inputSql = null;

            //act
            var result = this.Act();

            //assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void warns_of_use_db_at_beginning_of_script()
        {
            //arrange
            _inputSql = @"USE [MyTempDB];

CREATE TABLE [Company].[Company](
	[CompanyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) NOT NULL,
	[CreatedByUserId] [uniqueidentifier] NOT NULL,
	[CreatedOnDate] [datetimeoffset](7) NOT NULL,
	[UpdatedByUserId] [uniqueidentifier] NULL,
	[UpdatedOnDate] [datetimeoffset](7) NULL,
	[IsCurrent] [bit] NOT NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO";

            //act
            var result = this.Act();

            //assert
            result.Should().BeEquivalentTo(new string[] { string.Format(_expectedUseDbWarnText, arg0: 1) });

        }


        [TestMethod]
        public void warns_of_use_db_in_multiple_locations()
        {
            //arrange

            _inputSql = @"USE [MyTempDB];

CREATE TABLE [Company].[Company](
	[CompanyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) NOT NULL,
	[CreatedByUserId] [uniqueidentifier] NOT NULL,
	[CreatedOnDate] [datetimeoffset](7) NOT NULL,
	[UpdatedByUserId] [uniqueidentifier] NULL,
	[UpdatedOnDate] [datetimeoffset](7) NULL,
	[IsCurrent] [bit] NOT NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
    use otherdb
GO";

            //act
            var result = this.Act();

            //assert
            result.Should().BeEquivalentTo(new string[] {
                string.Format(_expectedUseDbWarnText, arg0: 1),
                string.Format(_expectedUseDbWarnText, arg0: 17),
            });
        }

        [TestMethod]
        public void does_not_warn_if_use_db_not_at_start_of_line()
        {
            _inputSql = @"
--USE DB
CREATE TABLE [Company].[Company](
	[CompanyId] [int] IDENTITY(1,1) NOT NULL, -- Use db
	[Name] [varchar](1000) NOT NULL,
	[CreatedByUserId] [uniqueidentifier] NOT NULL,
	[CreatedOnDate] [datetimeoffset](7) NOT NULL,
	[UpdatedByUserId] [uniqueidentifier] NULL,
	[UpdatedOnDate] [datetimeoffset](7) NULL,
	[IsCurrent] [bit] NOT NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO";

            //act
            var result = this.Act();

            //assert
            result.Should().BeEmpty();

        }

    }
}