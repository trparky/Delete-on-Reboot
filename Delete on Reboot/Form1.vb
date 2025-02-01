Imports Microsoft.Win32
Imports System.Text.RegularExpressions

Public Class Form1
    Private deleteAtReboot As New deleteAtReboot
    Private boolDoneLoading As Boolean = False

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        OpenFileDialog1.Title = "Select a file to be deleted on reboot..."
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        txtFile.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub chkAddToContextMenu_Click(sender As Object, e As EventArgs) Handles chkAddToContextMenu.Click
        If chkAddToContextMenu.Checked Then
            Using registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("*\shell", True)
                registryKey.CreateSubKey("Delete on Reboot")
            End Using

            Using registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("*\shell\Delete on Reboot", True)
                registryKey.CreateSubKey("command")
            End Using

            Using registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("*\shell\Delete on Reboot\command", True)
                registryKey.SetValue(vbNullString, """" & Application.ExecutablePath & """ ""%1""", RegistryValueKind.String)
            End Using
        Else
            Using registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("*\shell", True)
                registryKey.DeleteSubKeyTree("Delete on Reboot")
            End Using
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("*\shell\Delete on Reboot\command", True)

        If (registryKey Is Nothing) = False Then
            Dim matches As Match = Regex.Match(registryKey.GetValue(vbNullString), "(""{0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)""{0,1} )(.*)", RegexOptions.IgnoreCase)

            If matches.Groups(1).Value.Trim.Replace("""", "") <> Application.ExecutablePath Then
                registryKey.SetValue(vbNullString, """" & Application.ExecutablePath & """ ""%1""", RegistryValueKind.String)
            End If

            chkAddToContextMenu.Checked = True
            registryKey.Close()
            registryKey.Dispose()
        End If

        loadStagedOperations()
        colFileName.Width = My.Settings.fileNameColumnSize
        colRenamedTo.Width = My.Settings.renamedToColumnSize
        Size = My.Settings.windowSize

        boolDoneLoading = True
    End Sub

    Private Sub btnRemoveItem_Click(sender As Object, e As EventArgs) Handles btnRemoveItem.Click
        For Each item As operationsListEntry In listOperations.SelectedItems
            deleteAtReboot.removeItem(item.strFileToBeWorkedOn, True)
        Next

        loadStagedOperations()
        btnSave.Enabled = True
    End Sub

    Sub loadStagedOperations()
        Dim toBePutIntoTheListOnTheGUI As New List(Of operationsListEntry)
        Dim entryToBeAdded As operationsListEntry

        If deleteAtReboot.currentPendingOperations.Count <> 0 Then
            For Each item As deleteAtRebootStructure In deleteAtReboot.currentPendingOperations
                entryToBeAdded = New operationsListEntry With {
                    .strFileToBeWorkedOn = item.strFileToBeWorkedOn,
                    .strToBeRenamedTo = item.strToBeRenamedTo,
                    .boolDelete = item.boolDelete,
                    .boolExists = IO.File.Exists(item.strFileToBeWorkedOn)
                }
                entryToBeAdded.createItem()

                toBePutIntoTheListOnTheGUI.Add(entryToBeAdded)
            Next
        End If

        listOperations.Items.Clear()
        listOperations.Items.AddRange(toBePutIntoTheListOnTheGUI.ToArray())
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        deleteAtReboot.dispose(True)
        MsgBox("Pending operations saved.", MsgBoxStyle.Information, Text)
    End Sub

    Private Sub btnStageRename_Click(sender As Object, e As EventArgs) Handles btnStageRename.Click
        If String.IsNullOrEmpty(txtFile.Text) Then
            MsgBox("You must provide a file to work with.", MsgBoxStyle.Critical, Text)
        Else
            SaveFileDialog1.Title = "Enter new file name..."
            SaveFileDialog1.FileName = txtFile.Text

            If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                deleteAtReboot.addItem(txtFile.Text, SaveFileDialog1.FileName)
                deleteAtReboot.save()

                loadStagedOperations()
                askForReboot()
            End If
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If String.IsNullOrEmpty(txtFile.Text) Then
            MsgBox("You must provide a file to work with.", MsgBoxStyle.Critical, Text)
        Else
            deleteAtReboot.addItem(txtFile.Text)
            deleteAtReboot.save()

            loadStagedOperations()
            askForReboot()
        End If
    End Sub

    Private Sub askForReboot()
        Dim rebootRequestResponse As MsgBoxResult = MsgBox("The file has been scheduled to be deleted at reboot." & vbCrLf & vbCrLf & "Do you want to reboot your computer now?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Reboot now?")

        If rebootRequestResponse = MsgBoxResult.Yes Then
            deleteAtReboot.save()
            Shell("shutdown.exe -r -t 0", AppWinStyle.Hide)
        End If
    End Sub

    Private Sub listOperations_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles listOperations.ColumnWidthChanged
        If boolDoneLoading Then
            My.Settings.fileNameColumnSize = colFileName.Width
            My.Settings.renamedToColumnSize = colRenamedTo.Width
        End If
    End Sub

    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        If boolDoneLoading Then My.Settings.windowSize = Size
    End Sub

    Private Sub listOperations_KeyUp(sender As Object, e As KeyEventArgs) Handles listOperations.KeyUp
        If e.KeyCode = Keys.Delete Then btnRemoveItem.PerformClick()
    End Sub
End Class

' This class extends the ListViewItem so that I can add more properties to it for my purposes.
Public Class operationsListEntry
    Inherits ListViewItem
    Public Property boolDelete As Boolean
    Public Property strFileToBeWorkedOn As String
    Public Property strToBeRenamedTo As String
    Public Property boolExists As Boolean

    Public Sub createItem()
        If boolDelete Then
            Text = strFileToBeWorkedOn
            SubItems.Add("(To Be Deleted)")
        Else
            Text = strFileToBeWorkedOn
            SubItems.Add(strToBeRenamedTo)
        End If

        SubItems.Add(If(boolExists, "Yes", "No"))
        If Not boolExists Then BackColor = Color.Pink
    End Sub
End Class