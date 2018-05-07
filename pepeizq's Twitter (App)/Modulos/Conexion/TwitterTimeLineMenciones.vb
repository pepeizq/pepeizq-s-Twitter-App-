﻿Imports pepeizq.Twitter
Imports pepeizq.Twitter.Tweet

Module TwitterTimeLineMenciones

    Public Async Sub CargarTweets(megaUsuario As pepeizq.Twitter.MegaUsuario, ultimoTweet As String, limpiar As Boolean)

        Dim usuario As TwitterUsuario = megaUsuario.Usuario2.Usuario

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gridPrincipal As Grid = pagina.FindName("gridPrincipal")
        Dim gridUsuario As New Grid

        Dim gridTweets As Grid = pagina.FindName("gridMenciones" + usuario.ScreenNombre)

        If Not gridTweets Is Nothing Then
            Dim sv As ScrollViewer = gridTweets.Children(0)
            Dim lv As ListView = sv.Content

            If limpiar = True Then
                lv.Items.Clear()
            End If

            Dim provider As TwitterDataProvider = megaUsuario.Servicio.Provider
            Dim listaTweets As New List(Of Tweet)

            Try
                listaTweets = Await provider.CogerTweetsTimelineMenciones(Of Tweet)(megaUsuario.Usuario2.Usuario.Tokens, ultimoTweet, New TweetParser)
            Catch ex As Exception

            End Try

            If listaTweets.Count > 0 Then
                For Each tweet In listaTweets
                    Dim boolAñadir As Boolean = True

                    For Each item In lv.Items
                        Dim lvItem As ListViewItem = item
                        Dim gridTweet As Grid = lvItem.Content
                        Dim tweetAmpliado As pepeizq.Twitter.Objetos.TweetAmpliado = gridTweet.Tag
                        Dim lvTweet As Tweet = tweetAmpliado.Tweet

                        If lvTweet.ID = tweet.ID Then
                            boolAñadir = False
                        End If
                    Next

                    If boolAñadir = True Then
                        lv.Items.Add(pepeizq.Twitter.Xaml.TweetXaml.Añadir(tweet, megaUsuario, Nothing))
                    End If
                Next
            End If
        End If

    End Sub

End Module