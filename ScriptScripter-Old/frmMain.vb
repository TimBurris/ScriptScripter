Option Strict On
Option Explicit On
Friend Class frmMain
    Inherits System.Windows.Forms.Form
#Region "Windows Form Designer generated code "
    Public Sub New()
        MyBase.New()
        'If m_vb6FormDefInstance Is Nothing Then
        '	If m_InitializingDefInstance Then
        '		m_vb6FormDefInstance = Me
        '	Else
        '		Try 
        '			'For the start-up form, the first instance created is the default instance.
        '			If System.Reflection.Assembly.GetExecutingAssembly.EntryPoint.DeclaringType Is Me.GetType Then
        '				m_vb6FormDefInstance = Me
        '			End If
        '		Catch
        '		End Try
        '	End If
        'End If
        'This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub
    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
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
    Public WithEvents cmdApply As System.Windows.Forms.Button
    Public WithEvents chkBoring As System.Windows.Forms.CheckBox
    Public WithEvents txtDeveloper As System.Windows.Forms.TextBox
    Public WithEvents cmdCancel As System.Windows.Forms.Button
    Public WithEvents cmdCommit As System.Windows.Forms.Button
    Public WithEvents txtComments As System.Windows.Forms.TextBox
    Public WithEvents txtSQL As System.Windows.Forms.TextBox
    Public WithEvents lblBoring As System.Windows.Forms.Label
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    Public WithEvents Label03 As System.Windows.Forms.Label
    Public WithEvents Label01 As System.Windows.Forms.Label
    Public WithEvents Label04 As System.Windows.Forms.Label
    Public WithEvents Label02 As System.Windows.Forms.Label
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents lblCurrentDB As System.Windows.Forms.Label
    Friend WithEvents lstfrmServerInfo As System.Windows.Forms.ListBox
    Friend WithEvents lblServer As System.Windows.Forms.Label
    Friend WithEvents cmdServerInfo As System.Windows.Forms.Button
    Friend WithEvents ChkRetainSQL As System.Windows.Forms.CheckBox
    Friend WithEvents chkRetainComment As System.Windows.Forms.CheckBox
    Friend WithEvents cmdShowRevisions As System.Windows.Forms.Button
    Friend WithEvents cmdChangeConnection As System.Windows.Forms.Button
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmdApply = New System.Windows.Forms.Button()
        Me.chkBoring = New System.Windows.Forms.CheckBox()
        Me.txtDeveloper = New System.Windows.Forms.TextBox()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdCommit = New System.Windows.Forms.Button()
        Me.txtComments = New System.Windows.Forms.TextBox()
        Me.txtSQL = New System.Windows.Forms.TextBox()
        Me.lblBoring = New System.Windows.Forms.Label()
        Me.Label03 = New System.Windows.Forms.Label()
        Me.Label01 = New System.Windows.Forms.Label()
        Me.Label04 = New System.Windows.Forms.Label()
        Me.Label02 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.lblCurrentDB = New System.Windows.Forms.Label()
        Me.lstfrmServerInfo = New System.Windows.Forms.ListBox()
        Me.lblServer = New System.Windows.Forms.Label()
        Me.cmdServerInfo = New System.Windows.Forms.Button()
        Me.ChkRetainSQL = New System.Windows.Forms.CheckBox()
        Me.chkRetainComment = New System.Windows.Forms.CheckBox()
        Me.cmdShowRevisions = New System.Windows.Forms.Button()
        Me.cmdChangeConnection = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmdApply
        '
        Me.cmdApply.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdApply.BackColor = System.Drawing.SystemColors.Control
        Me.cmdApply.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdApply.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdApply.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdApply.Location = New System.Drawing.Point(999, 588)
        Me.cmdApply.Name = "cmdApply"
        Me.cmdApply.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdApply.Size = New System.Drawing.Size(88, 24)
        Me.cmdApply.TabIndex = 13
        Me.cmdApply.Text = "Apply Patches"
        Me.cmdApply.UseVisualStyleBackColor = False
        '
        'chkBoring
        '
        Me.chkBoring.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkBoring.BackColor = System.Drawing.SystemColors.Control
        Me.chkBoring.Cursor = System.Windows.Forms.Cursors.Default
        Me.chkBoring.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkBoring.ForeColor = System.Drawing.SystemColors.ControlText
        Me.chkBoring.Location = New System.Drawing.Point(134, 587)
        Me.chkBoring.Name = "chkBoring"
        Me.chkBoring.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.chkBoring.Size = New System.Drawing.Size(13, 13)
        Me.chkBoring.TabIndex = 11
        Me.chkBoring.UseVisualStyleBackColor = False
        '
        'txtDeveloper
        '
        Me.txtDeveloper.AcceptsReturn = True
        Me.txtDeveloper.BackColor = System.Drawing.SystemColors.Window
        Me.txtDeveloper.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtDeveloper.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDeveloper.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtDeveloper.Location = New System.Drawing.Point(145, 12)
        Me.txtDeveloper.MaxLength = 0
        Me.txtDeveloper.Name = "txtDeveloper"
        Me.txtDeveloper.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtDeveloper.Size = New System.Drawing.Size(232, 20)
        Me.txtDeveloper.TabIndex = 0
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.cmdCancel.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdCancel.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCancel.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdCancel.Location = New System.Drawing.Point(870, 588)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdCancel.Size = New System.Drawing.Size(84, 26)
        Me.cmdCancel.TabIndex = 7
        Me.cmdCancel.Text = "Exit"
        Me.cmdCancel.UseVisualStyleBackColor = False
        '
        'cmdCommit
        '
        Me.cmdCommit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCommit.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.cmdCommit.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdCommit.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCommit.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdCommit.Location = New System.Drawing.Point(743, 588)
        Me.cmdCommit.Name = "cmdCommit"
        Me.cmdCommit.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdCommit.Size = New System.Drawing.Size(82, 26)
        Me.cmdCommit.TabIndex = 6
        Me.cmdCommit.Text = "Commit"
        Me.cmdCommit.UseVisualStyleBackColor = False
        '
        'txtComments
        '
        Me.txtComments.AcceptsReturn = True
        Me.txtComments.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtComments.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.txtComments.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtComments.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtComments.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtComments.Location = New System.Drawing.Point(8, 463)
        Me.txtComments.MaxLength = 0
        Me.txtComments.Multiline = True
        Me.txtComments.Name = "txtComments"
        Me.txtComments.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtComments.Size = New System.Drawing.Size(1076, 117)
        Me.txtComments.TabIndex = 4
        Me.txtComments.WordWrap = False
        '
        'txtSQL
        '
        Me.txtSQL.AcceptsReturn = True
        Me.txtSQL.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSQL.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.txtSQL.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtSQL.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSQL.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtSQL.Location = New System.Drawing.Point(8, 120)
        Me.txtSQL.MaxLength = 0
        Me.txtSQL.Multiline = True
        Me.txtSQL.Name = "txtSQL"
        Me.txtSQL.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtSQL.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtSQL.Size = New System.Drawing.Size(1076, 287)
        Me.txtSQL.TabIndex = 3
        Me.txtSQL.WordWrap = False
        '
        'lblBoring
        '
        Me.lblBoring.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblBoring.BackColor = System.Drawing.Color.Transparent
        Me.lblBoring.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblBoring.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBoring.ForeColor = System.Drawing.Color.Red
        Me.lblBoring.Location = New System.Drawing.Point(152, 583)
        Me.lblBoring.Name = "lblBoring"
        Me.lblBoring.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblBoring.Size = New System.Drawing.Size(127, 19)
        Me.lblBoring.TabIndex = 12
        Me.lblBoring.Text = "I am very boring"
        '
        'Label03
        '
        Me.Label03.AutoSize = True
        Me.Label03.BackColor = System.Drawing.Color.Transparent
        Me.Label03.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label03.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label03.ForeColor = System.Drawing.Color.Red
        Me.Label03.Location = New System.Drawing.Point(8, 14)
        Me.Label03.Name = "Label03"
        Me.Label03.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label03.Size = New System.Drawing.Size(131, 18)
        Me.Label03.TabIndex = 10
        Me.Label03.Text = "Developer Name:"
        '
        'Label01
        '
        Me.Label01.AutoSize = True
        Me.Label01.BackColor = System.Drawing.Color.Transparent
        Me.Label01.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label01.Font = New System.Drawing.Font("Arial", 13.0!, System.Drawing.FontStyle.Bold)
        Me.Label01.ForeColor = System.Drawing.Color.Red
        Me.Label01.Location = New System.Drawing.Point(392, 14)
        Me.Label01.Name = "Label01"
        Me.Label01.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label01.Size = New System.Drawing.Size(92, 21)
        Me.Label01.TabIndex = 9
        Me.Label01.Text = "SERVER:"
        '
        'Label04
        '
        Me.Label04.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label04.AutoSize = True
        Me.Label04.BackColor = System.Drawing.Color.Transparent
        Me.Label04.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label04.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label04.ForeColor = System.Drawing.Color.Red
        Me.Label04.Location = New System.Drawing.Point(8, 441)
        Me.Label04.Name = "Label04"
        Me.Label04.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label04.Size = New System.Drawing.Size(166, 18)
        Me.Label04.TabIndex = 8
        Me.Label04.Text = "Developer Comments:"
        '
        'Label02
        '
        Me.Label02.AutoSize = True
        Me.Label02.BackColor = System.Drawing.Color.Transparent
        Me.Label02.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label02.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label02.ForeColor = System.Drawing.Color.Red
        Me.Label02.Location = New System.Drawing.Point(8, 98)
        Me.Label02.Name = "Label02"
        Me.Label02.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label02.Size = New System.Drawing.Size(137, 18)
        Me.Label02.TabIndex = 2
        Me.Label02.Text = "SQL Statement(s):"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label1.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(8, 48)
        Me.Label1.Name = "Label1"
        Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label1.Size = New System.Drawing.Size(78, 18)
        Me.Label1.TabIndex = 19
        Me.Label1.Text = "Database:"
        '
        'ComboBox1
        '
        Me.ComboBox1.AllowDrop = True
        Me.ComboBox1.BackColor = System.Drawing.SystemColors.Window
        Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox1.Location = New System.Drawing.Point(145, 48)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(232, 22)
        Me.ComboBox1.TabIndex = 20
        '
        'lblCurrentDB
        '
        Me.lblCurrentDB.AutoSize = True
        Me.lblCurrentDB.BackColor = System.Drawing.Color.Transparent
        Me.lblCurrentDB.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblCurrentDB.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrentDB.ForeColor = System.Drawing.Color.Red
        Me.lblCurrentDB.Location = New System.Drawing.Point(392, 50)
        Me.lblCurrentDB.Name = "lblCurrentDB"
        Me.lblCurrentDB.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblCurrentDB.Size = New System.Drawing.Size(57, 18)
        Me.lblCurrentDB.TabIndex = 22
        Me.lblCurrentDB.Text = "Status:"
        '
        'lstfrmServerInfo
        '
        Me.lstfrmServerInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstfrmServerInfo.FormattingEnabled = True
        Me.lstfrmServerInfo.ItemHeight = 14
        Me.lstfrmServerInfo.Location = New System.Drawing.Point(518, 48)
        Me.lstfrmServerInfo.Name = "lstfrmServerInfo"
        Me.lstfrmServerInfo.Size = New System.Drawing.Size(414, 60)
        Me.lstfrmServerInfo.TabIndex = 23
        '
        'lblServer
        '
        Me.lblServer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblServer.BackColor = System.Drawing.Color.Yellow
        Me.lblServer.Font = New System.Drawing.Font("Arial", 15.0!)
        Me.lblServer.Location = New System.Drawing.Point(518, 13)
        Me.lblServer.Name = "lblServer"
        Me.lblServer.Size = New System.Drawing.Size(414, 32)
        Me.lblServer.TabIndex = 24
        '
        'cmdServerInfo
        '
        Me.cmdServerInfo.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdServerInfo.Location = New System.Drawing.Point(1006, 17)
        Me.cmdServerInfo.Name = "cmdServerInfo"
        Me.cmdServerInfo.Size = New System.Drawing.Size(81, 23)
        Me.cmdServerInfo.TabIndex = 25
        Me.cmdServerInfo.Text = "Server Info"
        Me.cmdServerInfo.UseVisualStyleBackColor = True
        '
        'ChkRetainSQL
        '
        Me.ChkRetainSQL.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ChkRetainSQL.AutoSize = True
        Me.ChkRetainSQL.BackColor = System.Drawing.Color.Transparent
        Me.ChkRetainSQL.ForeColor = System.Drawing.Color.Red
        Me.ChkRetainSQL.Location = New System.Drawing.Point(8, 412)
        Me.ChkRetainSQL.Name = "ChkRetainSQL"
        Me.ChkRetainSQL.Size = New System.Drawing.Size(80, 18)
        Me.ChkRetainSQL.TabIndex = 26
        Me.ChkRetainSQL.Text = "Retain SQL"
        Me.ChkRetainSQL.UseVisualStyleBackColor = False
        '
        'chkRetainComment
        '
        Me.chkRetainComment.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkRetainComment.AutoSize = True
        Me.chkRetainComment.BackColor = System.Drawing.Color.Transparent
        Me.chkRetainComment.ForeColor = System.Drawing.Color.Red
        Me.chkRetainComment.Location = New System.Drawing.Point(8, 583)
        Me.chkRetainComment.Name = "chkRetainComment"
        Me.chkRetainComment.Size = New System.Drawing.Size(103, 18)
        Me.chkRetainComment.TabIndex = 27
        Me.chkRetainComment.Text = "Retain Comment"
        Me.chkRetainComment.UseVisualStyleBackColor = False
        '
        'cmdShowRevisions
        '
        Me.cmdShowRevisions.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdShowRevisions.Location = New System.Drawing.Point(938, 48)
        Me.cmdShowRevisions.Name = "cmdShowRevisions"
        Me.cmdShowRevisions.Size = New System.Drawing.Size(146, 24)
        Me.cmdShowRevisions.TabIndex = 28
        Me.cmdShowRevisions.Text = "Show Revisions"
        Me.cmdShowRevisions.UseVisualStyleBackColor = True
        '
        'cmdChangeConnection
        '
        Me.cmdChangeConnection.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdChangeConnection.Location = New System.Drawing.Point(938, 17)
        Me.cmdChangeConnection.Name = "cmdChangeConnection"
        Me.cmdChangeConnection.Size = New System.Drawing.Size(62, 23)
        Me.cmdChangeConnection.TabIndex = 29
        Me.cmdChangeConnection.Text = "Change"
        Me.cmdChangeConnection.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.BackgroundImage = Global.ScriptScripter.My.Resources.Resources.MainBackground
        Me.ClientSize = New System.Drawing.Size(1123, 630)
        Me.Controls.Add(Me.cmdChangeConnection)
        Me.Controls.Add(Me.cmdShowRevisions)
        Me.Controls.Add(Me.chkRetainComment)
        Me.Controls.Add(Me.ChkRetainSQL)
        Me.Controls.Add(Me.cmdServerInfo)
        Me.Controls.Add(Me.lblServer)
        Me.Controls.Add(Me.lstfrmServerInfo)
        Me.Controls.Add(Me.lblCurrentDB)
        Me.Controls.Add(Me.txtDeveloper)
        Me.Controls.Add(Me.txtComments)
        Me.Controls.Add(Me.txtSQL)
        Me.Controls.Add(Me.ComboBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmdApply)
        Me.Controls.Add(Me.chkBoring)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdCommit)
        Me.Controls.Add(Me.lblBoring)
        Me.Controls.Add(Me.Label03)
        Me.Controls.Add(Me.Label01)
        Me.Controls.Add(Me.Label04)
        Me.Controls.Add(Me.Label02)
        Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Location = New System.Drawing.Point(269, 212)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Database Versioning/Scripting"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
