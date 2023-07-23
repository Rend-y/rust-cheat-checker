using System;
using System.Windows;

namespace RCC.windows
{
    public interface ICustomWindow<T> : IDisposable
    {
        void Show(string title, string message = default, T messageType = default);
        void Window_OnLoaded(object sender, RoutedEventArgs e);
    }
}