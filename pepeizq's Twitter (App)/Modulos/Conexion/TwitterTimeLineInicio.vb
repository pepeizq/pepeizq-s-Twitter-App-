﻿Imports pepeizq.Twitter
Imports pepeizq.Twitter.Tweet

Module TwitterTimeLineInicio

    Dim intentosCarga As Integer = 0

    Public Async Sub CargarTweets(megaUsuario As pepeizq.Twitter.MegaUsuario, ultimoTweet As String, limpiar As Boolean)

        Dim usuario As TwitterUsuario = megaUsuario.Usuario2.Usuario

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gridTweets As Grid = pagina.FindName("gridTweets" + usuario.ScreenNombre)

        If Not gridTweets Is Nothing Then
            Dim pr As ProgressRing = gridTweets.Children(0)
            Dim pb As ProgressBar = gridTweets.Children(3)

            If Not ultimoTweet = Nothing Then
                pb.Visibility = Visibility.Visible
            End If

            '-------------------------------

            Dim sv As ScrollViewer = gridTweets.Children(1)
            Dim lv As ListView = sv.Content

            If limpiar = True Then
                lv.Items.Clear()
            End If

            Dim provider As TwitterDataProvider = megaUsuario.Servicio.Provider
            Dim listaTweets As New List(Of Tweet)

            Try
                listaTweets = Await provider.CogerTweetsTimelineInicio(Of Tweet)(megaUsuario.Usuario2.Usuario.Tokens, ultimoTweet, New TweetParser)
            Catch ex As Exception

            End Try

            If listaTweets.Count = 0 Then

                pr.IsActive = True

                Await Task.Delay(20000)

                intentosCarga = intentosCarga + 1
                pr.IsActive = False

                If intentosCarga < 500 Then
                    TwitterTimeLineInicio.CargarTweets(megaUsuario, ultimoTweet, False)
                Else
                    TwitterConexion.Desconectar(megaUsuario.Servicio)

                    Dim megaUsuarioNuevo As pepeizq.Twitter.MegaUsuario = Await TwitterConexion.Iniciar(megaUsuario.Usuario2)

                    If Not megaUsuarioNuevo Is Nothing Then
                        TwitterTimeLineInicio.CargarTweets(megaUsuarioNuevo, Nothing, False)

                        intentosCarga = 0
                    End If
                End If
            Else
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

                pr.IsActive = False

                If Not ultimoTweet = Nothing Then
                    pb.Visibility = Visibility.Collapsed
                End If
            End If
        End If

    End Sub

End Module