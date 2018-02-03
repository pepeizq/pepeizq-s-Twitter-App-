﻿Imports Microsoft.Toolkit.Uwp.Helpers
Imports pepeizq.Twitter
Imports pepeizq.Twitter.Tweet
Imports Windows.System

Module InicioXaml

    Public Function Generar(megaUsuario As pepeizq.Twitter.MegaUsuario, visibilidad As Visibility)

        Dim usuario As TwitterUsuario = megaUsuario.Usuario

        Dim gridTweets As New Grid
        gridTweets.SetValue(Grid.RowProperty, 1)
        gridTweets.Name = "gridTweets" + usuario.ScreenNombre
        gridTweets.Visibility = visibilidad

        Dim color1 As New GradientStop With {
            .Color = ColorHelper.ToColor("#e0e0e0"),
            .Offset = 0.5
        }

        Dim color2 As New GradientStop With {
            .Color = ColorHelper.ToColor("#d6d6d6"),
            .Offset = 1.0
        }

        Dim coleccion As New GradientStopCollection From {
            color1,
            color2
        }

        Dim brush As New LinearGradientBrush With {
            .StartPoint = New Point(0.5, 0),
            .EndPoint = New Point(0.5, 1),
            .GradientStops = coleccion
        }

        gridTweets.Background = brush

        Dim rowTweets1 As New RowDefinition
        Dim rowTweets2 As New RowDefinition

        rowTweets1.Height = New GridLength(1, GridUnitType.Star)
        rowTweets2.Height = New GridLength(1, GridUnitType.Auto)

        gridTweets.RowDefinitions.Add(rowTweets1)
        gridTweets.RowDefinitions.Add(rowTweets2)

        '---------------------------------

        Dim prTweets As New ProgressRing
        prTweets.SetValue(Grid.RowProperty, 0)
        prTweets.IsActive = True
        prTweets.Width = 50
        prTweets.Height = 50
        prTweets.Foreground = New SolidColorBrush(App.Current.Resources("ColorPrimario"))
        prTweets.VerticalAlignment = VerticalAlignment.Center
        prTweets.HorizontalAlignment = HorizontalAlignment.Center
        prTweets.Name = "prTweets" + usuario.ScreenNombre

        gridTweets.Children.Add(prTweets)

        '---------------------------------

        Dim svTweets As New ScrollViewer
        svTweets.SetValue(Grid.RowProperty, 0)
        AddHandler svTweets.ViewChanging, AddressOf SvTweets_ViewChanging

        gridTweets.Children.Add(svTweets)

        '---------------------------------

        Dim lvTweets As New ListView
        lvTweets.SetValue(Grid.RowProperty, 0)
        lvTweets.IsItemClickEnabled = True
        lvTweets.ItemContainerStyle = App.Current.Resources("ListViewEstilo1")
        lvTweets.Tag = usuario
        AddHandler lvTweets.ItemClick, AddressOf LvTweets_ItemClick

        svTweets.Content = lvTweets

        '---------------------------------

        Dim pbTweets As New ProgressBar
        pbTweets.SetValue(Grid.RowProperty, 1)
        pbTweets.IsIndeterminate = True
        pbTweets.Foreground = New SolidColorBrush(App.Current.Resources("ColorPrimario"))
        pbTweets.Visibility = Visibility.Collapsed
        pbTweets.Margin = New Thickness(10, 10, 10, 10)
        pbTweets.Padding = New Thickness(10, 10, 10, 10)
        pbTweets.Name = "pbTweets" + usuario.ScreenNombre

        gridTweets.Children.Add(pbTweets)

        svTweets.Tag = New pepeizq.Twitter.Objetos.ScrollViewerTweets(megaUsuario, prTweets, pbTweets, 0)

        '---------------------------------

        Return gridTweets

    End Function

    Public Async Sub SvTweets_ViewChanging(sender As Object, e As ScrollViewerViewChangingEventArgs)

        Dim sv As ScrollViewer = sender
        Dim cosas As pepeizq.Twitter.Objetos.ScrollViewerTweets = sv.Tag

        Dim pr As ProgressRing = cosas.Anillo
        Dim pb As ProgressBar = cosas.Barra

        Dim lv As ListView = sv.Content
        lv.Tag = cosas.MegaUsuario

        If pb.Visibility = Visibility.Collapsed Then
            If (sv.ScrollableHeight - 200) < sv.VerticalOffset Then
                Dim mostrar As Boolean = False

                If pr Is Nothing Then
                    mostrar = True
                Else
                    If pr.IsActive = False Then
                        mostrar = True
                    End If
                End If

                If mostrar = True Then
                    Dim lvItem As ListViewItem = lv.Items(lv.Items.Count - 1)
                    Dim gridTweet As Grid = lvItem.Content
                    Dim ultimoTweet As Tweet = gridTweet.Tag

                    If lv.Items.Count > 0 And lv.Items.Count < 280 Then
                        If Not ultimoTweet.ID = Nothing Then
                            Dim provider As TwitterDataProvider = cosas.MegaUsuario.Servicio.Provider
                            Dim listaTweets As New List(Of Tweet)

                            Try
                                If cosas.Query = 0 Then
                                    listaTweets = Await provider.CogerTweetsTimelineInicio(Of Tweet)(ultimoTweet.ID, New TweetParser)
                                ElseIf cosas.Query = 1 Then
                                    listaTweets = Await provider.CogerTweetsTimelineMenciones(Of Tweet)(ultimoTweet.ID, New TweetParser)
                                End If
                            Catch ex As Exception

                            End Try

                            If listaTweets.Count > 0 Then
                                For Each tweet In listaTweets
                                    Dim boolAñadir As Boolean = True

                                    For Each item In lv.Items
                                        Dim lvItem2 As ListViewItem = item
                                        Dim gridTweet2 As Grid = lvItem2.Content
                                        Dim lvTweet As Tweet = gridTweet2.Tag

                                        If lvTweet.ID = tweet.ID Then
                                            boolAñadir = False
                                        End If
                                    Next

                                    If boolAñadir = True Then
                                        lv.Items.Add(TweetXaml.Añadir(tweet, cosas.MegaUsuario))
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Public Async Sub LvTweets_ItemClick(sender As Object, e As ItemClickEventArgs)

        Dim grid As Grid = e.ClickedItem
        Dim tweet As Tweet = grid.Tag

        If Not tweet.Entidades.Enlaces Is Nothing Then
            'Try
            '    Await Launcher.LaunchUriAsync(New Uri(tweet.Entidades.Enlaces(0).Expandida))
            'Catch ex As Exception

            'End Try
        End If

    End Sub


End Module
