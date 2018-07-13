Public Class ConnectionParameters
    Property UseTrustedConnection As Boolean
    Property Server As String
    Property Password As String
    Property Username As String

    Public Function IsLocalMachineConnection() As Boolean
        Dim s As String = Me.Server.ToUpper()
        If s.StartsWith("(LOCAL)") Then
            Return True
        End If

        Dim computerName = My.Computer.Name.ToUpper()

        If s = computerName OrElse s.StartsWith(computerName + "\") Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function GetTrustedConnectionParams(server As String) As ConnectionParameters
        Dim p As New ConnectionParameters()
        p.UseTrustedConnection = True
        p.Server = server

        Return p
    End Function

    Public Shared Function GetConnectionParams(server As String, userName As String, password As String) As ConnectionParameters
        Dim p As New ConnectionParameters()
        p.UseTrustedConnection = False
        p.Server = server
        p.Username = userName
        p.Password = password
        Return p
    End Function

    Public Function TestConnection(Optional ByRef errorMessage As String = Nothing) As Boolean
        Return ConnectionParameters.TestConnection(connectionParams:=Me, errorMessage:=errorMessage)
    End Function

    Public Shared Function TestConnection(ByVal connectionParams As ConnectionParameters, Optional ByRef errorMessage As String = Nothing) As Boolean
        Try
            'some cases can be tested without even trying to connect to the database
            If String.IsNullOrEmpty(connectionParams.Server) = True Then
                errorMessage = "Server is blank"
                Return False
            End If
            If connectionParams.UseTrustedConnection = False AndAlso (String.IsNullOrEmpty(connectionParams.Username) OrElse String.IsNullOrEmpty(connectionParams.Password)) Then
                errorMessage = "User or password"
                Return False
            End If

            Dim oServer = GetServerConnection(connectionParams:=connectionParams)
            oServer.ConnectionContext.Disconnect()
            oServer = Nothing

            Return True
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Function GetServerConnection() As Microsoft.SqlServer.Management.Smo.Server
        Return GetServerConnection(connectionParams:=Me)
    End Function

    Public Shared Function GetServerConnection(connectionParams As ConnectionParameters) As Microsoft.SqlServer.Management.Smo.Server
        Dim oSMOServer = New Microsoft.SqlServer.Management.Smo.Server()
        oSMOServer.ConnectionContext.LoginSecure = connectionParams.UseTrustedConnection
        oSMOServer.ConnectionContext.ServerInstance = connectionParams.Server
        If connectionParams.UseTrustedConnection = False Then
            oSMOServer.ConnectionContext.Password = connectionParams.Password
            oSMOServer.ConnectionContext.Login = connectionParams.Username
        End If
        'moSMOServer.ConnectionContext.ConnectTimeout = 5
        oSMOServer.ConnectionContext.Connect()

        Return oSMOServer
    End Function
End Class
