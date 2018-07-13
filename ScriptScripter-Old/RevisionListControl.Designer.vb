<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RevisionListControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.grdItems = New System.Windows.Forms.DataGridView
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.chkFull = New System.Windows.Forms.CheckBox
        Me.lblTrimLegend = New System.Windows.Forms.Label
        CType(Me.grdItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'grdItems
        '
        Me.grdItems.AllowUserToAddRows = False
        Me.grdItems.AllowUserToDeleteRows = False
        Me.grdItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdItems.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grdItems.Location = New System.Drawing.Point(0, 32)
        Me.grdItems.Name = "grdItems"
        Me.grdItems.ReadOnly = True
        Me.grdItems.Size = New System.Drawing.Size(790, 267)
        Me.grdItems.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lblTrimLegend)
        Me.Panel1.Controls.Add(Me.chkFull)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(790, 32)
        Me.Panel1.TabIndex = 1
        '
        'chkFull
        '
        Me.chkFull.AutoSize = True
        Me.chkFull.Location = New System.Drawing.Point(3, 8)
        Me.chkFull.Name = "chkFull"
        Me.chkFull.Size = New System.Drawing.Size(443, 17)
        Me.chkFull.TabIndex = 0
        Me.chkFull.Text = "Include full SQL String (may cause UI performance issues for very large SQL State" & _
            "ments)"
        Me.chkFull.UseVisualStyleBackColor = True
        '
        'lblTrimLegend
        '
        Me.lblTrimLegend.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblTrimLegend.BackColor = System.Drawing.Color.LightBlue
        Me.lblTrimLegend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblTrimLegend.Location = New System.Drawing.Point(539, 7)
        Me.lblTrimLegend.Name = "lblTrimLegend"
        Me.lblTrimLegend.Size = New System.Drawing.Size(248, 18)
        Me.lblTrimLegend.TabIndex = 1
        Me.lblTrimLegend.Text = "Indicates SQL was trimmed to 1000 chars"
        Me.lblTrimLegend.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RevisionListControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.grdItems)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "RevisionListControl"
        Me.Size = New System.Drawing.Size(790, 299)
        CType(Me.grdItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents grdItems As System.Windows.Forms.DataGridView
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents chkFull As System.Windows.Forms.CheckBox
    Friend WithEvents lblTrimLegend As System.Windows.Forms.Label

End Class
