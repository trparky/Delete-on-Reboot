Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        ' Parses command-line arguments into a dictionary of key-value pairs.
        ' Arguments should be in the format --key=value or --key for boolean flags.
        Private Function ParseArguments(args As ObjectModel.ReadOnlyCollection(Of String)) As Dictionary(Of String, Object)
            Dim parsedArguments As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            Dim strValue As String

            ' Iterate through each argument provided in the command line.
            For Each strArgument As String In args
                ' Check if the argument starts with "--" indicating a valid format.
                If strArgument.StartsWith("--") Then
                    ' Split the argument into key and value using "=" as the delimiter.
                    Dim splitArg As String() = strArgument.Substring(2).Split(New Char() {"="c}, 2)
                    Dim key As String = splitArg(0)

                    If splitArg.Length = 2 Then
                        ' If the argument has a value, store it in the dictionary.
                        strValue = splitArg(1)
                        parsedArguments(key) = strValue
                    Else
                        ' If no value is provided, treat it as a boolean flag and set it to True.
                        parsedArguments(key) = True
                    End If
                Else
                    ' Log unrecognized argument formats to the console.
                    Console.WriteLine($"Unrecognized argument format: {strArgument}")
                End If
            Next

            ' Return the parsed arguments as a dictionary.
            Return parsedArguments
        End Function

        ' Handles the Startup event of the application.
        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            ' Check if there is exactly one command-line argument.
            If Application.CommandLineArgs.Count = 1 Then
                ' Parse the command-line arguments into a dictionary.
                Dim parsedArguments As Dictionary(Of String, Object) = ParseArguments(Application.CommandLineArgs)

                ' Check if the "delete" argument exists and the specified file exists.
                If parsedArguments.ContainsKey("delete") AndAlso IO.File.Exists(parsedArguments("delete")) Then
                    ' Create an instance of deleteAtReboot and add the file to be deleted on reboot.
                    Dim deleteAtRebootInstance As New deleteAtReboot()
                    deleteAtRebootInstance.addItem(parsedArguments("delete"))
                    deleteAtRebootInstance.dispose(True)
                End If

                ' Cancel the startup process and exit the application.
                e.Cancel = True
                Exit Sub
            End If
        End Sub
    End Class
End Namespace

