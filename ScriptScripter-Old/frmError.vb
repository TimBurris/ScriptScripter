Public Class frmError
    Inherits System.Windows.Forms.Form

    Private modsError As dsErrorInfo
#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Splitter1 As System.Windows.Forms.Splitter
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label
        Me.TreeView1 = New System.Windows.Forms.TreeView
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.Splitter1 = New System.Windows.Forms.Splitter
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(1053, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Errors were encountered in the update process."
        '
        'TreeView1
        '
        Me.TreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeView1.Location = New System.Drawing.Point(0, 23)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.Size = New System.Drawing.Size(1053, 335)
        Me.TreeView1.TabIndex = 4
        '
        'TextBox1
        '
        Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TextBox1.Location = New System.Drawing.Point(0, 364)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBox1.Size = New System.Drawing.Size(1053, 144)
        Me.TextBox1.TabIndex = 5
        '
        'Splitter1
        '
        Me.Splitter1.BackColor = System.Drawing.Color.Yellow
        Me.Splitter1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Splitter1.Location = New System.Drawing.Point(0, 358)
        Me.Splitter1.Name = "Splitter1"
        Me.Splitter1.Size = New System.Drawing.Size(1053, 6)
        Me.Splitter1.TabIndex = 6
        Me.Splitter1.TabStop = False
        '
        'frmError
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(1053, 508)
        Me.Controls.Add(Me.TreeView1)
        Me.Controls.Add(Me.Splitter1)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmError"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Error updating database"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Friend WriteOnly Property ErrorDataSet() As dsErrorInfo
        Set(ByVal Value As dsErrorInfo)
            modsError = Value
        End Set
    End Property

    Private Sub frmError_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If modsError Is Nothing Then
            Dim oRow As dsErrorInfo.GenericErrorsRow
            modsError = New dsErrorInfo
            oRow = modsError.GenericErrors.NewGenericErrorsRow
            oRow.ErrorText = "Unspecified error encountered updating databases.  " 'Not keeping db name modularly anymore.  Possibly in database " & mod_Common.DB_Name & "."
            modsError.GenericErrors.AddGenericErrorsRow(oRow)
        End If

        Dim oTable As DataTable

        If modsError.RevisionErrors.Count > 0 Then
            oTable = modsError.RevisionErrors
        Else
            oTable = modsError.GenericErrors
        End If

        ShowAsListBox(oTable)

    End Sub

    Private Sub ShowAsListBox(ByVal oTable As DataTable)
        Dim oRow As DataRow
        Dim i As Integer
        Dim oNode As TreeNode
        Dim oChild As TreeNode

        For Each oRow In oTable.Rows
            For i = 0 To oRow.ItemArray.Length - 1
                oNode = New TreeNode
                oNode.Text = oTable.Columns.Item(i).ColumnName
                oChild = New TreeNode
                oChild.Text = CType(oRow.Item(i), String)
                oNode.Nodes.Add(oChild)
                TreeView1.Nodes.Add(oNode)
            Next i
        Next oRow
        TreeView1.ExpandAll()
    End Sub

    Private Sub TreeView1_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView1.AfterSelect

        Dim oNode As TreeNode
        oNode = TreeView1.SelectedNode()

        If Not oNode Is Nothing Then
            If oNode.Parent Is Nothing Then
                If Not oNode.Nodes(0) Is Nothing Then
                    Try
                        TextBox1.Text = oNode.Nodes(0).Text
                    Catch
                        TextBox1.Text = "Error cannot be displayed.  SQL statement may be too long."
                    End Try
                Else
                    TextBox1.Text = ""
                End If
            Else
                Try
                    TextBox1.Text = oNode.Text
                Catch
                    TextBox1.Text = Strings.Left(oNode.Text, 30000)
                End Try
            End If
        End If
    End Sub
End Class
