
Namespace My
    
    'This class allows you to handle specific events on the settings class:
    ' The SettingChanging event is raised before a setting's value is changed.
    ' The PropertyChanged event is raised after a setting's value is changed.
    ' The SettingsLoaded event is raised after the setting values are loaded.
    ' The SettingsSaving event is raised before the setting values are saved.
    Partial Friend NotInheritable Class MySettings

        Private Sub MySettings_SettingsLoaded(sender As Object, e As Configuration.SettingsLoadedEventArgs) Handles Me.SettingsLoaded
            'decrypt password
            If String.IsNullOrEmpty(My.Settings.DBConnection_Password) = False Then
                My.Settings.DBConnection_Password = Security.PasswordDecrypt(My.Settings.DBConnection_Password)
            End If
        End Sub

        Private Sub MySettings_SettingsSaving(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.SettingsSaving
            'encrypt password
            If String.IsNullOrEmpty(My.Settings.DBConnection_Password) = False Then
                My.Settings.DBConnection_Password = Security.PasswordEncrypt(My.Settings.DBConnection_Password)
            End If
        End Sub
    End Class
End Namespace
