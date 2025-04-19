Imports System.Text.RegularExpressions
Imports Microsoft.Win32

''' <summary>This class manages the pending operations at system reboot.</summary>
Public Class deleteAtReboot
    Public Property currentPendingOperations As List(Of deleteAtRebootStructure)
    Private boolThingsChanged As Boolean = False

    ''' <summary>This class manages the pending operations at system reboot.</summary>
    Public Sub New()
        currentPendingOperations = loadStagedPendingOperations()
    End Sub

    ''' <summary>This saves any and all changes to the system Registry.</summary>
    Public Sub save()
        If boolThingsChanged Then saveStagedPendingOperations()
    End Sub

    ''' <summary>This closes out this class instance.</summary>
    ''' <param name="boolSaveChanges">This is an optional parameter. Set to True if you want to save any and all changes back to the system Registry.</param>
    Public Sub dispose(Optional boolSaveChanges As Boolean = False)
        If boolSaveChanges And boolThingsChanged Then saveStagedPendingOperations()
        currentPendingOperations.Clear()
    End Sub

    ''' <summary>This function removes an item from the list of pending file operations that are to occur at the next system reboot.</summary>
    ''' <param name="itemsGUIDToRemove">The GUID for the entry.</param>
    Public Sub removeItem(itemsGUIDToRemove As Guid)
        If currentPendingOperations.Any() Then
            Dim newCurrentPendingOperations As New List(Of deleteAtRebootStructure)

            For Each item As deleteAtRebootStructure In currentPendingOperations
                If Not item.GUID.Equals(itemsGUIDToRemove) Then
                    newCurrentPendingOperations.Add(item)
                    boolThingsChanged = True
                End If
            Next

            currentPendingOperations = newCurrentPendingOperations
        End If
    End Sub

    ''' <summary>This adds an item to be deleted to the list of operations.</summary>
    ''' <param name="strFileToBeDeleted">The file to be deleted.</param>
    Public Sub addItem(strFileToBeDeleted As String)
        If String.IsNullOrWhiteSpace(strFileToBeDeleted) Then Throw New ArgumentNullException(NameOf(strFileToBeDeleted))
        currentPendingOperations.Add(New deleteAtRebootStructure(strFileToBeDeleted))
        boolThingsChanged = True
    End Sub

    ''' <summary>This adds an item to be renamed to the list of operations.</summary>
    ''' <param name="strFileToBeRenamed">The name of the file to be renamed.</param>
    ''' <param name="strNewName">The new name of the file.</param>
    ''' <exception cref="ArgumentNullException" />
    Public Sub addItem(strFileToBeRenamed As String, strNewName As String)
        If String.IsNullOrWhiteSpace(strFileToBeRenamed) Then Throw New ArgumentNullException(NameOf(strFileToBeRenamed))
        If String.IsNullOrWhiteSpace(strNewName) Then Throw New ArgumentNullException(NameOf(strNewName))
        currentPendingOperations.Add(New deleteAtRebootStructure(strFileToBeRenamed, strNewName))
        boolThingsChanged = True
    End Sub

    Private Function RemoveNumber(strInput As String) As String
        Return Regex.Replace(strInput, "\*[0-9]{1,2}\!{0,1}", "")
    End Function

    ''' <summary>This function loads the list of pending operations at system reboot.</summary>
    ''' <returns>A list of pending operations.</returns>
    Private Function loadStagedPendingOperations() As List(Of deleteAtRebootStructure)
        Dim _pendingOperations As New List(Of deleteAtRebootStructure)

        Try
            Dim pendingOperations As String()
            Using registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Session Manager", False)
                pendingOperations = registryKey.GetValue("PendingFileRenameOperations")
            End Using

            Dim strFileToBeWorkedOn, strFileToBeRenamedTo As String

            If pendingOperations IsNot Nothing Then
                For i = 0 To pendingOperations.Count - 1 Step 2
                    strFileToBeWorkedOn = RemoveNumber(pendingOperations(i).Replace("\??\", ""))
                    strFileToBeRenamedTo = RemoveNumber(pendingOperations(i + 1).Replace("\??\", ""))

                    If String.IsNullOrEmpty(pendingOperations(i + 1).Trim) Then
                        _pendingOperations.Add(New deleteAtRebootStructure(strFileToBeWorkedOn))
                    Else
                        _pendingOperations.Add(New deleteAtRebootStructure(strFileToBeWorkedOn, strFileToBeRenamedTo))
                    End If
                Next
            End If

            Return _pendingOperations
        Catch ex As Exception
            Return _pendingOperations
        End Try
    End Function

    ''' <summary>This function saves any and all pending operations within this class instance back to the system Registry.</summary>
    Private Sub saveStagedPendingOperations()
        Using registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Session Manager", True)
            If currentPendingOperations.Count = 0 Then
                registryKey.DeleteValue("PendingFileRenameOperations", False)
            Else
                Dim itemsToBeSavedToTheRegistry As New Specialized.StringCollection()

                For Each pendingOperation As deleteAtRebootStructure In currentPendingOperations
                    If pendingOperation.boolDelete Then
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strFileToBeWorkedOn)
                        itemsToBeSavedToTheRegistry.Add("")
                    Else
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strFileToBeWorkedOn)
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strToBeRenamedTo)
                    End If
                Next

                Dim valuesToBeSavedToTheRegistry(itemsToBeSavedToTheRegistry.Count - 1) As String
                itemsToBeSavedToTheRegistry.CopyTo(valuesToBeSavedToTheRegistry, 0)

                registryKey.SetValue("PendingFileRenameOperations", valuesToBeSavedToTheRegistry, RegistryValueKind.MultiString)
            End If
        End Using
    End Sub
End Class

Public Structure deleteAtRebootStructure
    Public Property boolDelete As Boolean
    Public Property boolExists As Boolean
    Public Property strFileToBeWorkedOn As String
    Public Property strToBeRenamedTo As String
    Public Property GUID As Guid

    ''' <summary>This adds an item to be renamed.</summary>
    ''' <param name="strFileToBeWorkedOn">The file to be renamed.</param>
    ''' <param name="strToBeRenamedTo">The new name of the file to be renamed.</param>
    Public Sub New(strFileToBeWorkedOn As String, strToBeRenamedTo As String)
        boolExists = IO.File.Exists(strFileToBeWorkedOn) Or IO.Directory.Exists(strFileToBeWorkedOn)
        Me.strFileToBeWorkedOn = strFileToBeWorkedOn
        Me.strToBeRenamedTo = strToBeRenamedTo
        GUID = Guid.NewGuid()
        boolDelete = False
    End Sub

    ''' <summary>This adds an item to be deleted.</summary>
    ''' <param name="strFileToDelete">The file to be deleted from the pending operations.</param>
    Public Sub New(strFileToDelete As String)
        boolExists = IO.File.Exists(strFileToDelete) Or IO.Directory.Exists(strFileToDelete)
        strFileToBeWorkedOn = strFileToDelete
        GUID = Guid.NewGuid()
        strToBeRenamedTo = Nothing
        boolDelete = True
    End Sub
End Structure