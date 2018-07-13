Option Strict On
Option Explicit On

Module mod_Common
    Public Const XML_FILENAME As String = "DBScripts"

    Private Const _FileNamePrefix As String = "DBScripts_"
    Private Const _FileNameSuffix As String = ".xml"

    Public gDSDBs As cdsDBNames
    Public goDS As dsRevisions

    Private msFileName As String
    Private modsError As dsErrorInfo

    Public Property CurrentConnectionParameters As ConnectionParameters

    Public Sub LoadDBConnectionSettings()
        CurrentConnectionParameters = New ConnectionParameters()
        CurrentConnectionParameters.Server = My.Settings.DBConnection_Server
        CurrentConnectionParameters.Username = My.Settings.DBConnection_User
        CurrentConnectionParameters.Password = My.Settings.DBConnection_Password
        CurrentConnectionParameters.UseTrustedConnection = My.Settings.DBConnection_UseTrustedConnection
    End Sub

    Public Function InitXMLDoc(ByVal sDBName As String, ByVal sFileName As String, ByRef sErrorMessage As String) As Boolean

        'make sure the xml document exists
        If Not goDS Is Nothing Then
            goDS.Dispose()
            goDS = Nothing
        End If

        goDS = New dsRevisions

        Try
            Dim fileInfo As System.IO.FileInfo = New IO.FileInfo(fileName:=sFileName)
            'load the xml if the file exists and it has data (if the length is 0 the file exists but is blank, which is fine, no errors fro having a blank file, we simply just do not load any content.  if we attempted to read a blank file we would get an error saying root element did not exist)
            If fileInfo.Exists AndAlso fileInfo.Length > 0 Then
                goDS.ReadXml(sFileName)
            End If

            Return (True)
        Catch ex As Exception
            'sErrorMessage = "Error initializing '" & goDoc.parseError.url & "': " & goDoc.parseError.reason
            sErrorMessage = ex.Message
            Return (False)
        End Try
    End Function

    Public Function GetXMLFileName(ByVal sDBName As String) As String
        If Len(sDBName) > 0 Then sDBName = "_" & sDBName
        Return FixPath(Application.StartupPath) & XML_FILENAME & sDBName & ".xml"
    End Function

    Public Function FixPath(ByVal sPath As String) As String
        If Right(sPath, 1) <> "\" Then
            sPath = sPath & "\"
        End If

        FixPath = sPath
    End Function

    Friend Sub LoadDBList()
        Dim oRow As cdsDBNames.DBListRow
        Dim i As Integer
        Dim oFiles As Generic.List(Of System.IO.FileInfo)
        Dim oDir As New System.IO.DirectoryInfo(Application.StartupPath)

        oFiles = System.Linq.Enumerable.ToList(oDir.GetFiles(_FileNamePrefix & "*" & _FileNameSuffix, IO.SearchOption.TopDirectoryOnly))

        If Not gDSDBs Is Nothing Then
            gDSDBs.Dispose()
        End If

        gDSDBs = New cdsDBNames

        Dim updater As cUpdateMultipleDBs = Nothing
        Try
            If mod_Common.CurrentConnectionParameters.TestConnection() Then
                updater = New cUpdateMultipleDBs()
            End If

            For i = 1 To oFiles.Count
                oRow = CType(gDSDBs.DBList.NewRow, cdsDBNames.DBListRow)
                'oRow.DBName = System.Configuration.ConfigurationSettings.AppSettings.Item(CType(i, String))
                oRow.XMLFileName = oFiles(i - 1).Name
                oRow.DBName = GetDBNameFromFileName(oRow.XMLFileName)
                oRow.DBID = i
                oRow.DBIsConnected = If(updater Is Nothing, False, updater.TestDatabaseConnection(oRow.DBName))
                oRow.XMLFileIsReadOnly = New System.IO.FileInfo(oRow.XMLFileName).IsReadOnly
                If oRow.DBIsConnected Then
                    oRow.LastRevRun = updater.GetCurrentDBRevisionID(db:=updater.GetDatabaseConnection(sDBName:=oRow.DBName), sDBName:=oRow.DBName)

                    'now we're going to get the revision number from the xml file:
                    If InitXMLDoc(sDBName:=oRow.DBName, sFileName:=oRow.XMLFileName, sErrorMessage:="XML document could not be loaded for db:  " & oRow.DBName) Then
                        If goDS.Revisions.Count > 0 Then
                            oRow.XMLRevision = goDS.Revisions.Item(goDS.Revisions.Count - 1).Revision_ID
                        Else
                            oRow.XMLRevision = 0
                        End If
                        oRow.RevisionsToRun = oRow.XMLRevision - oRow.LastRevRun
                    End If
                Else
                    oRow.RevisionsToRun = 0
                End If
                gDSDBs.DBList.Rows.Add(oRow)
            Next i

        Finally
            If updater IsNot Nothing Then
                updater.Dispose()
                updater = Nothing
            End If
        End Try
        oRow = Nothing

    End Sub

    Public Function GetFileNameFromDBName(ByVal sDBName As String) As String
        Return _FileNamePrefix & sDBName & _FileNameSuffix
    End Function

    Public Function GetDBNameFromFileName(ByVal sFileName As String) As String
        Dim sName As String
        sName = sFileName.Remove(0, _FileNamePrefix.Length)
        Return Strings.Left(sName, (sName.Length - _FileNameSuffix.Length))
    End Function

    Friend Sub CloseObjects()
        If Not gDSDBs Is Nothing Then
            gDSDBs.Dispose()
            gDSDBs = Nothing
        End If
        If Not goDS Is Nothing Then
            goDS.Dispose()
            goDS = Nothing
        End If
        'If Not goMasterDB Is Nothing Then
        '    goMasterDB.Dispose()
        '    goMasterDB = Nothing
        'End If

        If Not modsError Is Nothing Then
            modsError.Dispose()
            modsError = Nothing
        End If
    End Sub

    Friend Sub AddGenericError(ByVal sError As String, ByVal sErrorRoutine As String)
        Dim oRow As dsErrorInfo.GenericErrorsRow

        If modsError Is Nothing Then
            modsError = New dsErrorInfo
        End If
        oRow = modsError.GenericErrors.NewGenericErrorsRow

        oRow.ErrorText = sError
        oRow.ErrorSub = sErrorRoutine
        modsError.GenericErrors.AddGenericErrorsRow(oRow)

        oRow = Nothing
    End Sub
    Friend Sub AddGenericError(ex As Exception)
        Dim oRow As dsErrorInfo.GenericErrorsRow

        If modsError Is Nothing Then
            modsError = New dsErrorInfo
        End If

        Dim tempEx As Exception
        tempEx = ex

        While tempEx IsNot Nothing
            oRow = modsError.GenericErrors.NewGenericErrorsRow

            oRow.ErrorText = ex.Message
            oRow.ErrorSub = "StackTrace: " + tempEx.StackTrace

            modsError.GenericErrors.AddGenericErrorsRow(oRow)

            oRow = Nothing
            tempEx = tempEx.InnerException
        End While
    End Sub

    Friend Sub OutputError()
        If modsError Is Nothing Then Return

        If modsError.GenericErrors.Any() OrElse modsError.RevisionErrors.Any() Then
            LaunchErrorForm()
            'now i think we should clear errors, right?
            modsError = Nothing
        End If
    End Sub

    Private Sub LaunchErrorForm()
        Dim ofrm As New frmError

        ofrm.ErrorDataSet = modsError
        ofrm.ShowDialog()

        ofrm.Dispose()
        ofrm = Nothing
    End Sub

    Friend ReadOnly Property GetDSError() As dsErrorInfo
        Get
            If modsError Is Nothing Then
                modsError = New dsErrorInfo
            End If
            Return modsError
        End Get
    End Property
End Module

