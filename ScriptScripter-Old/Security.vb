Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Friend Class Security

#Region " Password Encrypt/Decrypt "
    Private Shared ReadOnly Property KEY() As Byte()
        Get
            Dim sKey As String = "61EA47BDEA8F467D83C5D9FE" 'must be 24 chars or less ( it will be padded to 24)
            Return Encoding.Default.GetBytes(sKey.PadRight(24, Chr(0)))
        End Get
    End Property

    Private Shared ReadOnly Property VECTOR() As Byte()
        Get
            Dim sVector As String = "TJB666" 'must be 8 or less (it will be padded to 8)
            Return Encoding.Default.GetBytes(sVector.PadRight(8, Chr(0)))
        End Get
    End Property

    Friend Shared Function PasswordEncrypt(ByVal sPass As String) As String
        Return TransformText(sText:=sPass, CryptoTransform:=New TripleDESCryptoServiceProvider().CreateEncryptor(rgbKey:=KEY, rgbIV:=VECTOR))
    End Function

    Friend Shared Function PasswordDecrypt(ByVal sPass As String) As String
        Return TransformText(sText:=sPass, CryptoTransform:=New TripleDESCryptoServiceProvider().CreateDecryptor(rgbKey:=KEY, rgbIV:=VECTOR))
    End Function

    Private Shared Function TransformText(ByVal sText As String, ByVal CryptoTransform As ICryptoTransform) As String
        ' Create a MemoryStream.
        Dim mStream As New MemoryStream

        ' Create a CryptoStream using the MemoryStream 
        ' and the passed key and initialization vector (IV).
        Dim cStream As New CryptoStream(mStream, CryptoTransform, CryptoStreamMode.Write)

        ' Convert the passed string to a byte array.
        Dim input As Byte() = Encoding.Default.GetBytes(sText)

        ' Write the byte array to the crypto stream and flush it.
        cStream.Write(input, 0, input.Length)
        cStream.FlushFinalBlock()

        ' Get an array of bytes from the 
        ' MemoryStream that holds the 
        ' encrypted data.
        Dim ret As Byte() = mStream.ToArray()

        ' Close the streams.
        cStream.Close()
        mStream.Close()

        ' Return the encrypted buffer.
        Return Encoding.Default.GetString(ret)
    End Function
#End Region

End Class
