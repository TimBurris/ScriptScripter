<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmConnect
#Region "Windows Form Designer generated code "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'This call is required by the Windows Form Designer.
		InitializeComponent()
	End Sub
	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
	Public WithEvents chkSave As System.Windows.Forms.CheckBox
	Public WithEvents cmdLoadAvail As System.Windows.Forms.Button
	Public WithEvents chkSecure As System.Windows.Forms.CheckBox
	Public WithEvents cmdExit As System.Windows.Forms.Button
	Public WithEvents cmdConnect As System.Windows.Forms.Button
	Public WithEvents txtPass As System.Windows.Forms.TextBox
	Public WithEvents txtLogin As System.Windows.Forms.TextBox
	Public WithEvents cboServer As System.Windows.Forms.ComboBox
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.chkSave = New System.Windows.Forms.CheckBox()
        Me.cmdLoadAvail = New System.Windows.Forms.Button()
        Me.chkSecure = New System.Windows.Forms.CheckBox()
        Me.cmdExit = New System.Windows.Forms.Button()
        Me.cmdConnect = New System.Windows.Forms.Button()
        Me.txtPass = New System.Windows.Forms.TextBox()
        Me.txtLogin = New System.Windows.Forms.TextBox()
        Me.cboServer = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmdTest = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'chkSave
        '
        Me.chkSave.BackColor = System.Drawing.SystemColors.Control
        Me.chkSave.Checked = True
        Me.chkSave.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSave.Cursor = System.Windows.Forms.Cursors.Default
        Me.chkSave.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSave.ForeColor = System.Drawing.SystemColors.ControlText
        Me.chkSave.Location = New System.Drawing.Point(4, 212)
        Me.chkSave.Name = "chkSave"
        Me.chkSave.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.chkSave.Size = New System.Drawing.Size(135, 16)
        Me.chkSave.TabIndex = 10
        Me.chkSave.Text = "Save These Settings"
        Me.chkSave.UseVisualStyleBackColor = False
        '
        'cmdLoadAvail
        '
        Me.cmdLoadAvail.BackColor = System.Drawing.SystemColors.Control
        Me.cmdLoadAvail.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdLoadAvail.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdLoadAvail.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdLoadAvail.Location = New System.Drawing.Point(14, 32)
        Me.cmdLoadAvail.Name = "cmdLoadAvail"
        Me.cmdLoadAvail.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdLoadAvail.Size = New System.Drawing.Size(209, 23)
        Me.cmdLoadAvail.TabIndex = 1
        Me.cmdLoadAvail.Text = "Load Combo with all Available Serves"
        Me.cmdLoadAvail.UseVisualStyleBackColor = False
        '
        'chkSecure
        '
        Me.chkSecure.BackColor = System.Drawing.SystemColors.Control
        Me.chkSecure.Cursor = System.Windows.Forms.Cursors.Default
        Me.chkSecure.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSecure.ForeColor = System.Drawing.SystemColors.ControlText
        Me.chkSecure.Location = New System.Drawing.Point(14, 60)
        Me.chkSecure.Name = "chkSecure"
        Me.chkSecure.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.chkSecure.Size = New System.Drawing.Size(177, 18)
        Me.chkSecure.TabIndex = 2
        Me.chkSecure.Text = "Use Trusted Connection"
        Me.chkSecure.UseVisualStyleBackColor = False
        '
        'cmdExit
        '
        Me.cmdExit.BackColor = System.Drawing.SystemColors.Control
        Me.cmdExit.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdExit.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdExit.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdExit.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdExit.Location = New System.Drawing.Point(133, 184)
        Me.cmdExit.Name = "cmdExit"
        Me.cmdExit.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdExit.Size = New System.Drawing.Size(69, 23)
        Me.cmdExit.TabIndex = 9
        Me.cmdExit.Text = "Cancel"
        Me.cmdExit.UseVisualStyleBackColor = False
        '
        'cmdConnect
        '
        Me.cmdConnect.BackColor = System.Drawing.SystemColors.Control
        Me.cmdConnect.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdConnect.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdConnect.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdConnect.Location = New System.Drawing.Point(39, 184)
        Me.cmdConnect.Name = "cmdConnect"
        Me.cmdConnect.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdConnect.Size = New System.Drawing.Size(69, 23)
        Me.cmdConnect.TabIndex = 8
        Me.cmdConnect.Text = "Ok"
        Me.cmdConnect.UseVisualStyleBackColor = False
        '
        'txtPass
        '
        Me.txtPass.AcceptsReturn = True
        Me.txtPass.BackColor = System.Drawing.SystemColors.Window
        Me.txtPass.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtPass.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPass.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtPass.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtPass.Location = New System.Drawing.Point(74, 124)
        Me.txtPass.MaxLength = 0
        Me.txtPass.Name = "txtPass"
        Me.txtPass.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPass.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtPass.Size = New System.Drawing.Size(149, 20)
        Me.txtPass.TabIndex = 6
        '
        'txtLogin
        '
        Me.txtLogin.AcceptsReturn = True
        Me.txtLogin.BackColor = System.Drawing.SystemColors.Window
        Me.txtLogin.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtLogin.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLogin.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtLogin.Location = New System.Drawing.Point(74, 96)
        Me.txtLogin.MaxLength = 0
        Me.txtLogin.Name = "txtLogin"
        Me.txtLogin.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtLogin.Size = New System.Drawing.Size(149, 20)
        Me.txtLogin.TabIndex = 4
        Me.txtLogin.Text = "sa"
        '
        'cboServer
        '
        Me.cboServer.BackColor = System.Drawing.SystemColors.Window
        Me.cboServer.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboServer.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboServer.ForeColor = System.Drawing.SystemColors.WindowText
        Me.cboServer.Location = New System.Drawing.Point(12, 6)
        Me.cboServer.Name = "cboServer"
        Me.cboServer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cboServer.Size = New System.Drawing.Size(213, 22)
        Me.cboServer.TabIndex = 0
        Me.cboServer.Text = "(local)"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(8, 99)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(63, 14)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "User Name:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 127)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(60, 14)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Password:"
        '
        'cmdTest
        '
        Me.cmdTest.BackColor = System.Drawing.SystemColors.Control
        Me.cmdTest.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdTest.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdTest.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdTest.Location = New System.Drawing.Point(103, 150)
        Me.cmdTest.Name = "cmdTest"
        Me.cmdTest.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdTest.Size = New System.Drawing.Size(120, 21)
        Me.cmdTest.TabIndex = 7
        Me.cmdTest.Text = "Test Connection"
        Me.cmdTest.UseVisualStyleBackColor = False
        '
        'frmConnect
        '
        Me.AcceptButton = Me.cmdConnect
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.CancelButton = Me.cmdExit
        Me.ClientSize = New System.Drawing.Size(241, 233)
        Me.Controls.Add(Me.cmdTest)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.chkSave)
        Me.Controls.Add(Me.cmdLoadAvail)
        Me.Controls.Add(Me.chkSecure)
        Me.Controls.Add(Me.cmdExit)
        Me.Controls.Add(Me.cmdConnect)
        Me.Controls.Add(Me.txtPass)
        Me.Controls.Add(Me.txtLogin)
        Me.Controls.Add(Me.cboServer)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Location = New System.Drawing.Point(356, 266)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmConnect"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Select Server..."
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents cmdTest As System.Windows.Forms.Button
#End Region 
End Class