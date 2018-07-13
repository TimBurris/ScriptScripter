<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmShowRevisions
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.RevisionListControl1 = New ScriptScripter.RevisionListControl
        Me.SuspendLayout()
        '
        'RevisionListControl1
        '
        Me.RevisionListControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RevisionListControl1.Location = New System.Drawing.Point(0, 0)
        Me.RevisionListControl1.Name = "RevisionListControl1"
        Me.RevisionListControl1.Size = New System.Drawing.Size(1192, 570)
        Me.RevisionListControl1.TabIndex = 0
        '
        'frmShowRevisions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1192, 570)
        Me.Controls.Add(Me.RevisionListControl1)
        Me.Name = "frmShowRevisions"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Revisions"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RevisionListControl1 As ScriptScripter.RevisionListControl
End Class
