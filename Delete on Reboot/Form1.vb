Imports Microsoft.Win32
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices

Public Class Form1
    ' Instance of deleteAtReboot class to manage file operations
    Private deleteAtReboot As New deleteAtReboot
    ' Boolean to track if the form has finished loading
    Private boolDoneLoading As Boolean = False

    ' Event handler for Browse button click
    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        ' Set the title of the OpenFileDialog and show it
        OpenFileDialog1.Title = "Select a file to be deleted on reboot..."
        OpenFileDialog1.ShowDialog()
    End Sub

    ' Event handler for when a file is selected in the OpenFileDialog
    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        ' Set the selected file path to the text box
        txtFile.Text = OpenFileDialog1.FileName
    End Sub

    ' Event handler for the context menu checkbox click
    Private Sub chkAddToContextMenu_Click(sender As Object, e As EventArgs) Handles chkAddToContextMenu.Click
        If chkAddToContextMenu.Checked Then
            ' Add "Delete on Reboot" to the context menu in the registry
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
            ' Remove "Delete on Reboot" from the context menu in the registry
            Using registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("*\shell", True)
                registryKey.DeleteSubKeyTree("Delete on Reboot")
            End Using
        End If
    End Sub

    ' Event handler for form load
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Check if the context menu entry exists and update the checkbox state
        Using registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("*\shell\Delete on Reboot\command", True)
            If registryKey IsNot Nothing Then
                Dim matches As Match = Regex.Match(registryKey.GetValue(vbNullString), "(""{0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)""{0,1} )(.*)", RegexOptions.IgnoreCase)

                If matches.Groups(1).Value.Trim.Replace("""", "") <> Application.ExecutablePath Then
                    registryKey.SetValue(vbNullString, """" & Application.ExecutablePath & """ ""%1""", RegistryValueKind.String)
                End If

                chkAddToContextMenu.Checked = True
            End If
        End Using

        ' Load staged operations and restore UI settings
        loadStagedOperations()
        colFileName.Width = My.Settings.fileNameColumnSize
        colRenamedTo.Width = My.Settings.renamedToColumnSize
        If colRenamedTo.Width < 100 Then colRenamedTo.Width = 100
        Size = My.Settings.windowSize

        boolDoneLoading = True
    End Sub

    ' Event handler for Remove Item button click
    Private Sub btnRemoveItem_Click(sender As Object, e As EventArgs) Handles btnRemoveItem.Click
        If listOperations.SelectedItems.Count > 0 Then
            ' Remove selected items from the pending operations
            For Each item As operationsListEntry In listOperations.SelectedItems
                deleteAtReboot.removeItem(item.GUID)
            Next

            ' Reload the list and enable the Save button
            loadStagedOperations()
            btnSave.Enabled = True
        End If
    End Sub

    ' Load staged operations into the ListView
    Sub loadStagedOperations()
        Dim toBePutIntoTheListOnTheGUI As New List(Of operationsListEntry)

        ' Add current pending operations to the list
        If deleteAtReboot.currentPendingOperations.Count <> 0 Then
            For Each item As deleteAtRebootStructure In deleteAtReboot.currentPendingOperations
                toBePutIntoTheListOnTheGUI.Add(item.CreateListViewEntry())
            Next
        End If

        ' Clear and update the ListView
        listOperations.Items.Clear()
        listOperations.Items.AddRange(toBePutIntoTheListOnTheGUI.ToArray())

        ' Update the label with the number of pending operations
        lblPendingOperationsLabel.Text = $"Current Staged Operations ({listOperations.Items.Count} {If(listOperations.Items.Count = 1, "item", "items")})"
    End Sub

    ' Event handler for Save button click
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' Save pending operations and notify the user
        deleteAtReboot.dispose(True)
        MsgBox("Pending operations saved.", MsgBoxStyle.Information, Text)
    End Sub

    ' Event handler for Stage Rename button click
    Private Sub btnStageRename_Click(sender As Object, e As EventArgs) Handles btnStageRename.Click
        If String.IsNullOrEmpty(txtFile.Text) Then
            ' Show error if no file is provided
            MsgBox("You must provide a file to work with.", MsgBoxStyle.Critical, Text)
        Else
            ' Prompt user to enter a new file name
            SaveFileDialog1.Title = "Enter new file name..."
            SaveFileDialog1.FileName = txtFile.Text

            If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                ' Add rename operation and save
                deleteAtReboot.addItem(txtFile.Text, SaveFileDialog1.FileName)
                deleteAtReboot.save()

                ' Reload operations and ask for reboot
                loadStagedOperations()
                askForReboot()
            End If
        End If
    End Sub

    ' Event handler for Delete button click
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If String.IsNullOrEmpty(txtFile.Text) Then
            ' Show error if no file is provided
            MsgBox("You must provide a file to work with.", MsgBoxStyle.Critical, Text)
        Else
            ' Add delete operation and save
            deleteAtReboot.addItem(txtFile.Text)
            deleteAtReboot.save()

            ' Reload operations and ask for reboot
            loadStagedOperations()
            askForReboot()
        End If
    End Sub

    ' Prompt the user to reboot the system
    Private Sub askForReboot()
        Dim rebootRequestResponse As MsgBoxResult = MsgBox("The file has been scheduled to be deleted at reboot." & vbCrLf & vbCrLf & "Do you want to reboot your computer now?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Reboot now?")

        If rebootRequestResponse = MsgBoxResult.Yes Then
            ' Save operations and initiate system reboot
            deleteAtReboot.save()
            Shell("shutdown.exe -r -t 0", AppWinStyle.Hide)
        End If
    End Sub

    ' Event handler for column width change in the ListView
    Private Sub listOperations_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles listOperations.ColumnWidthChanged
        If boolDoneLoading Then
            ' Save column width settings
            My.Settings.fileNameColumnSize = colFileName.Width
            My.Settings.renamedToColumnSize = colRenamedTo.Width
            If colExist.Width < 60 Then colExist.Width = 60
            If colRenamedTo.Width < 100 Then colRenamedTo.Width = 100
        End If
    End Sub

    ' Event handler for form resize end
    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        If boolDoneLoading Then
            ' Save window size settings
            My.Settings.windowSize = Size
        End If
    End Sub

    ' Event handler for key up event in the ListView
    Private Sub listOperations_KeyUp(sender As Object, e As KeyEventArgs) Handles listOperations.KeyUp
        If e.KeyCode = Keys.Delete Then
            ' Trigger Remove Item button click on Delete key press
            btnRemoveItem.PerformClick()
        End If
    End Sub

    Public Sub SelectFileInWindowsExplorer(strFullPath As String)
        If Not String.IsNullOrEmpty(strFullPath) Then
            If IO.File.Exists(strFullPath) Then
                ' It's a file, select it
                Dim pidlList As IntPtr = NativeMethods.ILCreateFromPathW(strFullPath)

                If Not pidlList.Equals(IntPtr.Zero) Then
                    Try
                        NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0)
                    Finally
                        NativeMethods.ILFree(pidlList)
                    End Try
                End If
            ElseIf IO.Directory.Exists(strFullPath) Then
                ' It's a directory, open the folder
                Dim pidlList As IntPtr = NativeMethods.ILCreateFromPathW(strFullPath)

                If Not pidlList.Equals(IntPtr.Zero) Then
                    Try
                        NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0)
                    Finally
                        NativeMethods.ILFree(pidlList)
                    End Try
                End If
            End If
        End If
    End Sub

    Private Sub FileListMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles FileListMenu.Opening
        If listOperations.SelectedItems.Count > 1 Then e.Cancel = True
    End Sub

    Private Sub OpenExplorerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenExplorerToolStripMenuItem.Click
        If listOperations.SelectedItems.Count = 1 Then
            Dim selectedItem As operationsListEntry = CType(listOperations.SelectedItems(0), operationsListEntry)

            Dim path As String = selectedItem.strFileToBeWorkedOn ' Path to check

            Try
                ' Get the attributes of the file/directory
                Dim attributes As IO.FileAttributes = IO.File.GetAttributes(path)

                ' Check if the path exists and is a directory or file
                If (attributes And IO.FileAttributes.Directory) = IO.FileAttributes.Directory Then
                    SelectFileInWindowsExplorer(path)
                Else
                    SelectFileInWindowsExplorer(path)
                End If
            Catch ex As IO.FileNotFoundException
                ' Path does not exist
                MsgBox($"The file or directory ""{path}"" no longer exists.", MsgBoxStyle.Exclamation, "Error")
            Catch ex As Exception
                ' Other error (e.g., permission issue)
                MsgBox($"An error occurred while accessing ""{path}"". {ex.Message}", MsgBoxStyle.Exclamation, "Error")
            End Try
        End If
    End Sub
End Class

' This class extends the ListViewItem so that additional properties can be added
Public Class operationsListEntry
    Inherits ListViewItem
    ' Indicates if the file is marked for deletion
    Public Property boolDelete As Boolean
    ' The file to be worked on
    Public Property strFileToBeWorkedOn As String
    ' The new name for the file if being renamed
    Public Property strToBeRenamedTo As String
    ' Indicates if the file exists
    Public Property boolExists As Boolean
    ' Unique identifier for the operation
    Public Property GUID As Guid
End Class

Friend Class NativeMethods
    <DllImport("shell32.dll", ExactSpelling:=True)>
    Public Shared Function SHOpenFolderAndSelectItems(pidlList As IntPtr, cild As UInteger, children As IntPtr, dwFlags As UInteger) As Integer
    End Function

    <DllImport("shell32.dll", ExactSpelling:=True)>
    Public Shared Sub ILFree(pidlList As IntPtr)
    End Sub

    <DllImport("shell32.dll", CharSet:=CharSet.Unicode, ExactSpelling:=True)>
    Public Shared Function ILCreateFromPathW(pszPath As String) As IntPtr
    End Function
End Class