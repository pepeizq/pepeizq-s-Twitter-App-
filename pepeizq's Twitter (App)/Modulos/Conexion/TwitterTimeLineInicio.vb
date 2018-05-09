﻿Imports Microsoft.Advertising.WinRT.UI
Imports Microsoft.Toolkit.Uwp.Helpers
Imports pepeizq.Twitter
Imports pepeizq.Twitter.Tweet
Imports Windows.ApplicationModel.Store
Imports Windows.System
Imports Windows.UI
Imports Windows.UI.Core

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
                    Dim boolAñadirTweet As Boolean = True

                    For Each item In lv.Items
                        If TypeOf item Is ListViewItem Then
                            Dim lvItem As ListViewItem = item
                            Dim gridTweet As Grid = lvItem.Content
                            Dim tweetAmpliado As pepeizq.Twitter.Objetos.TweetAmpliado = gridTweet.Tag
                            Dim lvTweet As Tweet = tweetAmpliado.Tweet

                            If lvTweet.ID = tweet.ID Then
                                boolAñadirTweet = False
                            End If
                        End If
                    Next

                    If boolAñadirTweet = True Then
                        lv.Items.Add(pepeizq.Twitter.Xaml.TweetXaml.Añadir(tweet, megaUsuario, Nothing))
                    End If
                Next

                Dim licencia As LicenseInformation = Nothing

                Try
                    licencia = CurrentApp.LicenseInformation
                Catch ex As Exception

                End Try

                If Not licencia Is Nothing Then
                    If Not licencia.ProductLicenses("NoAds").IsActive Then
                        lv.Items.Insert(1, AñadirAnuncio("1100022916"))
                        lv.Items.Insert(4, AñadirAnuncio("1100022920"))
                        lv.Items.Insert(8, AñadirAnuncio("1100022962"))
                    End If
                Else
                    lv.Items.Insert(1, AñadirAnuncio("1100022916"))
                    lv.Items.Insert(4, AñadirAnuncio("1100022920"))
                    lv.Items.Insert(8, AñadirAnuncio("1100022962"))
                End If

                pr.IsActive = False

                If Not ultimoTweet = Nothing Then
                    pb.Visibility = Visibility.Collapsed
                End If
            End If
        End If

    End Sub

    Public Function AñadirAnuncio(id As String)

        Dim gridAnuncio As New Grid With {
            .Name = "gridAnuncio" + id,
            .Padding = New Thickness(10, 10, 10, 10)
        }

        Dim col1 As New ColumnDefinition
        Dim col2 As New ColumnDefinition

        col1.Width = New GridLength(1, GridUnitType.Auto)
        col2.Width = New GridLength(1, GridUnitType.Star)

        gridAnuncio.ColumnDefinitions.Add(col1)
        gridAnuncio.ColumnDefinitions.Add(col2)

        Dim color1 As New GradientStop With {
            .Color = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToColor("#e0e0e0"),
            .Offset = 0.5
        }

        Dim color2 As New GradientStop With {
            .Color = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToColor("#d6d6d6"),
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

        gridAnuncio.Background = brush

        Dim anuncio As New AdControl With {
            .AdUnitId = id,
            .Width = 728,
            .Height = 90,
            .HorizontalAlignment = HorizontalAlignment.Center
        }

        anuncio.SetValue(Grid.ColumnProperty, 0)
        gridAnuncio.Children.Add(anuncio)

        Dim recursos As New Resources.ResourceLoader

        Dim tbBoton As New TextBlock With {
            .Text = recursos.GetString("ButtonRemoveAds"),
            .Foreground = New SolidColorBrush(Colors.White)
        }

        Dim boton As New Button With {
            .Margin = New Thickness(10, 0, 10, 0),
            .Padding = New Thickness(15, 10, 15, 10),
            .HorizontalAlignment = HorizontalAlignment.Left,
            .Content = tbBoton,
            .Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        }

        AddHandler boton.Click, AddressOf BotonMostrarAnunciosClick
        AddHandler boton.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler boton.PointerExited, AddressOf UsuarioSaleBoton

        boton.SetValue(Grid.ColumnProperty, 1)
        gridAnuncio.Children.Add(boton)

        Return gridAnuncio

    End Function

    Private Async Sub BotonMostrarAnunciosClick(sender As Object, e As RoutedEventArgs)

        Await Launcher.LaunchUriAsync(New Uri("ms-windows-store://pdp/?ProductId=9NVM8JPQ57VT"))

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

End Module