#End Region
    Private Const REG_DEVELOPER_LOCATION As String = "Software\ScriptScripter"
    Private Const REG_DEVELOPER_ITEM As String = "DeveloperName"

    'Private miTotalRevisions As Integer
    Private mbSkipIndexChange As Boolean

    Private Sub SetDeveloperText()
        Dim oKey As Microsoft.Win32.RegistryKey

        oKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REG_DEVELOPER_LOCATION, False)

        If oKey Is Nothing Then
            oKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(REG_DEVELOPER_LOCATION)
        End If

        txtDeveloper.Text = CType(oKey.GetValue(REG_DEVELOPER_ITEM), String)    'GetRegStr(REG_DEVELOPER_LOCATION, REG_DEVELOPER_ITEM, Reg.EROOTKEY.HKEY_CURRENT_USER)
    End Sub

    Private Sub SetServerLabel()
        lblServer.Text = mod_Common.CurrentConnectionParameters.Server
    End Sub

    Private Sub chkBoring_CheckStateChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles chkBoring.CheckStateChanged
        ApplyColorScheme()
    End Sub

    Private Sub cmdApply_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdApply.Click
        'launch a confirmation dialog.
        Dim sRevisions As String = ""
        Dim oConfirm As New frmDialogConfirm

        For Each oRow As cdsDBNames.DBListRow In gDSDBs.DBList.Rows
            If oRow.RevisionsToRun > 0 Then
                oConfirm.lstRevisions.Items.Insert(0, oRow.DBName & " has " & oRow.RevisionsToRun & " revisions to run.")
            End If
        Next

        'Find out whether we're dealing with a remote db or a local one.  
        'so if the server we're applying to contains the current computer's name, we're local.
        oConfirm.LocalServer = mod_Common.CurrentConnectionParameters.IsLocalMachineConnection()

        oConfirm.ShowDialog()
        If oConfirm.DialogResult = Windows.Forms.DialogResult.OK Then
            oConfirm.Close()
            oConfirm = Nothing

            'First, we need to get the dataset reloaded.
            'we just dump it here, and when it's reloaded it gets put back in.
            'goMasterDB.Dispose()
            LoadDBList()

            Try
                Using updater As New cUpdateMultipleDBs()
                    If Not updater.PerformUpdates(Me) Then
                        mod_Common.OutputError()
                        'MBox(sErrorMessage, MsgBoxStyle.OKOnly Or MsgBoxStyle.Information, "Error updating database.")
                    End If
                End Using
                RefreshServerInfo()
            Catch ex As Exception
                mod_Common.AddGenericError("Could not update database." & vbCrLf & vbCrLf & ex.Message, "frmMain_cmdApply_Click")
                mod_Common.OutputError()
            End Try
        Else
            MsgBox("no revisions run.")
        End If


    End Sub

    Private Sub cmdCancel_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub

    Private Sub ShowGenericError(ByVal sProcedure As String)
        MsgBox("Unexpected error received in procedure:'" & sProcedure & "'" & vbCrLf & Err.Description)
    End Sub

    Private Function ValidateFields() As Boolean
        Dim controlToFocus As Control
        Dim message As String
        If Len(txtComments.Text) = 0 Then
            controlToFocus = txtComments
            message = "Why would you leave the comments section blank, do you think you are a programmer or something??"
        ElseIf Len(txtSQL.Text) = 0 Then
            controlToFocus = txtSQL
            message = "An SQL Statement would make this work a little better...."
        ElseIf Len(txtDeveloper.Text) = 0 Then
            controlToFocus = txtDeveloper
            message = "Please enter your name"
        ElseIf String.IsNullOrEmpty(CStr(ComboBox1.SelectedValue)) = True Then
            controlToFocus = ComboBox1
            message = "You must first select a database"
        Else
            Return True
        End If

        If controlToFocus IsNot Nothing Then controlToFocus.Focus()
        MessageBox.Show(Me, message, "Invalid Field", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Return False
    End Function

    Private Sub lblBoring_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles lblBoring.Click
        'if they click the label associated with the checkbox then toggle the check value
        'chkBoring.CheckState = IIf((chkBoring.CheckState = System.Windows.Forms.CheckState.Checked), System.Windows.Forms.CheckState.Unchecked, System.Windows.Forms.CheckState.Checked)

        If (chkBoring.CheckState = System.Windows.Forms.CheckState.Checked) Then
            chkBoring.CheckState = System.Windows.Forms.CheckState.Unchecked
        Else
            chkBoring.CheckState = System.Windows.Forms.CheckState.Checked
        End If

    End Sub

    Private Sub LoadDBCombo()

        Dim oRow As cdsDBNames.DBListRow
        mbSkipIndexChange = True

        ComboBox1.BeginUpdate()

        Dim lst As New List(Of DatabaseComboBoxItem)()

    
        For Each oRow In gDSDBs.DBList.Rows
            If Not String.IsNullOrEmpty(oRow.DBName) Then
                Dim displayName As String

                If oRow.DBIsConnected = True Then
                    displayName = oRow.DBName
                Else
                    displayName = oRow.DBName & " - (not attached)"
                End If

                If oRow.XMLFileIsReadOnly = True Then
                    displayName += " * ReadOnly File * "
                End If

                lst.Add(New DatabaseComboBoxItem() With {.DisplayName = displayName, .DBName = oRow.DBName, .Attached = oRow.DBIsConnected, .IsReadOnly = oRow.XMLFileIsReadOnly})
            End If
        Next oRow

        'sort the list
        lst = lst.OrderByDescending(Function(rec) rec.Attached).ThenBy(Function(rec) rec.DBName).ToList()

        'add a blank row, this is done AFTER sorting, so we can insert it at the top of the list
        lst.Insert(index:=0, item:=New DatabaseComboBoxItem() With {.DisplayName = "", .DBName = "", .Attached = False, .IsReadOnly = True})
        ComboBox1.DataSource = lst
        ComboBox1.DisplayMember = "DisplayName"
        ComboBox1.ValueMember = "DBName"
        'Dim tbl As New DataTable
        'tbl.Columns.Add(columnName:="DisplayName", type:=GetType(String), expression:=Nothing)
        'tbl.Columns.Add(columnName:="Name", type:=GetType(String), expression:=Nothing)
        'tbl.Columns.Add(columnName:="Attached", type:=GetType(Boolean), expression:=Nothing)
        'tbl.Columns.Add(columnName:="ReadOnly", type:=GetType(Boolean), expression:=Nothing)

        ''the "blank" row is listed as "attached" so that it will show at the top of the list
        'tbl.Rows.Add(New Object() {"", "", True})
        'For Each oRow In gDSDBs.DBList.Rows
        '    If Not String.IsNullOrEmpty(oRow.DBName) Then
        '        Dim displayName As String

        '        If oRow.DBIsConnected = True Then
        '            displayName = oRow.DBName
        '        Else
        '            displayName = oRow.DBName & " - (not attached)"
        '        End If

        '        If oRow.XMLFileIsReadOnly = True Then
        '            displayName += " * ReadOnly File * "
        '        End If

        '        tbl.Rows.Add(New Object() {displayName, oRow.DBName, oRow.DBIsConnected, oRow.XMLFileIsReadOnly})

        '        'AddDBInfoToList(oRow)
        '    End If
        'Next oRow
        'tbl.DefaultView.Sort = "Attached DESC, Name ASC"
        'ComboBox1.DataSource = tbl.DefaultView
        'ComboBox1.DisplayMember = "DisplayName"
        'ComboBox1.ValueMember = "Name"
        ComboBox1.EndUpdate()

        mbSkipIndexChange = False
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If Not mbSkipIndexChange Then
            If ComboBox1.SelectedIndex >= 0 Then
                Dim sError As String = String.Empty

                'Get the text value of the entry based on combobox1.selectedvalue
                Dim record As DatabaseComboBoxItem = CType(ComboBox1.SelectedItem, Global.ScriptScripter.frmMain.DatabaseComboBoxItem)

                cmdCommit.Enabled = Not record.IsReadOnly
            End If
        End If
    End Sub

    Private Sub ApplyColorScheme()
        Dim lLabelColor As Integer
        Dim lTextBoxColor As Integer
        Dim lButtonColor As Integer
        Dim lComboBoxColor As Integer
        Dim oControl As System.Windows.Forms.Control

        If chkBoring.CheckState = System.Windows.Forms.CheckState.Checked Then
            Me.BackgroundImage = Nothing 'System.Drawing.Image.FromFile("")
            lLabelColor = &H80000012
            lButtonColor = &H8000000F
            lTextBoxColor = &H80000005
            lComboBoxColor = &H80000005
        Else
            lTextBoxColor = &H80C0FF
            lLabelColor = &HFF
            lButtonColor = &H80FF
            'lComboBoxColor = &HFF
            lComboBoxColor = &H80C0FF
            Me.BackgroundImage = My.Resources.MainBackground
        End If

        For Each oControl In Me.Controls
            If TypeOf oControl Is System.Windows.Forms.Label Then
                oControl.ForeColor = System.Drawing.ColorTranslator.FromOle(lLabelColor)

            ElseIf TypeOf oControl Is System.Windows.Forms.Button Then
                oControl.BackColor = System.Drawing.ColorTranslator.FromOle(lButtonColor)

            ElseIf TypeOf oControl Is System.Windows.Forms.CheckBox Then
                oControl.ForeColor = System.Drawing.ColorTranslator.FromOle(lLabelColor)

            ElseIf TypeOf oControl Is System.Windows.Forms.TextBox Then
                If CType(oControl, TextBox).Enabled = True Then
                    oControl.BackColor = System.Drawing.ColorTranslator.FromOle(lTextBoxColor)
                Else
                    oControl.BackColor = System.Drawing.Color.Gray
                End If

            ElseIf TypeOf oControl Is System.Windows.Forms.ComboBox Then
                oControl.BackColor = System.Drawing.ColorTranslator.FromOle(lComboBoxColor)
            End If
        Next oControl
    End Sub

    Private Sub AddDBInfoToList(ByVal oRow As cdsDBNames.DBListRow, updater As cUpdateMultipleDBs)
        If Not String.IsNullOrEmpty(oRow.DBName) Then
            If updater.TestDatabaseConnection(sDBName:=oRow.DBName) Then
                If InitXMLDoc(sDBName:=oRow.DBName, sFileName:=oRow.XMLFileName, sErrorMessage:="XML document could not be loaded for db:  " & oRow.DBName) Then
                    If oRow.RevisionsToRun > 0 Then
                        'lstfrmServerInfo.Items.Insert(0, sDBName & " " & "revision is " & iDBRevision & ", current xml revision is " & iXMLRevision)
                        lstfrmServerInfo.Items.Insert(0, "Number of " & oRow.DBName & " revisions:  " & oRow.RevisionsToRun)
                    ElseIf oRow.RevisionsToRun < 0 Then
                        lstfrmServerInfo.Items.Insert(0, oRow.DBName & " will not be updated.  Your XML revision is lower than the current revision in the database")
                    End If
                End If
            End If

        End If
    End Sub

    Private Sub cmdCommit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCommit.Click
        'problem:  when you save a revision it uses mod_common.db_name to decide what database to save it to, 
        'not whatever database is in the combobox.  Why does it cache the database name?
        If SaveRevision() Then
            'need to update the form information
            'If Not gDSDBs Is Nothing Then
            '    gDSDBs.Dispose()
            'End If

            'gDSDBs = New cdsDBNames

            RefreshServerInfo()

            MsgBox("Scripted successfully!")
        End If
    End Sub

    Private Sub RefreshServerInfo()
        lstfrmServerInfo.Items.Clear()
        Using updater As New cUpdateMultipleDBs
            For Each oRow As cdsDBNames.DBListRow In gDSDBs.DBList.Rows
                AddDBInfoToList(oRow, updater:=updater)
            Next oRow
        End Using
    End Sub

    Private Function SaveRevision() As Boolean
        Dim oRow As dsRevisions.RevisionsRow
        'Dim oMasterRow As dsMasterSequence.MasterSequenceRow
        Dim sFileName As String
        Dim sDBName As String

        If ValidateFields() Then
            'check to make sure this revision hasn't already been added.  But who really cares if it has?

            'don't do this anymore.  Instead, get the dbname from the selected combo
            'sFileName = mod_Common.GetXMLFileName(DB_Name)
            sDBName = CStr(ComboBox1.SelectedValue)

            sFileName = mod_Common.GetFileNameFromDBName(sDBName)
            Try
                goDS = New dsRevisions

                Dim fileInfo As System.IO.FileInfo = New IO.FileInfo(fileName:=sFileName)
                'load the xml if the file exists and it has data (if the length is 0 the file exists but is blank, which is fine, no errors fro having a blank file, we simply just do not load any content.  if we attempted to read a blank file we would get an error saying root element did not exist)
                If fileInfo.Exists AndAlso fileInfo.Length > 0 Then
                    goDS.ReadXml(sFileName)
                End If
                oRow = goDS.Revisions.NewRevisionsRow

                oRow.Developer_Name = txtDeveloper.Text
                oRow.Developer_Comments = txtComments.Text
                oRow.SQL_Statement = txtSQL.Text
                oRow.RevisionDate = Date.Now 'Format(Date.Now, "M/dd/yyyy h:mm:ss.fff tt")
                goDS.Revisions.AddRevisionsRow(oRow)

                Try
                    goDS.WriteXml(sFileName)
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Error applying revision - File not checked out?")
                    Return False
                End Try

                'oMasterRow = goMasterDB.MasterSequence.NewMasterSequenceRow
                'oMasterRow.Rev_ID = oRow.Revision_ID
                'oMasterRow.DBName = sDBName
                'goMasterDB.MasterSequence.AddMasterSequenceRow(oMasterRow)

                For Each oDBRow As cdsDBNames.DBListRow In gDSDBs.DBList
                    If oDBRow.DBIsConnected = True AndAlso oDBRow.DBName = sDBName Then
                        oDBRow.RevisionsToRun += 1
                        oDBRow.XMLRevision += 1
                        oDBRow.XMLFileName = sFileName
                    End If
                Next oDBRow

                'Try
                '    mod_Common.SaveMasterDB()
                'Catch ex As Exception
                '    MsgBox(ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Error applying revision - File not checked out?")
                '    If Not oRow Is Nothing Then
                '        goDS.Revisions.RemoveRevisionsRow(oRow)
                '        goDS.WriteXml(sFileName)
                '    End If
                '    Return False
                'End Try

                'update the revision text to show that this one was added.
                'mbCodeSetRevision = True
                'miTotalRevisions = CType(oMasterRow.MasterSequence_ID + 1, Integer)
                'mbCodeSetRevision = False
                If Not chkRetainComment.Checked Then txtComments.Text = ""
                If Not ChkRetainSQL.Checked Then txtSQL.Text = ""
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Error applying revision")
                Return False
            End Try

            Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REG_DEVELOPER_LOCATION, True).SetValue(REG_DEVELOPER_ITEM, txtDeveloper.Text)

            oRow = Nothing
            'oMasterRow = Nothing

            Return True
        End If

    End Function

    Private Sub cmdServerInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdServerInfo.Click
        Dim oFrm As New frmServerInfo
        For Each oRow As cdsDBNames.DBListRow In gDSDBs.DBList.Rows
            oFrm.Label1.Text = "Database information for server " & lblServer.Text
            oFrm.tvServerInfo.Nodes.Add(oRow.DBName, oRow.DBName)
            With oFrm.tvServerInfo.Nodes.Item(oRow.DBName).Nodes
                .Add("DB is connected:  " & oRow.DBIsConnected)
                If oRow.DBIsConnected Then
                    If oRow.IsLastRevRunNull Then
                    Else
                        .Add("Last revision run:  " & IIf(oRow.IsLastRevRunNull, "null", oRow.LastRevRun).ToString)
                    End If
                    If oRow.IsXMLRevisionNull Then
                    Else
                        .Add("Local XML revision number:  " & IIf(oRow.IsXMLRevisionNull, "null", oRow.XMLRevision).ToString)
                    End If
                    If oRow.IsRevisionsToRunNull Then
                    Else
                        .Add("Revisions to run:  " & IIf(oRow.IsRevisionsToRunNull, "null", oRow.RevisionsToRun).ToString)
                    End If
                End If
            End With

        Next oRow

        oFrm.ShowDialog(Me)
        oFrm.Close()
        oFrm = Nothing
    End Sub

    Private Sub cmdShowRevisions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdShowRevisions.Click
        frmShowRevisions.ShowRevisionsToRun(owner:=Me)
    End Sub

    Private Sub cmdChangeConnection_Click(sender As Object, e As EventArgs) Handles cmdChangeConnection.Click
        If Me.ChangeConnection() Then
            mod_Common.LoadDBList()
            SetServerLabel()
        End If
    End Sub

    Private Function ChangeConnection() As Boolean
        Using frm As New frmConnect()
            If frm.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                Return True
            Else
                Return False
            End If
        End Using
    End Function

    Private Sub frmMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            Me.Cursor = Cursors.WaitCursor

            cmdCommit.Enabled = False 'disabled by default, enabled once a db is chosen

            chkBoring.Checked = My.MySettings.Default.DisableBackgroundImage
            chkBoring_CheckStateChanged(chkBoring, New System.EventArgs)

            SetDeveloperText()

            'if they don't have a preset server, then do the connection click
            '  else, go ahead and load the dblist based on their current connection
            If mod_Common.CurrentConnectionParameters Is Nothing OrElse String.IsNullOrEmpty(mod_Common.CurrentConnectionParameters.Server) = True Then
                Me.ChangeConnection()
            End If
            Me.Cursor = Cursors.WaitCursor
            'regardless of whether or not change connection was called or worked, we need to load the list of DB's
            mod_Common.LoadDBList()

            SetServerLabel()
            LoadDBCombo()
            RefreshServerInfo()
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Class DatabaseComboBoxItem
        Public Property DisplayName As String
        Public Property DBName As String
        Public Property Attached As Boolean
        Public Property IsReadOnly As Boolean
    End Class
End Class

