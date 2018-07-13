Option Strict On

Imports System.IO
Imports Microsoft.SqlServer.Management.Smo

Friend Class cUpdateMultipleDBs
    Implements IDisposable

    'Private Const REVISION_PROPERTY As String = "Script_Revision2"
    'Private Const BULKINSERTTEXTFLAG As String = "[**BULKINSERTPATH**]"
    'Private Const BULKINSERTFOLDER As String = "BulkInsertFiles"
    Private moSplash As frmSplash

    Private lstDBConnection As System.Collections.Generic.Dictionary(Of String, Database)

    Private moSMOServer As Server

    Public Function TestDatabaseConnection(ByVal sDBName As String) As Boolean
        Return (GetDatabaseConnection(sDBName:=sDBName) IsNot Nothing)
    End Function

    Public Function GetDatabaseConnection(ByVal sDBName As String) As Database
        Dim sKey As String = sDBName.ToLower() 'key will be all lower
        Dim db As Database

        If lstDBConnection Is Nothing Then
            lstDBConnection = New System.Collections.Generic.Dictionary(Of String, Database)()
        End If

        If lstDBConnection.ContainsKey(sKey) = True Then
            db = lstDBConnection.Item(key:=sKey)
        Else
            Try
                db = moSMOServer.Databases.Item(sDBName)
            Catch ex As Exception
                db = Nothing
            End Try

            lstDBConnection.Add(key:=sKey, value:=db)
        End If

        Return db
    End Function

    Public Sub New()
        Me.New(connectionParams:=mod_Common.CurrentConnectionParameters)
    End Sub

    Public Sub New(connectionParams As ConnectionParameters)
        Try
            If moSMOServer Is Nothing Then
                Dim errorMessage As String = String.Empty
                'call "test" first because it has some "smarts" built in to make it fail faster than just "getserverconnection"
                If connectionParams.TestConnection(errorMessage:=errorMessage) = True Then
                    moSMOServer = connectionParams.GetServerConnection()
                Else
                    mod_Common.AddGenericError("Cannot connect to database server: " + errorMessage, "cUpdateMultipleDBs_Init")
                End If
            End If

            Exit Sub
        Catch ex As Exception
            mod_Common.AddGenericError("Cannot connect to database server.", "cUpdateMultipleDBs_Init")
            mod_Common.AddGenericError(ex)
            'MBox("Cannot connect to database server.", MBoxStyle.Information)
        End Try
    End Sub

    Public Function PerformUpdates(ByVal owner As IWin32Window) As Boolean
        Dim bValid As Boolean

        Try
            moSplash = New frmSplash
            'moSplash.TopMost = True
            'moSplash.TopLevel = True
            moSplash.Show(owner:=owner)
            moSplash.UpdateStatusLabel("Checking for Database Updates...")

            Try
                bValid = ApplyRevisions()
            Catch ex As Exception
                mod_Common.AddGenericError("Unexpected Error in 'ProcessPatches': " & ex.Message, "cUpdateMultipleDBs_ProcessPatches")
                mod_Common.AddGenericError(ex)
            End Try


            Return bValid
        Catch ex As Exception
            mod_Common.AddGenericError("Unexpected Error in 'PerformDBUpdate': " & ex.Message, "cUpdateMultipleDBs_PerformDBUpdate")
            mod_Common.AddGenericError(ex)

        Finally
            If moSplash IsNot Nothing Then
                moSplash.Close()
                moSplash.Dispose()
                moSplash = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' Gets a datatable with all the revisions that need to be run, the table will be sorted, so the order of the rows in the table is the order they run
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAllRevisionsToRun() As dsRevisions.ApplyRevisionDataTable
        Dim tblRevsToApply As dsRevisions.ApplyRevisionDataTable = New dsRevisions().ApplyRevision 'note, we need to initialize the dataset, not just the table, because the DS has to be passed to a function for merging

        For Each oDBRow As cdsDBNames.DBListRow In gDSDBs.DBList
            If oDBRow.DBIsConnected = True Then ' DBIsConnected tells us if the test connection worked earlier TestConnection(oDBRow.DBName) Then
                Me.GetRevisions(sDBName:=oDBRow.DBName, lMinDBRev:=oDBRow.LastRevRun + 1, ds:=CType(tblRevsToApply.DataSet, dsRevisions))
            End If
        Next oDBRow

        'now tblRevsToApply should holds all the revisions that should be applied for all of the databases
        Dim sSortString As String = tblRevsToApply.RevisionDateColumn.ColumnName + ", " + tblRevsToApply.Revision_IDColumn.ColumnName
        Dim tblReturn As New dsRevisions.ApplyRevisionDataTable
        Dim iSequence As Integer = 1
        'now we want to fill our return row with all the rows in the order inwhich they should be run
        For Each rw In CType(tblRevsToApply.Select(filterExpression:=Nothing, sort:=sSortString), dsRevisions.ApplyRevisionRow())
            'we don't care about the sequence it HAD, we have applied a sort, and that sort is what we will use
            rw.SequenceToRun = iSequence
            tblReturn.ImportRow(rw)
            iSequence += 1
        Next

        tblRevsToApply.DataSet.Dispose()
        tblRevsToApply = Nothing

        Return tblReturn
    End Function

    Private Function ApplyRevisions() As Boolean
        'the idea of this procedure is this:
        ' 1. Find all the databases we plan to check/update by finding all the DBScript files
        ' 2. for each DB
        '    a. find(it) 's current Revision number
        '    b. make a table of all revisions from the script file where the revision is greater than the database's current revision level
        ' 3. apply the revisions in order of DateScripted

        Dim sSQL As String
        Dim sLogSQL As String
        Dim iTotalCount As Integer
        Dim iPosition As Integer
        Dim bSuccessful As Boolean
        Dim sCurrentDatabase As String
        Dim oDict As New Hashtable
        Dim lDBRev As Long
        Dim sDeveloper As String = "<not implemented>"
        Dim sMachine As String = Environment.MachineName
        'this table will store ALL revisions that need to applied to all DB's
        Dim tblRevsToApply As dsRevisions.ApplyRevisionDataTable
        Dim db As Database
        Dim oRow As dsRevisions.ApplyRevisionRow

        On Error GoTo EH_Generic
        'start by clearing errors:
        mod_Common.GetDSError.Clear()

        For Each oDBRow As cdsDBNames.DBListRow In gDSDBs.DBList
            If oDBRow.DBIsConnected = True Then ' DBIsConnected tells us if the test connection worked earlier TestConnection(oDBRow.DBName) Then
                CreatePropertiesAndDependents(db:=GetDatabaseConnection(sDBName:=oDBRow.DBName))
                oDict.Add(oDBRow.DBName.ToUpper, oDBRow.LastRevRun)
            End If
        Next oDBRow

        tblRevsToApply = Me.GetAllRevisionsToRun()

        iTotalCount = tblRevsToApply.Count
        iPosition = 1

        'now tblRevsToApply should hold all the revisions that should be applied for all of the databases
        For Each oRow In tblRevsToApply.Select(filterExpression:=Nothing, sort:=tblRevsToApply.SequenceToRunColumn.ColumnName)
            sCurrentDatabase = oRow.DBName
            lDBRev = oRow.Revision_ID
            'get the DB for this revision
            db = GetDatabaseConnection(sDBName:=oRow.DBName)
            sSQL = oRow.SQL_Statement

            Dim lLastRevRun As Long = CType(oDict.Item(sCurrentDatabase.ToUpper), Long)
            'this check has to occur after the ssql length check, because if for some reason a revision for database x exists in the master file, but this version of database x script file is old and does not include that version we don't want the revision id to be compared.  the fact that the revision does not exist in the dbscript file means just skip it
            If (lLastRevRun + 1) <> lDBRev Then 'if this revision is not the next in line, then we have a problem
                Throw New ApplicationException("DB Script Revision '" & lDBRev.ToString & "' is not the next sequential revision for '" & sCurrentDatabase & "'!  We expect the next revision to be '" + (lLastRevRun + 1).ToString & "'")
            End If

            If HasRevisionBeenRunBefore(db:=db, lRev:=lDBRev) = True Then
                Throw New ApplicationException("DB Script Revision '" & lDBRev.ToString & "' already exists in the script revision log of '" & sCurrentDatabase & "'!")
            End If

            'moSplash.UpdateStatusLabel("Updating " & sCurrentDatabase & " to Revision ID '" & lDBRev & "' (" & iMasterRevision & " of " & lTotalCount & ")")
            moSplash.UpdateMainStatusLabel("Total Progress " & iPosition.ToString() & " of " & iTotalCount & ")")
            moSplash.UpdateStatusLabel("Updating database " & sCurrentDatabase & " to Revision ID '" & lDBRev & "' Rev.Date: " + oRow.RevisionDate.ToString())
            moSplash.RefreshLabels()

            bSuccessful = False

            'moSMOServer.BeginTransaction()
            moSMOServer.ConnectionContext.BeginTransaction()

            On Error GoTo EH_NeedRollback
            db.ExecuteNonQuery(sSQL)
            'the bSuccessful flag is here for ONE REASON ONLY.... a hack
            '   when using SQLDMO begin, commit and rollback for transactions you can ONLY call rollback if at least one transaction completed succesful...
            '   so, if for example, the first transaction in the batch fails, if you try to call rollback another error will be raised... so we use this flag
            '   to determine whether or not we can call rollback in our error handler
            bSuccessful = True
            'write our new Revision to the database so that is the NEXT revision fails, when this until is run again, it can pick up were it left off and not apply the same patch twice
            db.ExecuteNonQuery("UPDATE [Versioning].Settings set CurrentRevision=" & lDBRev.ToString())
            On Error GoTo EH_Log
            Const LOG_SCRIPT As String = "INSERT INTO [Versioning].[ScriptLog]([RevisionNumber], [DateRun], [Machine], [DeveloperWhoRan], [SQLScript])" _
                                            & "Select {0} as RevisionNumber,'{1}' as DateRun,'{2}' as Machine,'{3}' as DeveloperWhoRan,'{4}' as SQLScript"
            sLogSQL = String.Format(LOG_SCRIPT,
                                   lDBRev.ToString(),
                                   Now.ToString(),
                                   SQLSafeString(sMachine),
                                   SQLSafeString(sDeveloper),
                                   SQLSafeString(sSQL)
                                   )
            db.ExecuteNonQuery(sLogSQL)

            On Error GoTo EH_NeedRollback
            moSMOServer.ConnectionContext.CommitTransaction()
            On Error GoTo EH_Generic
            'update the dictionary with the new "latestrev" so that we will know what the last rev for each db is.  we use it to make sure revisions are sequential without skipping steps
            oDict.Item(sCurrentDatabase.ToUpper) = lDBRev
