Imports StarMicronics.StarIO
Imports System.Text

Public Class StarIOPrinter
    Private PortName As String

    Sub New(portName As String)
        Me.PortName = portName
    End Sub

    Public Function GetStatus() As StarPrinterStatus
        Dim Port As IPort

        Try
            Port = Factory.I.GetPort(Me.PortName, "", 10000)
            Return Port.GetParsedStatus()
        Finally
            If Port IsNot Nothing Then
                Factory.I.ReleasePort(Port)
            End If
        End Try
    End Function

    Public Sub Print(prnData As String)
        Dim isOnline As Boolean
        Dim Port As IPort

        Try
            Port = Factory.I.GetPort(Me.PortName, "", 10000)
            isOnline = Port.GetOnlineStatus()

            If (Not isOnline) Then
                Throw New PortException("The printer is offline")
            Else
                Dim dataByteArray() As Byte = Encoding.GetEncoding("Windows-1252").GetBytes(prnData)
                Dim amountWritten As UInteger = 0
                Dim amountWrittenKeep As UInteger
                While dataByteArray.Length > amountWritten
                    amountWrittenKeep = amountWritten
                    amountWritten += Port.WritePort(dataByteArray, amountWritten, CUInt(dataByteArray.Length) - amountWritten)
                    If amountWrittenKeep = amountWritten Then
                        Throw New PortException("Can't send data")
                    End If
                End While

                If amountWritten <> dataByteArray.Length Then
                    Throw New PortException("All data was not sent")
                End If
            End If
        Finally
            If Port IsNot Nothing Then
                Factory.I.ReleasePort(Port)
            End If
        End Try
    End Sub
End Class
