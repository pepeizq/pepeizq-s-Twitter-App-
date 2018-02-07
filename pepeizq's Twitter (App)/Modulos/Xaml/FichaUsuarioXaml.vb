﻿Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports pepeizq.Twitter
Imports pepeizq.Twitter.Banner
Imports pepeizq.Twitter.Tweet
Imports Windows.UI
Imports Windows.UI.Text
Imports Windows.UI.Xaml.Shapes

Module FichaUsuarioXaml

    Public Async Sub Generar(cosas As pepeizq.Twitter.Objetos.UsuarioAmpliado)

        Dim recursos As New Resources.ResourceLoader

        Dim usuario As TwitterUsuario = cosas.Usuario
        Dim provider As TwitterDataProvider = cosas.MegaUsuario.Servicio.Provider

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gridTitulo As Grid = pagina.FindName("gridTitulo")

        Dim gridUsuario As Grid = pagina.FindName("gridUsuarioAmpliado")
        gridUsuario.Visibility = Visibility.Visible

        Dim color As Color = Nothing

        Try
            color = ("#" + usuario.ColorEnlace).ToColor
        Catch ex As Exception
            color = App.Current.Resources("ColorSecundario")
        End Try

        App.Current.Resources("ButtonBackgroundPointerOver") = color

        Dim transpariencia As New UISettings
        Dim boolTranspariencia As Boolean = transpariencia.AdvancedEffectsEnabled

        If boolTranspariencia = False Then
            gridTitulo.Background = New SolidColorBrush(color)
            gridUsuario.Background = New SolidColorBrush(color)
        Else
            Dim acrilico As New AcrylicBrush With {
                .BackgroundSource = AcrylicBackgroundSource.Backdrop,
                .TintOpacity = 0.7,
                .TintColor = color
            }

            gridTitulo.Background = acrilico
            gridUsuario.Background = acrilico
        End If

        Dim lvTweets As ListView = pagina.FindName("lvTweetsUsuario")

        If lvTweets.Items.Count > 0 Then
            lvTweets.Items.Clear()
        End If

        Dim pbTweets As ProgressBar = pagina.FindName("pbTweetsUsuario")
        Dim svTweets As ScrollViewer = pagina.FindName("svTweetsUsuario")
        svTweets.Tag = New pepeizq.Twitter.Objetos.ScrollViewerTweets(cosas.MegaUsuario, Nothing, pbTweets, 2, usuario.ScreenNombre, color)
        svTweets.Foreground = New SolidColorBrush(("#" + usuario.ColorTexto).ToColor)
        AddHandler svTweets.ViewChanging, AddressOf SvTweets_ViewChanging

        '------------------------------------

        Dim botonCerrar As Button = pagina.FindName("botonCerrarUsuario")
        botonCerrar.Background = New SolidColorBrush(color)

        Dim banner As Banner = Nothing

        Try
            banner = Await provider.CogerBannerUsuario(usuario.ScreenNombre, New BannerParser)
        Catch ex As Exception

        End Try

        Dim spFondo As StackPanel = pagina.FindName("gridImagenFondoUsuario")
        Dim imagenFondo As ImageEx = pagina.FindName("imagenFondoUsuario")

        If Not banner Is Nothing Then
            imagenFondo.Source = New Uri(banner.Tamaños.I600x200.Enlace)
            spFondo.Background = New SolidColorBrush(Colors.Transparent)
        Else
            imagenFondo.Source = Nothing
            spFondo.Background = New SolidColorBrush(Colors.Black)
        End If

        Dim circuloAvatar As Ellipse = pagina.FindName("ellipseAvatar")

        Dim imagenAvatar As New ImageBrush With {
            .Stretch = Stretch.Uniform,
            .ImageSource = New BitmapImage(New Uri(usuario.ImagenAvatar.Replace("_normal.png", "_bigger.png")))
        }

        circuloAvatar.Fill = imagenAvatar

        Dim tbNombre As TextBlock = pagina.FindName("tbNombreUsuario")
        tbNombre.Text = usuario.Nombre

        Dim imagenVerificado As ImageEx = pagina.FindName("imagenUsuarioVerificado")

        If usuario.Verificado = True Then
            imagenVerificado.Visibility = Visibility.Visible
        Else
            imagenVerificado.Visibility = Visibility.Collapsed
        End If

        Dim tbScreenNombre As TextBlock = pagina.FindName("tbScreenNombreUsuario")
        tbScreenNombre.Text = "@" + usuario.ScreenNombre

        Dim columnaEnlace As ColumnDefinition = pagina.FindName("columnaEnlaceUsuario")

        If Not usuario.Entidades.Enlace Is Nothing Then
            Try
                Dim hlEnlace As HyperlinkButton = pagina.FindName("hlEnlaceUsuario")
                hlEnlace.NavigateUri = New Uri(usuario.Entidades.Enlace.Enlaces(0).Expandida)

                Dim tbEnlace As New TextBlock With {
                    .Text = usuario.Entidades.Enlace.Enlaces(0).Mostrar,
                    .Foreground = New SolidColorBrush(Colors.White),
                    .FontWeight = FontWeights.SemiBold
                }

                hlEnlace.Content = tbEnlace

                columnaEnlace.Width = New GridLength(1, GridUnitType.Star)
            Catch ex As Exception
                columnaEnlace.Width = New GridLength(1, GridUnitType.Auto)
            End Try
        Else
            columnaEnlace.Width = New GridLength(1, GridUnitType.Auto)
        End If

        Dim tbNumTweets As TextBlock = pagina.FindName("tbNumTweetsUsuario")
        tbNumTweets.Text = String.Format("{0:n0}", Integer.Parse(usuario.NumTweets))

        Dim tbNumSeguidores As TextBlock = pagina.FindName("tbNumSeguidoresUsuario")
        tbNumSeguidores.Text = String.Format("{0:n0}", Integer.Parse(usuario.Followers))

        Dim hlSeguidores As HyperlinkButton = pagina.FindName("hlSeguidoresUsuario")
        hlSeguidores.NavigateUri = New Uri("https://twitter.com/" + usuario.ScreenNombre + "/followers")

        Dim tbNumFavoritos As TextBlock = pagina.FindName("tbNumFavoritosUsuario")
        tbNumFavoritos.Text = String.Format("{0:n0}", Integer.Parse(usuario.Favoritos))

        Dim hlFavoritos As HyperlinkButton = pagina.FindName("hlFavoritosUsuario")
        hlFavoritos.NavigateUri = New Uri("https://twitter.com/" + usuario.ScreenNombre + "/likes")

        Dim botonSeguir As Button = pagina.FindName("botonSeguirUsuario")

        If usuario.Siguiendo = True Then
            botonSeguir.Content = recursos.GetString("Following")
        Else
            botonSeguir.Content = recursos.GetString("Follow")
        End If

        botonSeguir.Background = New SolidColorBrush(color)
        botonSeguir.Tag = New pepeizq.Twitter.Objetos.SeguirUsuarioBoton(cosas.MegaUsuario, cosas.Usuario)
        AddHandler botonSeguir.Click, AddressOf BotonSeguirClick

        '------------------------------------

        Dim listaTweets As New List(Of Tweet)

        Try
            listaTweets = Await provider.CogerTweetsTimelineUsuario(Of Tweet)(usuario.ScreenNombre, Nothing, New TweetParser)
        Catch ex As Exception

        End Try

        For Each tweet In listaTweets
            Dim boolAñadir As Boolean = True

            For Each item In lvTweets.Items
                Dim lvItem As ListViewItem = item
                Dim gridTweet As Grid = lvItem.Content
                Dim lvTweet As Tweet = gridTweet.Tag

                If lvTweet.ID = tweet.ID Then
                    boolAñadir = False
                End If
            Next

            If boolAñadir = True Then
                lvTweets.Items.Add(TweetXaml.Añadir(tweet, cosas.MegaUsuario, color))
            End If
        Next

    End Sub

    Private Async Sub BotonSeguirClick(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader

        Dim boton As Button = sender
        Dim cosas As pepeizq.Twitter.Objetos.SeguirUsuarioBoton = boton.Tag

        If boton.Content = recursos.GetString("Following") Then
            Await cosas.MegaUsuario.Servicio.DeshacerSeguirUsuario(cosas.MegaUsuario.Usuario.Tokens, cosas.Usuario.Id)
            boton.Content = recursos.GetString("Follow")
        Else
            Await cosas.MegaUsuario.Servicio.SeguirUsuario(cosas.MegaUsuario.Usuario.Tokens, cosas.Usuario.Id)
            boton.Content = recursos.GetString("Following")
        End If

        TwitterTimeLineInicio.CargarTweets(cosas.MegaUsuario, Nothing, True)

    End Sub

End Module
