using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace ScriptScripter.Processor.Data.Models.Tests
{
    [TestClass()]
    public class DatabaseConnectionParametersTests
    {
        [TestMethod()]
        public void GetConnectionString_user_pass()
        {
            /*************  arrange  ******************/
            var connParams = new DatabaseConnectionParameters()
            {
                Server = "(local)\\Ninja",
                Username = "SamHarris",
                Password = "*8675309!",
                DatabaseName = "MyDb"
            };

            /*************    act    ******************/
            var result = connParams.GetConnectionString();

            /*************  assert   ******************/
            result.Should().Be("Data Source=(local)\\Ninja;Initial Catalog=MyDb;User ID=SamHarris;Password=*8675309!");
        }

        [TestMethod]
        public void GetConnectionString_trusted()
        {

            /*************  arrange  ******************/
            var connParams = new DatabaseConnectionParameters()
            {
                Server = "(local)\\Ninja",
                UseTrustedConnection = true,
                Username = "SamHarris", //should be ignored
                Password = "*8675309!",//should be ignored
                DatabaseName = "MyDb"
            };

            /*************    act    ******************/
            var result = connParams.GetConnectionString();

            /*************  assert   ******************/
            result.Should().Be("Data Source=(local)\\Ninja;Initial Catalog=MyDb;Integrated Security=True");

        }
    }
}