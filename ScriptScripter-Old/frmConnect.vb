Option Strict On
Option Explicit On
Imports Microsoft.SqlServer.Management.Smo

Friend Class frmConnect
    Inherits System.Windows.Forms.Form

    Private Sub LoadServersCombo()
        cboServer.Items.Clear()
        cboServer.Sorted = True

        'AddServersInGroup((moSQLApp))
        'TODO: load MRU
        'LoadAvailServers2005MRU()
    End Sub

    Private Sub LoadAvailServers()
        'Dim lCount As Integer

        'For lCount = 1 To moSQLApp.ListAvailableSQLServers.Count
        '	cboServer.Items.Add(moSQLApp.ListAvailableSQLServers.Item(lCount))
        '      Next lCount

        Dim dtlSQLServers As DataTable

        ' Get list of all available servers.
        dtlSQLServers = SmoApplication.EnumAvailableSqlServers(False)

        ' Display the list of all available servers and
        ' identify the local sql server.
        For Each drServer As DataRow In dtlSQLServers.Rows
            cboServer.Items.Add(drServer.Item("Name"))
        Next

        dtlSQLServers.Dispose()
        dtlSQLServers = Nothing
    End Sub

    'Private Sub LoadAvailServers2005MRU()
    '    Dim sMRUFile As String
    '    sMRUFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)

    '    If String.IsNullOrEmpty(sMRUFile) = True Then
    '        Exit Sub
    '    End If

    '    If sMRUFile.EndsWith("\") = False Then
    '        sMRUFile &= "\"
    '    End If

    '    sMRUFile &= "Microsoft\Microsoft SQL Server\90\Tools\Shell\mru.dat"

    '    If IO.File.Exists(sMRUFile) = False Then
    '        Exit Sub
    '    End If

    '    Try
    '        Dim sFileContents As String

    '        'read all contents into a string
    '        sFileContents = IO.File.ReadAllText(sMRUFile)

    '        'TJB: ok, the logic here... well i HOPE it's correct... basically i took my MRU file and opened it in .NET to view it
    '        '   what i found was that the server names are all listed like this: Database Engine@myserver\instance@
    '        '  so below i find the Database Engine@ string, then take everything before the next @
    '        '   if you open you MRU you will see your servers listed manytimes for different things, that Database Engine@ was what i found to be most consistent.
    '        If String.IsNullOrEmpty(sFileContents) = False Then
    '            Dim sDE() As String = sFileContents.Split(separator:=New String() {"Database Engine@"}, options:=StringSplitOptions.RemoveEmptyEntries)

    '            For Each s As String In sDE
    '                Dim sAt() As String
    '                sAt = Split(s, "@")
    '                If sAt IsNot Nothing AndAlso sAt.Length > 1 Then
    '                    sAt(0) = Trim(sAt(0))
    '                    If String.IsNullOrEmpty(sAt(0)) = False AndAlso cboServer.Items.Contains(sAt(0)) = False Then
    '                        cboServer.Items.Add(sAt(0))
    '                    End If
    '                End If
    '            Next
    '        End If

    '    Catch ex As Exception
    '        'ignore error?
    '        If My.Settings.Show2005MRUErrors = True Then
    '            If MessageBox.Show(Me, "Error occured processing '" & sMRUFile & "'.  This is not critical, just means we could not load your list of recently used servers from SQL Server 2005 Studio.  The error was: " & ex.Message & vbCrLf & "Do you want to continue recieving this message?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = Windows.Forms.DialogResult.No Then
    '                My.Settings.Show2005MRUErrors = False
    '            End If
    '        End If
    '    End Try

    'End Sub

    Private Sub AddServersInGroup(ByRef oGroups As Microsoft.SqlServer.Management.Smo.SmoApplication)
        ''Dim oGroup As SQLDMO.ServerGroup
        ''Dim oRegSer As SQLDMO.RegisteredServer
        'Dim oGroup As Microsoft.SqlServer.Management.Smo.RegisteredServers.ServerGroup
        'Dim oRegSer As Microsoft.SqlServer.Management.Smo.RegisteredServers.RegisteredServer

        'For Each o In oGroup.RegisteredServers.GetEnumerator

        'Next o

        'For Each oGroup In oGroups
        '    For Each oRegSer In oGroup.RegisteredServers
        '        cboServer.Items.Add(oRegSer.Name) 'oGroup.Name & "\" & oRegSer.Name
        '    Next oRegSer

        '    AddServersInGroup(oGroup.ServerGroups)
        'Next oGroup
        LoadAvailServers()
    End Sub

    Private Function GetConnectionParams() As ConnectionParameters
        Dim p As New ConnectionParameters()
        p.Server = Me.ServerName
        p.Username = Me.UserName
        p.Password = Me.Password
        p.UseTrustedConnection = Me.UseTrustedConnection

        Return p
    End Function

    Private Sub cmdConnect_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdConnect.Click
        Try
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
            mod_Common.CurrentConnectionParameters = Me.GetConnectionParams()

            If chkSave.CheckState = System.Windows.Forms.CheckState.Checked Then
                SavePrefs()
            End If

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MsgBox("Unable to connect to SQL Server for the following reason: " & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical, Me.Text)
        Finally
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
        End Try
    End Sub

    Private Sub cmdExit_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdExit.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub cmdLoadAvail_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdLoadAvail.Click
        On Error GoTo EH

        'UPGRADE_WARNING: Screen property Screen.MousePointer has a new behavior. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
        LoadAvailServers()
        'UPGRADE_WARNING: Screen property Screen.MousePointer has a new behavior. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
        Exit Sub
