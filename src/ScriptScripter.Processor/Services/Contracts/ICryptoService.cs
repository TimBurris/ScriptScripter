using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptScripter.Processor.Services.Contracts
{
    public interface ICryptoService
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}
