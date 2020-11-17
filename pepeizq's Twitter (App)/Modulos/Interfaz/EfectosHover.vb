﻿Imports Microsoft.Toolkit.Uwp.UI.Animations
Imports Windows.UI.Core
Imports Windows.UI.Xaml.Shapes

Namespace Interfaz
    Module EfectosHover

        Public Sub Entra_Basico(sender As Object, e As PointerRoutedEventArgs)

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

        End Sub

        Public Sub Sale_Basico(sender As Object, e As PointerRoutedEventArgs)

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

        End Sub

        Public Sub Entra_Sp_IconoNombre(sender As Object, e As PointerRoutedEventArgs)

            Dim boton As Button = sender
            Dim sp As StackPanel = boton.Content

            Dim icono As FontAwesome5.FontAwesome = sp.Children(0)
            icono.Saturation(1).Scale(1.1, 1.1, icono.ActualWidth / 2, icono.ActualHeight / 2).Start()

            Dim texto As TextBlock = sp.Children(1)
            texto.Saturation(1).Scale(1.1, 1.1, texto.ActualWidth / 2, texto.ActualHeight / 2).Start()

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

        End Sub

        Public Sub Sale_Sp_IconoNombre(sender As Object, e As PointerRoutedEventArgs)

            Dim boton As Button = sender
            Dim sp As StackPanel = boton.Content

            Dim icono As FontAwesome5.FontAwesome = sp.Children(0)
            icono.Saturation(1).Scale(1, 1, icono.ActualWidth / 2, icono.ActualHeight / 2).Start()

            Dim texto As TextBlock = sp.Children(1)
            texto.Saturation(1).Scale(1, 1, texto.ActualWidth / 2, texto.ActualHeight / 2).Start()

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

        End Sub

        Public Sub Entra_Boton_Icono(sender As Object, e As PointerRoutedEventArgs)

            Dim boton As Button = sender
            Dim icono As FontAwesome5.FontAwesome = boton.Content
            icono.Saturation(1).Scale(1.1, 1.1, icono.ActualWidth / 2, icono.ActualHeight / 2).Start()

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

        End Sub

        Public Sub Sale_Boton_Icono(sender As Object, e As PointerRoutedEventArgs)

            Dim boton As Button = sender
            Dim icono As FontAwesome5.FontAwesome = boton.Content
            icono.Saturation(1).Scale(1, 1, icono.ActualWidth / 2, icono.ActualHeight / 2).Start()

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

        End Sub

        Public Sub Entra_NVItem_Icono(sender As Object, e As PointerRoutedEventArgs)

            Dim item As NavigationViewItem = sender
            Dim icono As FontAwesome5.FontAwesome = item.Icon
            icono.Saturation(1).Scale(1.2, 1.2, icono.ActualWidth / 2, icono.ActualHeight / 2).Start()

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

        End Sub

        Public Sub Sale_NVItem_Icono(sender As Object, e As PointerRoutedEventArgs)

            Dim item As NavigationViewItem = sender
            Dim icono As FontAwesome5.FontAwesome = item.Icon
            icono.Saturation(1).Scale(1, 1, icono.ActualWidth / 2, icono.ActualHeight / 2).Start()

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

        End Sub

        Public Sub Entra_NVItem_Ellipse(sender As Object, e As PointerRoutedEventArgs)

            Dim item As NavigationViewItem = sender
            Dim sp As StackPanel = item.Content

            Dim icono As Ellipse = sp.Children(0)
            icono.Saturation(1).Scale(1.1, 1.1, icono.ActualWidth / 2, icono.ActualHeight / 2).Start()

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

        End Sub

        Public Sub Sale_NVItem_Ellipse(sender As Object, e As PointerRoutedEventArgs)

            Dim item As NavigationViewItem = sender
            Dim sp As StackPanel = item.Content

            Dim icono As Ellipse = sp.Children(0)
            icono.Saturation(1).Scale(1, 1, icono.ActualWidth / 2, icono.ActualHeight / 2).Start()

            Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

        End Sub

    End Module
End Namespace