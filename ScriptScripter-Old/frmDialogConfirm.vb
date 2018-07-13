Imports System.Windows.Forms

Public Class frmDialogConfirm

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private msLabelText As String
    Public Property LabelText() As String
        Get
            Return msLabelText
        End Get
        Set(ByVal value As String)
            msLabelText = value
        End Set
    End Property

    Private mbLocalServer As Boolean
    Public Property LocalServer() As Boolean
        Get
            Return mbLocalServer
        End Get
        Set(ByVal value As Boolean)
            mbLocalServer = value
        End Set
    End Property

    Private Sub frmDialogConfirm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If mbLocalServer Then
            lblRemote.Visible = False
        End If
    End Sub

    Private Sub Show_button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Show_button.Click
        frmShowRevisions.ShowRevisionsToRun(owner:=Me)
    End Sub
End Class
