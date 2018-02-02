﻿Imports System.Net
Imports pepeizq.Twitter.Tweet
Imports Windows.System
Imports Windows.UI.Xaml.Documents

Namespace pepeTwitterXaml
    Module TweetXamlTexto

        Public Function Generar(tweet As Tweet, citaOrigen As Tweet)

            Dim textoSpan As New Span
            Dim textoTweet As String = Nothing

            If tweet.Retweet Is Nothing Then
                textoTweet = tweet.Texto

                textoTweet = textoTweet.Remove(tweet.TextoRango(1), textoTweet.Length - tweet.TextoRango(1))
                textoTweet = textoTweet.Remove(0, tweet.TextoRango(0))
            Else
                textoTweet = tweet.Retweet.Texto

                textoTweet = textoTweet.Remove(tweet.Retweet.TextoRango(1), textoTweet.Length - tweet.Retweet.TextoRango(1))
                textoTweet = textoTweet.Remove(0, tweet.Retweet.TextoRango(0))
            End If

            If Not textoTweet = String.Empty Then
                Dim entidades As New TweetEntidad

                If tweet.Retweet Is Nothing Then
                    entidades = tweet.Entidades
                Else
                    entidades = tweet.Retweet.Entidades
                End If

                Dim listaEntidades As New List(Of pepeTwitter.Objetos.TextoTweetEntidad)

                If entidades.Enlaces.Count > 0 Then
                    For Each url In entidades.Enlaces
                        Dim coordenadas As New List(Of Integer) From {
                           url.Rango(0),
                           url.Rango(1)
                        }

                        listaEntidades.Add(New pepeTwitter.Objetos.TextoTweetEntidad(url.Mostrar, url.Expandida, coordenadas, 0))
                    Next
                End If

                If entidades.Menciones.Count > 0 Then
                    For Each mencion In entidades.Menciones
                        Dim coordenadas As New List(Of Integer) From {
                            mencion.Rango(0),
                            mencion.Rango(1)
                        }

                        listaEntidades.Add(New pepeTwitter.Objetos.TextoTweetEntidad("@" + mencion.ScreenNombre, "pepeizqtwitter://@" + mencion.ScreenNombre, coordenadas, 1))
                    Next
                End If

                If entidades.Hashtags.Count > 0 Then
                    For Each tag In entidades.Hashtags
                        Dim coordenadas As New List(Of Integer) From {
                            tag.Rango(0),
                            tag.Rango(1)
                        }

                        listaEntidades.Add(New pepeTwitter.Objetos.TextoTweetEntidad("#" + tag.Nombre, "https://twitter.com/#" + tag.Nombre, coordenadas, 2))
                    Next
                End If

                listaEntidades.Sort(Function(x, y) x.Coordenadas(0).CompareTo(y.Coordenadas(0)))

                If listaEntidades.Count > 0 Then
                    For Each entidad In listaEntidades
                        If entidad.Tipo = 0 Then
                            If textoTweet.Contains("https://") Then
                                Dim int As Integer = textoTweet.IndexOf("https://")
                                Dim temp As String = textoTweet.Remove(0, int)

                                Dim int2 As Integer = temp.IndexOf(" ")

                                If int2 = -1 Then
                                    int2 = temp.Length
                                End If

                                Dim tbTextoAnterior As String = WebUtility.HtmlDecode(textoTweet)

                                If tbTextoAnterior.Trim.Length > 0 Then
                                    Dim fragmentoAnterior As New Run With {
                                       .Text = tbTextoAnterior.Remove(int, tbTextoAnterior.Length - int)
                                   }

                                    textoSpan.Inlines.Add(fragmentoAnterior)
                                End If

                                textoTweet = textoTweet.Remove(0, int + int2)

                                Dim contenidoEnlace As New Run With {
                                    .Text = entidad.Mostrar
                                }

                                Dim enlace As New Hyperlink With {
                                    .NavigateUri = New Uri(entidad.Enlace),
                                    .TextDecorations = Nothing,
                                    .Foreground = New SolidColorBrush(App.Current.Resources("ColorCuarto"))
                                }

                                enlace.Inlines.Add(contenidoEnlace)
                                textoSpan.Inlines.Add(enlace)
                            End If
                        End If

                        If entidad.Tipo = 1 Or entidad.Tipo = 2 Then
                            If textoTweet.ToLower.Contains(entidad.Mostrar.ToLower) Then
                                textoTweet = WebUtility.HtmlDecode(textoTweet)

                                Dim int As Integer = textoTweet.ToLower.IndexOf(entidad.Mostrar.ToLower)

                                Dim fragmentoAnterior As New Run With {
                                    .Text = textoTweet.Remove(int, textoTweet.Length - int)
                                }

                                textoSpan.Inlines.Add(fragmentoAnterior)

                                textoTweet = textoTweet.Remove(0, int + entidad.Mostrar.Length)

                                Dim contenidoEnlace As New Run With {
                                    .Text = entidad.Mostrar
                                }

                                Dim enlace As New Hyperlink With {
                                    .NavigateUri = New Uri(entidad.Enlace, UriKind.RelativeOrAbsolute),
                                    .TextDecorations = Nothing,
                                    .Foreground = New SolidColorBrush(App.Current.Resources("ColorCuarto"))
                                }

                                AddHandler enlace.Click, AddressOf EnlaceClick

                                enlace.Inlines.Add(contenidoEnlace)
                                textoSpan.Inlines.Add(enlace)
                            End If
                        End If
                    Next
                End If

                If textoTweet.Trim.Length > 0 Then
                    textoTweet = LimpiarEnlaces(textoTweet)

                    If textoTweet.Trim.Length > 0 Then
                        Dim fragmento As New Run With {
                           .Text = WebUtility.HtmlDecode(textoTweet)
                        }

                        textoSpan.Inlines.Add(fragmento)
                    End If
                End If

                If textoSpan.Inlines.Count > 0 Then
                    Dim tbTweet As New TextBlock With {
                        .TextWrapping = TextWrapping.Wrap,
                        .Margin = New Thickness(5, 10, 5, 5)
                    }

                    tbTweet.Inlines.Add(textoSpan)

                    Return tbTweet
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If

        End Function

        Private Function LimpiarEnlaces(texto As String)

            Dim i As Integer = 0
            While i < 5
                If texto.Contains("https://") Then
                    Dim temp As String
                    Dim int, int2 As Integer

                    int = texto.IndexOf("https://")
                    temp = texto.Remove(0, int)

                    int2 = temp.IndexOf(" ")

                    If int2 = -1 Then
                        int2 = temp.Length
                    End If

                    texto = texto.Remove(int, int2)
                End If
                i += 1
            End While

            Return texto

        End Function

        Private Async Sub EnlaceClick(sender As Object, e As HyperlinkClickEventArgs)
            Dim test As Hyperlink = e.OriginalSource
            Dim enlace As Hyperlink = sender
            Dim enlaceString As String = enlace.NavigateUri.ToString

            If enlaceString.IndexOf("pepeizqtwitter://@") = 0 Then

            Else
                Try
                    Await Launcher.LaunchUriAsync(New Uri(enlaceString))
                Catch ex As Exception

                End Try
            End If

        End Sub

    End Module
End Namespace

