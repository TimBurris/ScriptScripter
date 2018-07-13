Public Class frmShowRevisions

    Public Sub LoadData(ByVal tbl As dsRevisions.ApplyRevisionDataTable)
        RevisionListControl1.LoadData(tbl)
    End Sub

    'Public Sub LoadData(ByVal DBName As String, ByVal rows() As dsRevisions.RevisionsRow)
    '    RevisionListControl1.LoadData(DBName, rows)
    'End Sub
    Public Shared Sub ShowRevisionsToRun(Optional ByVal owner As IWin32Window = Nothing)
        Using oDBUpdater As cUpdateMultipleDBs = New cUpdateMultipleDBs()
            Using frm As New frmShowRevisions()
                Using tbl As dsRevisions.ApplyRevisionDataTable = oDBUpdater.GetAllRevisionsToRun()
                    frm.LoadData(tbl:=tbl)
                    frm.ShowDialog(owner)
                End Using
            End Using
        End Using
    End Sub
End Class