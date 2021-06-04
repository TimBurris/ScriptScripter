using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptScripter.Processor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace ScriptScripter.Processor.Services.Tests
{
    [TestClass()]
    public class CryptoServiceTests
    {
        [TestMethod()]
        [DataRow("a")]
        [DataRow("1")]
        [DataRow("!")]
        [DataRow("*")]
        [DataRow("pizza")]
        [DataRow("this is a test of the national broadcast system")]
        [DataRow("1qaz@WSX1qaz#EDC")]
        [DataRow("myPassw0rd")]
        public void EncryptDecrypt(string plainText)
        {
            var encryptService = new CryptoService();
            string encryptedValue = encryptService.Encrypt(plainText);

            encryptedValue.Should().NotBeNullOrWhiteSpace();
            encryptedValue.Should().NotBe(plainText);

            var decrypteService = new CryptoService();
            string decryptedValue = decrypteService.Decrypt(encryptedValue);

            decryptedValue.Should().Be(plainText);
        }
    }
}