EH:
        MsgBox(Err.Description)
    End Sub

    Public Property ServerName As String
        Get
            Return cboServer.Text
        End Get
        Set(value As String)
            cboServer.Text = value
        End Set
    End Property

    Public Property UseTrustedConnection As Boolean
        Get
            Return chkSecure.Checked
        End Get
        Set(value As Boolean)
            chkSecure.Checked = value
        End Set
    End Property

    Public Property UserName As String
        Get
            Return txtLogin.Text
        End Get
        Set(value As String)
            txtLogin.Text = value
        End Set
    End Property

    Public Property Password As String
        Get
            Return txtPass.Text
        End Get
        Set(value As String)
            txtPass.Text = value
        End Set
    End Property

    Private Sub SavePrefs()
        Try
            My.Settings.DBConnection_Server = cboServer.Text
            My.Settings.DBConnection_User = txtLogin.Text
            My.Settings.DBConnection_Password = txtPass.Text
            My.Settings.DBConnection_UseTrustedConnection = chkSecure.Checked

        Catch ex As Exception
            MessageBox.Show(Me, "User settings could not be saved.  The following error occured while saving:" & ex.Message & vbCrLf & ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub LoadPrefs()
        Try
            'note: passwords are not stored or restored
            cboServer.Text = My.Settings.DBConnection_Server
            txtLogin.Text = My.Settings.DBConnection_User
            txtPass.Text = My.Settings.DBConnection_Password
            chkSecure.Checked = My.Settings.DBConnection_UseTrustedConnection

        Catch ex As Exception
            'Throw
            MessageBox.Show(Me, "User settings could not be loaded.  The following error occured while Loading:" & ex.Message & vbCrLf & ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub frmConnect_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        LoadServersCombo()
        LoadPrefs()
    End Sub

    Private Sub cmdTest_Click(sender As Object, e As EventArgs) Handles cmdTest.Click
        'testing
        Try
            Me.Cursor = Cursors.WaitCursor
            Dim p = Me.GetConnectionParams()
            Dim message As String = String.Empty

            If p.TestConnection(errorMessage:=message) Then
                MessageBox.Show(Me, "Connection Succeeded.", "Success", MessageBoxButtons.OK)
            Else
                MessageBox.Show(Me, "Connection Failed (" + message + ").", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub cboServer_TextChanged(sender As Object, e As EventArgs) Handles cboServer.TextChanged
        'you are NOT allowed to save connection settings for anything but your local machine
        If Me.GetConnectionParams().IsLocalMachineConnection() = True Then
            chkSave.Enabled = True
            chkSave.Checked = True
        Else
            chkSave.Enabled = False
            chkSave.Checked = False
        End If
    End Sub
End Class