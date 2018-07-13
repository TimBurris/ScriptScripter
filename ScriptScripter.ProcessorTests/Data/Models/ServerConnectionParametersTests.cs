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
    public class ServerConnectionParametersTests
    {
        [TestMethod()]
        public void GetConnectionString_user_pass()
        {
            /*************  arrange  ******************/
            var connParams = new ServerConnectionParameters()
            {
                Server = "(local)\\Ninja",
                Username = "JackSparrow",
                Password = "dumpster!"
            };

            /*************    act    ******************/
            var result = connParams.GetConnectionString();

            /*************  assert   ******************/
            result.Should().Be("Data Source=(local)\\Ninja;User ID=JackSparrow;Password=dumpster!");
        }

        [TestMethod]
        public void GetConnectionString_trusted()
        {

            /*************  arrange  ******************/
            var connParams = new ServerConnectionParameters()
            {
                Server = "(local)\\Ninja",
                UseTrustedConnection = true,
                Username = "JackSparrow", //should be ignored
                Password = "dumpster!"//should be ignored
            };

            /*************    act    ******************/
            var result = connParams.GetConnectionString();

            /*************  assert   ******************/
            result.Should().Be("Data Source=(local)\\Ninja;Integrated Security=True");

        }
    }
}