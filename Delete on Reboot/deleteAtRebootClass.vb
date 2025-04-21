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
        ' If the save changes flag is set to True and there are changes to be saved,
        ' call the method to save the staged pending operations to the system Registry.
        If boolSaveChanges And boolThingsChanged Then saveStagedPendingOperations()

        ' Clear the current list of pending operations to release resources and reset the state.
        currentPendingOperations.Clear()
    End Sub

    ''' <summary>This function removes an item from the list of pending file operations that are to occur at the next system reboot.</summary>
    ''' <param name="itemsGUIDToRemove">The GUID for the entry.</param>
    Public Sub removeItem(itemsGUIDToRemove As Guid)
        ' First we check to see if there are any objects in the list.
        If currentPendingOperations.Any() Then
            ' OK, there are objects in the list to work with. We now create a new list to temporarily hold the objects that we want to keep.
            Dim newCurrentPendingOperations As New List(Of deleteAtRebootStructure)

            ' We now loop through the current list of pending operations and add any objects that do not match the GUID of the object we want to remove to the new list.
            For Each item As deleteAtRebootStructure In currentPendingOperations
                ' We now check to see if the GUID of the current object matches the GUID of the object we want to remove. If it does not match, we add it to the new list.
                If Not item.GUID.Equals(itemsGUIDToRemove) Then
                    ' We now add the object to the new list.
                    newCurrentPendingOperations.Add(item)
                    ' We set the boolThingsChanged bit to True to indicate that things have changed.
                    boolThingsChanged = True
                End If
            Next

            ' We now copy the new list we created at the beginning of this subroutine onto the global list of pending operations for the class instance.
            currentPendingOperations = newCurrentPendingOperations
        End If
    End Sub

    ''' <summary>This adds an item to be deleted to the list of operations.</summary>
    ''' <param name="strFileToBeDeleted">The file to be deleted.</param>
    Public Sub addItem(strFileToBeDeleted As String)
        ' Check if the provided file path is null, empty, or consists only of whitespace.
        If String.IsNullOrWhiteSpace(strFileToBeDeleted) Then
            ' Throw an exception if the file path is invalid.
            Throw New ArgumentNullException(NameOf(strFileToBeDeleted))
        End If

        ' Add a new deleteAtRebootStructure instance for the file to be deleted to the list of pending operations.
        currentPendingOperations.Add(New deleteAtRebootStructure(strFileToBeDeleted))

        ' Mark that changes have been made to the pending operations list.
        boolThingsChanged = True
    End Sub

    ''' <summary>This adds an item to be renamed to the list of operations.</summary>
    ''' <param name="strFileToBeRenamed">The name of the file to be renamed.</param>
    ''' <param name="strNewName">The new name of the file.</param>
    ''' <exception cref="ArgumentNullException" />
    Public Sub addItem(strFileToBeRenamed As String, strNewName As String)
        ' Check if the provided file path to be renamed is null, empty, or consists only of whitespace.
        If String.IsNullOrWhiteSpace(strFileToBeRenamed) Then
            ' Throw an exception if the file path to be renamed is invalid.
            Throw New ArgumentNullException(NameOf(strFileToBeRenamed))
        End If

        ' Check if the provided new file name is null, empty, or consists only of whitespace.
        If String.IsNullOrWhiteSpace(strNewName) Then
            ' Throw an exception if the new file name is invalid.
            Throw New ArgumentNullException(NameOf(strNewName))
        End If

        ' Add a new deleteAtRebootStructure instance for the file rename operation to the list of pending operations.
        currentPendingOperations.Add(New deleteAtRebootStructure(strFileToBeRenamed, strNewName))

        ' Mark that changes have been made to the pending operations list.
        boolThingsChanged = True
    End Sub

    Private Function RemoveNumber(strInput As String) As String
        ' Use a regular expression to remove patterns matching "*<number>!" or "*<number>" from the input string.
        Return Regex.Replace(strInput, "\*[0-9]{1,2}\!{0,1}", "")
    End Function

    ''' <summary>This function loads the list of pending operations at system reboot.</summary>
    ''' <returns>A list of pending operations.</returns>
    Private Function loadStagedPendingOperations() As List(Of deleteAtRebootStructure)
        ' Initialize a new list to store pending operations.
        Dim _pendingOperations As New List(Of deleteAtRebootStructure)

        Try
            ' Declare a variable to hold the registry value for pending operations.
            Dim pendingOperations As String()

            ' Open the registry key to read the "PendingFileRenameOperations" value.
            Using registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Session Manager", False)
                ' Retrieve the value of "PendingFileRenameOperations" from the registry.
                pendingOperations = registryKey.GetValue("PendingFileRenameOperations")
            End Using

            ' Declare variables to hold file paths for processing.
            Dim strFileToBeWorkedOn, strFileToBeRenamedTo As String

            ' Check if there are any pending operations retrieved from the registry.
            If pendingOperations IsNot Nothing Then
                ' Loop through the pending operations in pairs (source and target).
                For i = 0 To pendingOperations.Count - 1 Step 2
                    ' Remove any special patterns and prefixes from the source file path.
                    strFileToBeWorkedOn = RemoveNumber(pendingOperations(i).Replace("\??\", ""))
                    ' Remove any special patterns and prefixes from the target file path.
                    strFileToBeRenamedTo = RemoveNumber(pendingOperations(i + 1).Replace("\??\", ""))

                    ' Check if the target file path is empty or consists only of whitespace.
                    If String.IsNullOrEmpty(pendingOperations(i + 1).Trim) Then
                        ' Add a delete operation to the list of pending operations.
                        _pendingOperations.Add(New deleteAtRebootStructure(strFileToBeWorkedOn))
                    Else
                        ' Add a rename operation to the list of pending operations.
                        _pendingOperations.Add(New deleteAtRebootStructure(strFileToBeWorkedOn, strFileToBeRenamedTo))
                    End If
                Next
            End If

            ' Return the list of pending operations.
            Return _pendingOperations
        Catch ex As Exception
            ' In case of an exception, return the current list of pending operations (empty or partially filled).
            Return _pendingOperations
        End Try
    End Function

    ''' <summary>This function saves any and all pending operations within this class instance back to the system Registry.</summary>
    Private Sub saveStagedPendingOperations()
        ' Open the registry key for the "Session Manager" to modify its values.
        Using registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Session Manager", True)
            ' Check if there are no pending operations in the current list.
            If currentPendingOperations.Count = 0 Then
                ' If the list is empty, delete the "PendingFileRenameOperations" value from the registry.
                registryKey.DeleteValue("PendingFileRenameOperations", False)
            Else
                ' Create a collection to store the items to be saved to the registry.
                Dim itemsToBeSavedToTheRegistry As New Specialized.StringCollection()

                ' Iterate through each pending operation in the current list.
                For Each pendingOperation As deleteAtRebootStructure In currentPendingOperations
                    ' Check if the operation is a delete operation.
                    If pendingOperation.boolDelete Then
                        ' Add the file path to be deleted to the collection.
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strFileToBeWorkedOn)
                        ' Add an empty string to indicate a delete operation.
                        itemsToBeSavedToTheRegistry.Add("")
                    Else
                        ' Add the source file path to the collection for a rename operation.
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strFileToBeWorkedOn)
                        ' Add the target file path to the collection for a rename operation.
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strToBeRenamedTo)
                    End If
                Next

                ' Convert the collection to an array to save it to the registry.
                Dim valuesToBeSavedToTheRegistry(itemsToBeSavedToTheRegistry.Count - 1) As String
                itemsToBeSavedToTheRegistry.CopyTo(valuesToBeSavedToTheRegistry, 0)

                ' Save the array of pending operations to the "PendingFileRenameOperations" registry value.
                registryKey.SetValue("PendingFileRenameOperations", valuesToBeSavedToTheRegistry, RegistryValueKind.MultiString)
            End If
        End Using
    End Sub
End Class

''' <summary>
''' Represents a structure for managing file operations (delete or rename) 
''' that are scheduled to occur at the next system reboot.
''' </summary>
Public Structure deleteAtRebootStructure
    ''' <summary>
    ''' Indicates whether the operation is a delete operation.
    ''' </summary>
    Public Property boolDelete As Boolean

    ''' <summary>
    ''' Indicates whether the file or directory exists at the time of scheduling.
    ''' </summary>
    Public Property boolExists As Boolean

    ''' <summary>
    ''' The file or directory to be worked on (deleted or renamed).
    ''' </summary>
    Public Property strFileToBeWorkedOn As String

    ''' <summary>
    ''' The new name of the file or directory if it is a rename operation.
    ''' </summary>
    Public Property strToBeRenamedTo As String

    ''' <summary>
    ''' A unique identifier for the operation.
    ''' </summary>
    Public Property GUID As Guid

    ''' <summary>
    ''' Creates a ListView entry representation of the current deleteAtRebootStructure instance.
    ''' </summary>
    ''' <returns>An operationsListEntry object representing the current instance.</returns>
    Public Function CreateListViewEntry() As operationsListEntry
        ' Initialize a new operationsListEntry object with the properties of the current instance.
        Dim operationsListEntry As New operationsListEntry With {
            .GUID = GUID, ' Assign the GUID of the current instance.
            .strFileToBeWorkedOn = strFileToBeWorkedOn, ' Assign the file to be worked on.
            .strToBeRenamedTo = strToBeRenamedTo, ' Assign the new name if it's a rename operation.
            .boolDelete = boolDelete, ' Indicate whether this is a delete operation.
            .boolExists = boolExists ' Indicate whether the file or directory exists.
        }

        ' Configure the ListView entry based on the operation type (delete or rename).
        With operationsListEntry
            If boolDelete Then
                ' If it's a delete operation, set the text to the file path and add a "(To Be Deleted)" sub-item.
                .Text = strFileToBeWorkedOn
                .SubItems.Add("(To Be Deleted)")
            Else
                ' If it's a rename operation, set the text to the source file path and add the target file path as a sub-item.
                .Text = strFileToBeWorkedOn
                .SubItems.Add(strToBeRenamedTo)
            End If

            ' Add a sub-item indicating whether the file or directory exists.
            .SubItems.Add(If(boolExists, "Yes", "No"))

            ' If the file or directory does not exist, set the background color to pink.
            If Not boolExists Then .BackColor = Color.Pink
        End With

        ' Return the configured operationsListEntry object.
        Return operationsListEntry
    End Function

    ''' <summary>This adds an item to be renamed.</summary>
    ''' <param name="strFileToBeWorkedOn">The file to be renamed.</param>
    ''' <param name="strToBeRenamedTo">The new name of the file to be renamed.</param>
    Public Sub New(strFileToBeWorkedOn As String, strToBeRenamedTo As String)
        ' Check if the file or directory to be renamed exists.
        boolExists = IO.File.Exists(strFileToBeWorkedOn) Or IO.Directory.Exists(strFileToBeWorkedOn)
        ' Assign the file to be worked on to the corresponding property.
        Me.strFileToBeWorkedOn = strFileToBeWorkedOn
        ' Assign the new name of the file to the corresponding property.
        Me.strToBeRenamedTo = strToBeRenamedTo
        ' Generate a unique identifier for this operation.
        GUID = Guid.NewGuid()
        ' Set the operation type to rename (not delete).
        boolDelete = False
    End Sub

    ''' <summary>This adds an item to be deleted.</summary>
    ''' <param name="strFileToDelete">The file to be deleted from the pending operations.</param>
    Public Sub New(strFileToDelete As String)
        ' Check if the file or directory to be deleted exists.
        boolExists = IO.File.Exists(strFileToDelete) Or IO.Directory.Exists(strFileToDelete)
        ' Assign the file to be deleted to the corresponding property.
        strFileToBeWorkedOn = strFileToDelete
        ' Generate a unique identifier for this operation.
        GUID = Guid.NewGuid()
        ' Set the new name property to Nothing since this is a delete operation.
        strToBeRenamedTo = Nothing
        ' Set the operation type to delete.
        boolDelete = True
    End Sub
End Structure