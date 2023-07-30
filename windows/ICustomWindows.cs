using System;
using System.Windows;

namespace RCC.windows
{
    public interface ICustomWindow<T>
    {
        public T WindowType { get; set; }
    }
}