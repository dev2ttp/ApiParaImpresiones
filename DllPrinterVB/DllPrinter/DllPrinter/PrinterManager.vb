Imports Microsoft.VisualBasic
Public Class PrinterManager
    Private response As PrintResponseModel = New PrintResponseModel()
    Public Function PrintVoucher(Ticket As String, tipoImpresora As String, nombreImpresora As String) As PrintResponseModel
        If tipoImpresora.ToUpper() = "TUP992" Then
            'Default -> USBPRN:Star TUP900 Presenter (TUP992)
            Try
                Dim i As Integer
                Dim sTck As String = ""
                Dim printer As New StarIOPrinter("USBPRN:" + nombreImpresora)
                Dim linesToFeed As String = [Char].ConvertFromUtf32(6)

                For i = 0 To Ticket.Length Step 40
                    If i + 40 > Ticket.Length Then 'fin de ticket
                        sTck &= Ticket.Substring(i)
                    Else
                        sTck &= Ticket.Substring(i, 40) & vbLf
                    End If
                Next

                printer.Print(Chr(27) & Chr(29) & "a" & Chr(1)) 'Center
                printer.Print(sTck)                             'Ticket
                printer.Print(Chr(27) & "a" & linesToFeed)      'Line feed
                printer.Print(Chr(27) & "d" & vbNullChar)       'Full cut

                response.setResponse(True)
                response.setGlosa("Voucher impreso")

                Return response

            Catch ex As Exception
                response.setResponse(False)
                response.setGlosa(ex.ToString())

                Return response
            End Try
        Else
            Try
                Dim i As Integer
                Dim sTck, sAux(2) As String
                Dim ret, jid As Integer
                Dim nii As New NiiPrinterCLib.NIIClassLib()
                Dim sNombre As String = nombreImpresora

                sTck = ""
                sTck &= Chr(&H1B) & Chr(&H32) & vbLf              'set pitch default
                sTck &= Chr(&H1B) & Chr(&H74) & Chr(&H0)          'no Japan code

                For i = 0 To Ticket.Length Step 40
                    If i + 40 > Ticket.Length Then 'fin de ticket
                        sTck &= Ticket.Substring(i)
                    Else
                        sTck &= Ticket.Substring(i, 40) & vbCrLf
                    End If
                Next

                sTck &= Chr(&H1B) & Chr(&H64) & Chr(&H4)          '6LF
                sTck &= Chr(&H1B) & "i"                           'Corte Total

                ret = nii.NiiStartDoc(sNombre, jid)
                If ret = 0 Then
                    sAux(0) = ""
                    For i = 1 To Len(sTck)
                        sAux(0) = sAux(0) & CHGHEX2(Hex(Asc(Mid(sTck, i, 1))))
                    Next i
                    ret = nii.NiiPrint(sNombre, sAux(0), Len(sAux(0)), 0)
                    ret = nii.NiiEndDoc(sNombre)

                    response.setResponse(True)
                    response.setGlosa("Voucher impreso")

                    Return response
                Else
                    response.setResponse(False)
                    response.setGlosa("Documento no impreso")

                    Return response
                End If
            Catch ex As Exception
                response.setResponse(False)
                response.setGlosa(ex.ToString())

                Return response
            End Try
        End If
    End Function

    Public Function PrintTicket(tipoImpresora As String, ticket As String, nombreImpresora As String) As PrintResponseModel
        Dim i As Integer
        Dim sTck, sAux(2) As String
        Dim ret, jid As Long

        'Imprime Ticket
        '0,letra 1,turno 2,ticket 3,fecha(dd/mm/yyyy) 4,hora(hh:nn) 5,TEsp(min)

        sTck = ""
        If tipoImpresora = "TUP992" Then
            sTck &= ControlCharToPrinterChar(New Ticket() With {.Ticket = ticket}, tipoImpresora)
            Dim printer As New StarIOPrinter("USBPRN:" & nombreImpresora)
            Dim linesToFeed As String = [Char].ConvertFromUtf32(3)
            printer.Print(sTck)                             'Ticket
            'printer.Print(Chr(27) & "a" & linesToFeed)      'Line feed
            printer.Print(Chr(27) & "d" & vbNullChar)       'Full cut
        Else
            sTck &= Chr(&H1B) & Chr(&H32) & vbLf              'set pitch default
            sTck &= Chr(&H1B) & Chr(&H74) & Chr(&H0)          'no Japan code
            sTck &= ControlCharToPrinterChar(New Ticket() With {.Ticket = ticket}, tipoImpresora)
            sTck = LatinCharToPrinterChar(sTck)
            sTck &= Chr(&H1B) & Chr(&H64) & Chr(&H6)  '6LF
            sTck &= Chr(&H1B) & "i"          'guillotina

            'Impresora USB
            Dim nii As New NiiPrinterCLib.NIIClassLib()
            ret = nii.NiiStartDoc(nombreImpresora, jid)
            If ret = 0 Then
                sAux(0) = ""
                For i = 1 To Len(sTck)
                    sAux(0) = sAux(0) & CHGHEX2(Hex(Asc(Mid(sTck, i, 1))))
                Next i
                ret = nii.NiiPrint(nombreImpresora, sAux(0), Len(sAux(0)), 0)
                ret = nii.NiiEndDoc(nombreImpresora)

                response.setResponse(True)
                response.setGlosa("Voucher impreso")
                Return response
            Else
                response.setResponse(False)
                response.setGlosa("ticket no impreso")
                Return response
            End If
        End If
    End Function

    Private Function CHGHEX2(dat As String) As String
        CHGHEX2 = If(Len(dat) = 1, "0", "") & dat
    End Function

    Private Function LatinCharToPrinterChar(text As String) As String
        text = Replace(text, "á", Chr(&HA0))
        text = Replace(text, "é", Chr(&H82))
        text = Replace(text, "í", Chr(&HA1))
        text = Replace(text, "ó", Chr(&HA2))
        text = Replace(text, "ú", Chr(&HA3))
        text = Replace(text, "ñ", Chr(&HA4))
        text = Replace(text, "Á", "A")
        text = Replace(text, "É", "E")
        text = Replace(text, "Í", "I")
        text = Replace(text, "Ó", "O")
        text = Replace(text, "Ú", "U")
        text = Replace(text, "Ñ", Chr(&HA5))
        Return text
    End Function

    Public Function ControlCharToPrinterChar(ByVal ticket As Ticket, tipoImpresora As String) As String
        Dim i, j As Integer
        Dim bControl, bHexa As Boolean
        Dim sTck As String = ""

        If tipoImpresora = "TUP992" Then
            Dim largoRaya As Byte = 16
            Dim alignLeft As String = ChrW(27) & ChrW(29) & "a" & vbNullChar
            Dim alignCenter As String = ChrW(27) & ChrW(29) & "a" & ChrW(1)
            Dim alignRight As String = ChrW(27) & ChrW(29) & "a" & ChrW(2)
            Dim fontA As String = ChrW(27) & ChrW(30) & "F" & vbNullChar
            Dim fontB As String = ChrW(27) & ChrW(30) & "F" & ChrW(1)
            Dim boldOn As String = ChrW(27) & "E"
            Dim boldOff As String = ChrW(27) & "F"
            Dim widthExpansion As String = ChrW(27) & "W"
            Dim heightExpansion As String = ChrW(27) & "h"
            Dim fullCut As String = ChrW(27) & "d" & vbNullChar
            Dim endCodePage As String = vbLf
            Dim latinEncoding As String = "1252"
            Dim linesToFeed As String = ChrW(27) & "a" & Char.ConvertFromUtf32(6)
            Dim height As String = Char.ConvertFromUtf32(1)
            Dim width As String = Char.ConvertFromUtf32(1)

            For i = 1 To Len(ticket.Ticket)
                bControl = False
                If Not bHexa Or (bHexa And Mid(ticket.Ticket, i, 3) = "\91") Then
                    If Mid(ticket.Ticket, i, 1) = "\" And Len(ticket.Ticket) >= i + 2 Then
                        bControl = True
                        Select Case Mid(ticket.Ticket, i + 1, 2)
                            Case "01"
                                sTck &= alignLeft
                            Case "02"
                                sTck &= alignCenter
                            Case "03"
                                sTck &= alignRight
                            Case "04"
                                sTck &= fontA & (widthExpansion & 0) & (heightExpansion & 0) & boldOff
                            Case "05"
                                sTck &= fontA & (widthExpansion & width) & (heightExpansion & 0) & boldOff
                            Case "06"
                                sTck &= fontA & (widthExpansion & 0) & (heightExpansion & height) & boldOff
                            Case "07"
                                sTck &= fontA & (widthExpansion & width) & (heightExpansion & height) & boldOff
                            Case "08"
                                sTck &= fontA & (widthExpansion & 0) & (heightExpansion & 0) & boldOn
                            Case "09"
                                sTck &= fontA & (widthExpansion & width) & (heightExpansion & 0) & boldOn
                            Case "10"
                                sTck &= fontA & (widthExpansion & 0) & (heightExpansion & height) & boldOn
                            Case "11"
                                sTck &= fontA & (widthExpansion & width) & (heightExpansion & height) & boldOn
                            Case "12"
                                sTck &= fontB & (widthExpansion & 0) & (heightExpansion & 0) & boldOff
                            Case "13"
                                sTck &= fontB & (widthExpansion & width) & (heightExpansion & 0) & boldOff
                            Case "14"
                                sTck &= fontB & (widthExpansion & 0) & (heightExpansion & height) & boldOff
                            Case "15"
                                sTck &= fontB & (widthExpansion & width) & (heightExpansion & height) & boldOff
                            Case "16"
                                sTck &= fontB & (widthExpansion & 0) & (heightExpansion & 0) & boldOn
                            Case "17"
                                sTck &= fontB & (widthExpansion & width) & (heightExpansion & 0) & boldOn
                            Case "18"
                                sTck &= fontB & (widthExpansion & 0) & (heightExpansion & height) & boldOn
                            Case "19"
                                sTck &= fontB & (widthExpansion & width) & (heightExpansion & height) & boldOn
                            Case "21"
                                sTck &= ticket.Letra & " " + ticket.Turno
                            Case "22"
                                sTck &= ticket.Turno
                            Case "23"
                                sTck &= ticket.Letra & " " + ticket.Turno
                            Case "24"
                                sTck &= ticket.Turno
                            Case "25"
                                sTck &= ticket.Fecha
                            Case "26"
                                sTck &= ticket.Hora
                            Case "27"
                                If ticket.TEspE > 0 Then sTck &= "Su espera aprox. es de: " & ticket.TEspE & " min."
                            Case "28"
                                sTck &= New String("="c, largoRaya)
                            Case "29"
                                sTck &= New String("-"c, largoRaya)
                            Case "90"
                                bHexa = True
                            Case "91"
                                bHexa = False
                            Case Else
                                bControl = False
                        End Select
                        If bControl Then i = i + 2
                    End If
                End If
                If bHexa And Not bControl And Len(ticket.Ticket) >= i + 1 Then
                    sTck &= Chr(CInt("&H" & Mid(ticket.Ticket, i, 2)))
                    i = i + 1
                End If
                If Not bHexa And Not bControl Then sTck &= Mid$(ticket.Ticket, i, 1)
            Next
            sTck &= linesToFeed

        Else
            For i = 1 To Len(ticket.Ticket)
                bControl = False
                If Not bHexa Or (bHexa And Mid(ticket.Ticket, i, 3) = "\91") Then
                    If Mid(ticket.Ticket, i, 1) = "\" And Len(ticket.Ticket) >= i + 2 Then
                        bControl = True
                        Select Case Mid(ticket.Ticket, i + 1, 2)
                            Case "01"   'align Izquierda
                                sTck &= Chr(&H1B) & Chr(&H61) & Chr(&H0)
                            Case "02"   'align Centro
                                sTck &= Chr(&H1B) & Chr(&H61) & Chr(&H1)
                            Case "03"   'align Derecha
                                sTck &= Chr(&H1B) & Chr(&H61) & Chr(&H2)
                            Case "04"   'font A
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H2)
                            Case "05"   'font A Ancho
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H20)
                            Case "06"   'font A Alto
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H10)
                            Case "07"   'font A Ancho-Alto
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H30)
                            Case "08"   'font A Negrita
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H8)
                            Case "09"   'font A Negrita Ancho
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H28)
                            Case "10"   'font A Negrita Alto
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H18)
                            Case "11"   'font A Negrita Ancho-Alto
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H38)
                            Case "12"   'font B
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H1)
                            Case "13"   'font B Ancho
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H21)
                            Case "14"   'font B Alto
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H11)
                            Case "15"   'font B Ancho-Alto
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H31)
                            Case "16"   'font B Negrita
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H9)
                            Case "17"   'font B Negrita Ancho
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H29)
                            Case "18"   'font B Negrita Alto
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H19)
                            Case "19"   'font B Negrita Ancho-Alto
                                sTck &= Chr(&H1B) & Chr(&H21) & Chr(&H39)
                            Case "21"   'Letra Turno (texto)
                                sTck &= ticket.Letra & " " & ticket.Turno
                            Case "22"   'Turno (texto)
                                sTck &= ticket.Turno
                            Case "23"   'Letra Turno (gráfico)
                                sTck &= ticket.Turno
                            Case "25"   'Fecha
                                sTck &= DateTime.Now.ToString("dd-MM-yyyy")
                            Case "26"   'Hora
                                sTck &= DateTime.Now.ToString("HH:mm")
                            Case "27"   'Tiempo de Espera
                                '    If CInt(ticket.TEspE) > 0 And CInt(vDat(5)) < giEspera Then _
                                sTck &= "Su espera aprox. es de: " & "5" & " min."
                            Case "28"   'Raya doble
                                For j = 1 To 10
                                    sTck &= Chr(&HCD)
                                Next j
                            Case "29"   'Raya simple
                                For j = 1 To 10
                                    sTck &= Chr(&HC4)
                                Next j
                            Case "90"   'Hexa ini
                                bHexa = True
                            Case "91"   'Hexa fin
                                bHexa = False
                            Case Else
                                bControl = False
                        End Select
                        If bControl Then i = i + 2
                    End If
                End If
                If bHexa And Not bControl And Len(ticket.Ticket) >= i + 1 Then
                    sTck &= Chr(CInt("&H" & Mid(ticket.Ticket, i, 2)))
                    i = i + 1
                End If
                If Not bHexa And Not bControl Then sTck &= Mid$(ticket.Ticket, i, 1)
            Next i
        End If

        Return sTck
    End Function

    Public Structure Ticket
        Dim Letra As String
        Dim Turno As String
        Dim Ticket As String
        Dim Fecha As String
        Dim Hora As String
        Dim TEspE As String
    End Structure
End Class