SKIP:
            iPosition += 1
        Next oRow

        'now reset the list of dbs, because total number of revisions has possibly changed for each database.
        mod_Common.LoadDBList()

        ApplyRevisions = True
        Exit Function

EH_Generic:
        mod_Common.AddGenericError("Unexpected Error in 'ApplyBuildRevisions': " & Err.Number & " - " & Err.Description, "cUpdateMultipleDBs_ApplyRevisions")
        mod_Common.AddGenericError(Err.GetException())

        ApplyRevisions = False
        Exit Function
EH_Log:
        If bSuccessful Then
            moSMOServer.ConnectionContext.RollBackTransaction()
        End If
        mod_Common.AddGenericError("Error Writing Log Entry: " & Err.Number & " - " & Err.Description & vbCrLf & "SQL:" & vbCrLf & sLogSQL, "cUpdateMultipleDBs_ApplyRevisions")
        mod_Common.AddGenericError(Err.GetException())
        Return False
EH_NeedRollback:
        'see comments above where we set bSuccessful to TRUE for more information about why this flag is used

        Dim oDSErrors As dsErrorInfo

        If bSuccessful Then
            moSMOServer.ConnectionContext.RollBackTransaction()
        End If

        oDSErrors = mod_Common.GetDSError

        Dim odsRow As dsErrorInfo.RevisionErrorsRow

        odsRow = oDSErrors.RevisionErrors.NewRevisionErrorsRow

        SetError(odsRow:=odsRow)

        odsRow.Database = db.Name
        odsRow.RevisionNumber = CInt(lDBRev)
        If oRow IsNot Nothing Then
            odsRow.SqlString = oRow.SQL_Statement
            odsRow.Developer_Comments = oRow.Developer_Comments
            odsRow.Developer_Name = oRow.Developer_Name
            odsRow.RevisionDate = oRow.RevisionDate
        End If

        oDSErrors.RevisionErrors.AddRevisionErrorsRow(odsRow)

        'mod_Common.OutputError() don't do this here.  Do it when we return
        ApplyRevisions = False

    End Function

    Private Sub SetError(ByVal odsRow As dsErrorInfo.RevisionErrorsRow)
        'the purpose here is to take the error object, try to get the exception for it, then find the innermost exception.  The inner most exception generally contains the most detailed info about the error
        Try
            Dim innerEx As Exception
            innerEx = Err.GetException()
            Do Until innerEx.InnerException Is Nothing
                innerEx = innerEx.InnerException
            Loop

            If innerEx Is Nothing Then
                odsRow.ErrorMessage = Err.Description
            Else
                odsRow.ErrorMessage = innerEx.Message
            End If
        Catch ex As Exception
            odsRow.ErrorMessage = Err.Description
        End Try
    End Sub

    Private Function SQLSafeString(ByVal s As String) As String
        Return Replace(s, "'", "''")
    End Function

    Private Function HasRevisionBeenRunBefore(ByVal db As Database, ByVal lRev As Long) As Boolean
        Dim oResult As DataSet
        oResult = db.ExecuteWithResults("select RevisionNumber from [Versioning].ScriptLog where RevisionNumber=" & lRev.ToString)

        If oResult.Tables.Count > 0 Then
            If oResult.Tables(0).Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    'checks the script log table to determine if this revision is the next one be run
    Private Function RevisionIsNextInLine(ByVal db As Database, ByVal lRev As Long) As Boolean
        Dim oResult As DataSet
        oResult = db.ExecuteWithResults("select coalesce(max(RevisionNumber),0) from [Versioning].ScriptLog")

        If oResult.Tables.Count > 0 Then
            If oResult.Tables(0).Rows.Count = 0 Then
                Return False
            Else
                Dim iMaxRevNumber As Integer
                iMaxRevNumber = CType(oResult.Tables(0).Rows(0).Item(0), Integer)

                If lRev = (iMaxRevNumber + 1) Then
                    Return True
                Else
                    Return False
                End If
            End If
        Else
            Return False
        End If
    End Function

    Private Function DatabaseScriptFileExits(ByVal sDBName As String) As Boolean
        Dim sFileName As String

        sFileName = mod_Common.GetXMLFileName(sDBName)

        Return IO.File.Exists(sFileName)
    End Function

    ''' <summary>
    ''' for the specified database, gets all the revisions that are greater than or equal to lMinDBRev and puts them into tbl
    ''' </summary>
    ''' <param name="sDBName">The database to get revisions for</param>
    ''' <param name="lMinDBRev">the minimum revision number that should be included</param>
    ''' <param name="ds">the table that the rows should be added to</param>
    ''' <remarks></remarks>
    Private Sub GetRevisions(ByVal sDBName As String, ByVal lMinDBRev As Long, ByVal ds As dsRevisions)
        Dim oDS As New dsRevisions
        Dim sFileName As String
        Dim iSequence As Integer
        sFileName = mod_Common.GetXMLFileName(sDBName)

        Dim fileInfo As System.IO.FileInfo = New IO.FileInfo(fileName:=sFileName)
        'load the xml if the file exists and it has data (if the length is 0 the file exists but is blank, which is fine, no errors fro having a blank file, we simply just do not load any content.  if we attempted to read a blank file we would get an error saying root element did not exist)
        If fileInfo.Exists AndAlso fileInfo.Length > 0 Then
            oDS.ReadXml(sFileName)
            iSequence = 1
            'take all the revision rows and put them into the "applyRevision" table
            '  note, the only difference is that table has "DBName" so we can keep track of which revisions belong to which DB
            'our SORT is by REVISION, not by date, REVISION is more important than date
            'NOTE: the sequence here is probablynot even going to be used, once all revs from all DB's are piled into one big group, sequence will be reevaluated
            For Each row As dsRevisions.RevisionsRow In CType(oDS.Revisions.Select(filterExpression:=oDS.Revisions.Revision_IDColumn.ColumnName + ">=" + lMinDBRev.ToString(), sort:=oDS.Revisions.Revision_IDColumn.ColumnName), dsRevisions.RevisionsRow())
                ds.ApplyRevision.AddApplyRevisionRow(SequenceToRun:=iSequence, DBName:=sDBName, Revision_ID:=row.Revision_ID, Developer_Comments:=row.Developer_Comments, Developer_Name:=row.Developer_Name, RevisionDate:=row.RevisionDate, SQL_Statement:=row.SQL_Statement)
                iSequence += 1
            Next
        End If
        If Not oDS Is Nothing Then
            oDS.Dispose()
            oDS = Nothing
        End If
    End Sub


    Private Sub CreatePropertiesAndDependents(ByVal db As Database)
        Dim thisAss As System.Reflection.Assembly
        thisAss = System.Reflection.Assembly.GetExecutingAssembly()
        Dim oStream As System.IO.Stream = thisAss.GetManifestResourceStream(thisAss.GetName().Name & ".VersioningObjectsScript.txt")
        Dim sr As New System.IO.StreamReader(oStream)
        Dim sSQL As String = sr.ReadToEnd
        sr.Close()
        sr = Nothing
        oStream.Close()
        oStream = Nothing

        db.ExecuteNonQuery(sSQL)

    End Sub

    Friend Function GetCurrentDBRevisionID(ByVal db As Database, ByVal sDBName As String) As Integer

        If Not db Is Nothing Then
            Dim oResult As DataSet
            Try
                oResult = db.ExecuteWithResults("select CurrentRevision from " & sDBName & ".Versioning.Settings")
                If oResult.Tables.Count > 0 Then
                    If oResult.Tables(0).Rows.Count = 0 Then
                        Return 0
                    Else
                        Return CInt(oResult.Tables(0).Rows(0).Item(0))
                    End If
                Else
                End If
            Catch ex As Exception
                'exception would occur if the settings table does not exist
                Return CInt(0)
            End Try
        End If

        Return 0
    End Function

    Friend Sub Closedown()

        If lstDBConnection IsNot Nothing Then
            lstDBConnection.Clear()
            lstDBConnection = Nothing
        End If
        If moSMOServer IsNot Nothing Then
            moSMOServer.ConnectionContext.Disconnect()
            moSMOServer = Nothing
        End If
    End Sub

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
            End If

            Closedown()
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
