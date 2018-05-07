﻿Imports FontAwesome.UWP
Imports Microsoft.Toolkit.Uwp.UI.Animations
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports pepeizq.Twitter.Tweet
Imports Windows.Media.Core
Imports Windows.UI
Imports Windows.UI.Core
Imports Windows.UI.Xaml.Media.Animation

Namespace pepeizq.Twitter.Xaml
    Module TweetMediaXaml

        Public Function Generar(tweet As Tweet, color As Color)

            If color = Nothing Then
                color = App.Current.Resources("ColorSecundario")
            End If

            Dim colorPlay As New Color

            If color = App.Current.Resources("ColorSecundario") Then
                colorPlay = App.Current.Resources("ColorPrimario")
            Else
                colorPlay = color
            End If

            If Not tweet.Entidades.Media Is Nothing Then
                Dim spMedia As New StackPanel With {
                    .Orientation = Orientation.Horizontal,
                    .Margin = New Thickness(5, 0, 0, 5)
                }

                Dim listaMedia() As TweetMedia

                If tweet.Retweet Is Nothing Then
                    listaMedia = tweet.EntidadesExtendida.Media

                    If listaMedia.Count = 0 Then
                        listaMedia = tweet.Entidades.Media
                    End If
                Else
                    listaMedia = tweet.Retweet.EntidadesExtendida.Media

                    If listaMedia.Count = 0 Then
                        listaMedia = tweet.Retweet.Entidades.Media
                    End If
                End If

                For Each itemMedia In listaMedia
                    Dim objetoString As String = Nothing

                    If itemMedia.Tipo = "photo" Then
                        objetoString = itemMedia.Tipo
                    ElseIf itemMedia.Tipo = "video" Then
                        objetoString = itemMedia.Tipo
                    ElseIf itemMedia.Tipo = "animated_gif" Then
                        objetoString = itemMedia.Tipo
                    End If

                    If Not objetoString = Nothing Then
                        Dim gridPlay As New Grid

                        If itemMedia.Tipo = "video" Or itemMedia.Tipo = "animated_gif" Then
                            gridPlay.Background = New SolidColorBrush(colorPlay)
                            gridPlay.MinHeight = 50
                            gridPlay.MinWidth = 50
                            gridPlay.CornerRadius = New CornerRadius(30)

                            Dim simboloPlay As New FontAwesome.UWP.FontAwesome With {
                                .Icon = FontAwesomeIcon.Play,
                                .Foreground = New SolidColorBrush(Colors.White)
                            }

                            gridPlay.Children.Add(simboloPlay)
                        End If

                        Dim gridMedia As New Grid With {
                            .BorderBrush = New SolidColorBrush(color),
                            .BorderThickness = New Thickness(1, 1, 1, 1),
                            .MaxHeight = 200,
                            .MaxWidth = 500,
                            .HorizontalAlignment = HorizontalAlignment.Left
                        }

                        Dim imagenMedia As New ImageEx With {
                            .Stretch = Stretch.Uniform,
                            .IsCacheEnabled = True
                        }

                        Dim imagenUrl As String = String.Empty

                        If Not itemMedia.EnlaceHttps = String.Empty Then
                            imagenUrl = itemMedia.EnlaceHttps
                        End If

                        If imagenUrl = String.Empty Then
                            imagenUrl = itemMedia.Enlace
                        End If

                        Try
                            imagenMedia.Source = New BitmapImage(New Uri(imagenUrl))
                        Catch ex As Exception

                        End Try

                        If itemMedia.Tipo = "video" Or itemMedia.Tipo = "animated_gif" Then
                            gridMedia.Background = New SolidColorBrush(Colors.Black)
                            imagenMedia.Opacity = 0.6
                        End If

                        gridMedia.BorderThickness = New Thickness(1, 1, 1, 1)
                        gridMedia.Margin = New Thickness(0, 10, 5, 0)

                        AddHandler gridMedia.PointerEntered, AddressOf UsuarioEntraMedia
                        AddHandler gridMedia.PointerExited, AddressOf UsuarioSaleMedia

                        Dim datos As New Objetos.MediaDatos(color, imagenUrl, imagenMedia)

                        Dim frame As Frame = Window.Current.Content
                        Dim pagina As Page = frame.Content

                        If itemMedia.Tipo = "photo" Then
                            AddHandler gridMedia.PointerPressed, AddressOf UsuarioClickeaImagen
                        ElseIf itemMedia.Tipo = "video" Then
                            Dim listaVideos As TweetVideoVariante() = itemMedia.Video.Variantes
                            Dim listaOrdenada As New List(Of TweetVideoVariante)

                            For Each item In listaVideos
                                listaOrdenada.Add(item)
                            Next

                            listaOrdenada.Sort(Function(x, y) y.Bitrate.CompareTo(x.Bitrate))

                            datos.Enlace = listaOrdenada(0).Enlace

                            AddHandler gridMedia.PointerPressed, AddressOf UsuarioClickeaVideo
                        ElseIf itemMedia.Tipo = "animated_gif" Then
                            datos.Enlace = itemMedia.Video.Variantes(0).Enlace
                            AddHandler gridMedia.PointerPressed, AddressOf UsuarioClickeaGif
                        End If

                        gridMedia.Tag = datos
                        gridMedia.Children.Add(imagenMedia)

                        If itemMedia.Tipo = "video" Or itemMedia.Tipo = "animated_gif" Then
                            gridPlay.Width = gridMedia.ActualWidth / 2
                            gridPlay.Height = gridMedia.ActualHeight / 2

                            Dim gridTipo As New Grid With {
                                .HorizontalAlignment = HorizontalAlignment.Left,
                                .VerticalAlignment = VerticalAlignment.Bottom,
                                .Padding = New Thickness(3, 3, 3, 3),
                                .Background = New SolidColorBrush(color)
                            }

                            Dim tbTipo As New TextBlock With {
                                .Foreground = New SolidColorBrush(Colors.White),
                                .FontSize = 13
                            }

                            If itemMedia.Tipo = "video" Then
                                tbTipo.Text = "video"
                            ElseIf itemMedia.Tipo = "animated_gif" Then
                                tbTipo.Text = "gif"
                            End If

                            gridTipo.Children.Add(tbTipo)

                            gridMedia.Children.Add(gridTipo)
                            gridMedia.Children.Add(gridPlay)
                        End If

                        spMedia.Children.Add(gridMedia)
                    End If
                Next

                If spMedia.Children.Count > 0 Then
                    Dim sv As New ScrollViewer With {
                        .HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        .Content = spMedia,
                        .HorizontalAlignment = HorizontalAlignment.Left,
                        .VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
                    }

                    Return sv
                End If
            End If

            Return New StackPanel
        End Function

        Private Sub UsuarioEntraMedia(sender As Object, e As PointerRoutedEventArgs)

            Dim grid As Grid = sender

            If TypeOf grid.Children(0) Is ImageEx Then
                Dim imagen As ImageEx = grid.Children(0)
                imagen.Saturation(0).Start()
            End If

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

        End Sub

        Private Sub UsuarioSaleMedia(sender As Object, e As PointerRoutedEventArgs)

            Dim grid As Grid = sender

            If TypeOf grid.Children(0) Is ImageEx Then
                Dim imagen As ImageEx = grid.Children(0)
                imagen.Saturation(1).Start()
            End If

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

        End Sub

        Public Sub UsuarioClickeaImagen(sender As Object, e As PointerRoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gridRecibido As Grid = sender
            Dim datos As Objetos.MediaDatos = gridRecibido.Tag

            Dim color As Color = datos.Color

            If color = Nothing Then
                color = App.Current.Resources("ColorSecundario")
            End If

            Dim gridUsuario As Grid = pagina.FindName("gridUsuarioAmpliado")

            If gridUsuario.Visibility = Visibility.Collapsed Then
                color = App.Current.Resources("ColorSecundario")
            End If

            Dim botonCerrar As Button = pagina.FindName("botonCerrarImagen")
            botonCerrar.Background = New SolidColorBrush(color)

            Dim tbImagenAmpliada As TextBox = pagina.FindName("tbImagenAmpliada")
            tbImagenAmpliada.Text = datos.Enlace

            Dim botonCopiar As Button = pagina.FindName("botonCopiarImagen")
            botonCopiar.Background = New SolidColorBrush(color)

            Dim bordeImagen As Border = pagina.FindName("bordeImagenAmpliada")
            bordeImagen.BorderBrush = New SolidColorBrush(color)

            Dim gridImagen As Grid = pagina.FindName("gridImagenAmpliada")
            gridImagen.Visibility = Visibility.Visible

            Dim imagenAmpliada As ImageEx = pagina.FindName("imagenAmpliada")

            Try
                imagenAmpliada.Foreground = New SolidColorBrush(color)
                imagenAmpliada.Source = datos.Enlace
                imagenAmpliada.Tag = datos.Imagen

                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("imagenAmpliada", datos.Imagen)

                Dim animacion As ConnectedAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("imagenAmpliada")

                If Not animacion Is Nothing Then
                    animacion.TryStart(imagenAmpliada)
                End If
            Catch ex As Exception

            End Try

        End Sub

        Public Sub UsuarioClickeaVideo(sender As Object, e As PointerRoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gridRecibido As Grid = sender
            Dim datos As Objetos.MediaDatos = gridRecibido.Tag

            Dim color As Color = datos.Color

            If color = Nothing Then
                color = App.Current.Resources("ColorSecundario")
            End If

            Dim gridUsuario As Grid = pagina.FindName("gridUsuarioAmpliado")

            If gridUsuario.Visibility = Visibility.Collapsed Then
                color = App.Current.Resources("ColorSecundario")
            End If

            Dim botonCerrar As Button = pagina.FindName("botonCerrarVideo")
            botonCerrar.Background = New SolidColorBrush(color)

            Dim tbVideoAmpliado As TextBox = pagina.FindName("tbVideoAmpliado")
            tbVideoAmpliado.Text = datos.Enlace

            Dim botonCopiar As Button = pagina.FindName("botonCopiarVideo")
            botonCopiar.Background = New SolidColorBrush(color)

            Dim bordeVideo As Border = pagina.FindName("bordeVideoAmpliado")
            bordeVideo.BorderBrush = New SolidColorBrush(color)

            Dim gridImagen As Grid = pagina.FindName("gridVideoAmpliado")
            gridImagen.Visibility = Visibility.Visible

            Dim reproductor As MediaPlayerElement = pagina.FindName("videoAmpliado")

            Try
                reproductor.Source = MediaSource.CreateFromUri(New Uri(datos.Enlace))
                reproductor.MediaPlayer.Play()
                reproductor.Tag = datos.Imagen
            Catch ex As Exception

            End Try

        End Sub

        Public Sub UsuarioClickeaGif(sender As Object, e As PointerRoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gridRecibido As Grid = sender
            Dim datos As Objetos.MediaDatos = gridRecibido.Tag

            Dim boolPlay As Boolean = True

            For Each hijo In gridRecibido.Children
                If TypeOf hijo Is MediaPlayerElement Then
                    Dim hijoReproductor As MediaPlayerElement = hijo

                    If hijoReproductor.MediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Playing Then
                        boolPlay = False
                    Else
                        boolPlay = True
                    End If
                End If
            Next

            Dim reproductor As New MediaPlayerElement

            Try
                reproductor.Source = MediaSource.CreateFromUri(New Uri(datos.Enlace))
                reproductor.MediaPlayer.IsLoopingEnabled = True

                If boolPlay = True Then
                    reproductor.MediaPlayer.Play()
                Else
                    reproductor.MediaPlayer.Pause()
                End If
            Catch ex As Exception

            End Try

            gridRecibido.Children.Add(reproductor)

        End Sub

    End Module
End Namespace
