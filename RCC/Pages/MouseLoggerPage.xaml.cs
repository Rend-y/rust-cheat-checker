using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Extensions.Logging;

namespace RCC.Pages
{
    public partial class MouseLoggerPage : APage
    {
        public MouseLoggerPage(ILogger<MouseLoggerPage> logger) : base(logger)
        {
            InitializeComponent();
        }

        private void CanvasMouseDrawing_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            string conversionEventToString;
            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    conversionEventToString = "Пользователь нажал правую кнопку";
                    break;
                case MouseButton.Left:
                    conversionEventToString = "Пользователь нажал левую кнопку";
                    break;
                case MouseButton.Middle:
                    conversionEventToString = "Пользователь нажал центральную кнопку";
                    break;
                case MouseButton.XButton1:
                    conversionEventToString = "Пользователь нажал боковую (1) кнопку";
                    break;
                case MouseButton.XButton2:
                    conversionEventToString = "Пользователь нажал боковую (2) кнопку";
                    break;
                default:
                    conversionEventToString = "Пользователь нажал неизвестную кнопку";
                    break;
            }
            ListAllMouseEvent.Items.Insert(0, new MouseActivity(conversionEventToString));
            if (ListAllMouseEvent.Items.Count > 10) ListAllMouseEvent.Items.RemoveAt(ListAllMouseEvent.Items.Count - 1);
        }

        private void CanvasMouseDrawing_OnMouseUp(object sender, MouseButtonEventArgs e) =>
            CanvasMouseDrawing.Children.Clear();

        private void CanvasMouseDrawing_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line
                {
                    Stroke = SystemColors.WindowFrameBrush,
                    X1 = e.GetPosition(CanvasMouseDrawing).X - 1,
                    Y1 = e.GetPosition(CanvasMouseDrawing).Y - 1,
                    X2 = e.GetPosition(CanvasMouseDrawing).X,
                    Y2 = e.GetPosition(CanvasMouseDrawing).Y
                };

                CanvasMouseDrawing.Children.Add(line);
            }
        }

        public class MouseActivity
        {
            public MouseActivity(string key)
            {
                this.Key = key;
            }
            public string Key { get; set; }
        }
    }
}