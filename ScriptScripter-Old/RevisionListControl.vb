Public Class RevisionListControl
    Private Const COLUMNNAME_TRIMMEDSQL As String = "TrimmedSQL"
    Private COLUMNNAME_FULLSQL As String 'this will be set in load data

    Public Sub LoadData(ByVal tbl As dsRevisions.ApplyRevisionDataTable)
        COLUMNNAME_FULLSQL = tbl.SQL_StatementColumn.ColumnName

        tbl.Columns.Add(columnName:=COLUMNNAME_TRIMMEDSQL, type:=GetType(String), expression:="SUBSTRING(" + COLUMNNAME_FULLSQL + ",1,1000)")

        Dim sSortString As String = tbl.SequenceToRunColumn.ColumnName
        tbl.DefaultView.Sort = sSortString
        grdItems.DataSource = tbl

        Dim iPos As Integer = 0

        With grdItems.Columns.Item(columnName:=tbl.SequenceToRunColumn.ColumnName)
            .DisplayIndex = iPos
            .HeaderText = "Order"
            .Width = 45
        End With
        iPos += 1

        With grdItems.Columns.Item(columnName:=tbl.DBNameColumn.ColumnName)
            .DisplayIndex = iPos
            .Width = 100
        End With
        iPos += 1

        With grdItems.Columns.Item(columnName:=tbl.Revision_IDColumn.ColumnName)
            .DisplayIndex = iPos
            .HeaderText = "Rev.ID"
            .Width = 60
        End With
        iPos += 1

        With grdItems.Columns.Item(columnName:=tbl.RevisionDateColumn.ColumnName)
            .DisplayIndex = iPos
            .Width = 120
        End With
        iPos += 1

        With grdItems.Columns.Item(columnName:=tbl.Developer_NameColumn.ColumnName)
            .DisplayIndex = iPos
            .Width = 100
        End With
        iPos += 1

        With grdItems.Columns.Item(columnName:=tbl.Developer_CommentsColumn.ColumnName)
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .DisplayIndex = iPos
            .Width = 180
        End With
        iPos += 1

        With grdItems.Columns.Item(columnName:=COLUMNNAME_TRIMMEDSQL)
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .DisplayIndex = iPos
            .HeaderText = "SQL"
            .Width = 300
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        End With

        With grdItems.Columns.Item(columnName:=tbl.SQL_StatementColumn.ColumnName)
            .Visible = False
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .DisplayIndex = iPos
            .HeaderText = "SQL"
            .Width = 300
        End With
        iPos += 1

        SetFullVersusTrimSQL()
    End Sub

    Private Sub chkFull_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkFull.CheckedChanged
        SetFullVersusTrimSQL()
    End Sub

    Private Sub SetFullVersusTrimSQL()
        If chkFull.Checked = True Then
            grdItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            grdItems.Columns.Item(columnName:=COLUMNNAME_FULLSQL).Visible = True
            grdItems.Columns.Item(columnName:=COLUMNNAME_TRIMMEDSQL).Visible = False
        Else
            grdItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            grdItems.Columns.Item(columnName:=COLUMNNAME_FULLSQL).Visible = False
            grdItems.Columns.Item(columnName:=COLUMNNAME_TRIMMEDSQL).Visible = True
        End If
    End Sub

    Private Sub grdItems_DataBindingComplete(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewBindingCompleteEventArgs) Handles grdItems.DataBindingComplete
        For Each rw As DataGridViewRow In grdItems.Rows
            Dim fcell = rw.Cells(columnName:=COLUMNNAME_FULLSQL)
            Dim tcell = rw.Cells(columnName:=COLUMNNAME_TRIMMEDSQL)

            If fcell.Value.Equals(tcell.Value) Then
            Else
                tcell.Style.BackColor = Color.LightBlue
            End If
        Next
    End Sub
End Class

