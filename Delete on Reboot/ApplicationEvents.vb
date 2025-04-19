Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Private Function ParseArguments(args As ObjectModel.ReadOnlyCollection(Of String)) As Dictionary(Of String, Object)
            Dim parsedArguments As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            Dim strValue As String

            For Each strArgument As String In args
                If strArgument.StartsWith("--") Then
                    Dim splitArg As String() = strArgument.Substring(2).Split(New Char() {"="c}, 2)
                    Dim key As String = splitArg(0)

                    If splitArg.Length = 2 Then
                        ' Argument with a value
                        strValue = splitArg(1)
                        parsedArguments(key) = strValue
                    Else
                        ' Boolean flag
                        parsedArguments(key) = True
                    End If
                Else
                    Console.WriteLine($"Unrecognized argument format: {strArgument}")
                End If
            Next

            Return parsedArguments
        End Function

        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            If Application.CommandLineArgs.Count = 1 Then
                Dim parsedArguments As Dictionary(Of String, Object) = ParseArguments(Application.CommandLineArgs)

                If parsedArguments.ContainsKey("delete") AndAlso IO.File.Exists(parsedArguments("delete")) Then
                    Dim deleteAtRebootInstance As New deleteAtReboot()
                    deleteAtRebootInstance.addItem(parsedArguments("delete"))
                    deleteAtRebootInstance.dispose(True)
                End If

                e.Cancel = True
                Exit Sub
            End If
        End Sub
    End Class
End Namespace

