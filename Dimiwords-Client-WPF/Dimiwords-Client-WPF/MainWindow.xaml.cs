using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Dimiwords_Client_WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime downTime_Close, downTime_Min, downTime_Max;
        private object downSender_Close, downSender_Min, downSender_Max;
        private bool isInsideimage = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Discord.Start();
            Discord.UpdatePresence();
            TTS.Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Discord.Stop();
            TTS.Dispose();
        }

        private void SetCloseImage(string bmp)
        {
            Image1.Source = new BitmapImage(new Uri($"/Dimiwords-Client-WPF;component/Resources/{bmp}.png", UriKind.Relative));
        }

        private void SetMaxImage(string bmp)
        {
            Image2.Source = new BitmapImage(new Uri($"/Dimiwords-Client-WPF;component/Resources/{bmp}.png", UriKind.Relative));
        }

        private void SetMinImage(string bmp)
        {
            Image3.Source = new BitmapImage(new Uri($"/Dimiwords-Client-WPF;component/Resources/{bmp}.png", UriKind.Relative));
        }

        private void Theme(bool isMagenta)
        {
            if (isMagenta)
            {

            }
            else
            {

            }
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isInsideimage)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    Top = 0;
                    Left = Mouse.GetPosition(this).X - (Width / 2);
                    if (Left < 0)
                        Left -= Left;
                    else if (Left + Width > SystemParameters.WorkArea.Width)
                        Left -= Left + Width - SystemParameters.WorkArea.Width;
                }
                DragMove();
            }
        }

        private void Image3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                downSender_Min = sender;
                downTime_Min = DateTime.Now;
                SetMinImage("Min_Mouse_Down");
            }
        }

        private void Image3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && sender == downSender_Min)
            {
                var timeSinceDown = DateTime.Now - downTime_Min;
                if (timeSinceDown.TotalMilliseconds < 500)
                {
                    WindowState = WindowState.Minimized;
                }
                SetMinImage("Min_On_Mouse");
            }
        }

        private void Image3_MouseEnter(object sender, MouseEventArgs e)
        {
            isInsideimage = true;
            SetMinImage("Min_On_Mouse");
        }

        private void Image3_MouseLeave(object sender, MouseEventArgs e)
        {
            isInsideimage = false;
            SetMinImage("Min");
        }

        private void Image2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                downSender_Max = sender;
                downTime_Max = DateTime.Now;
                SetMaxImage("Max_Mouse_Down");
            }
        }

        private void Image2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && sender == downSender_Max)
            {
                var timeSinceDown = DateTime.Now - downTime_Max;
                if (timeSinceDown.TotalMilliseconds < 500)
                {
                    if (WindowState == WindowState.Maximized)
                        WindowState = WindowState.Normal;
                    else
                        WindowState = WindowState.Maximized;
                }
                SetMaxImage("Max_On_Mouse");
            }
        }

        private void Image2_MouseEnter(object sender, MouseEventArgs e)
        {
            isInsideimage = true;
            SetMaxImage("Max_On_Mouse");
        }

        private void Image2_MouseLeave(object sender, MouseEventArgs e)
        {
            isInsideimage = false;
            SetMaxImage("Max");
        }

        private void Image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                downSender_Close = sender;
                downTime_Close = DateTime.Now;
                SetCloseImage("Close_Mouse_Down");
            }
        }

        private void Image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && sender ==  downSender_Close)
            {
                var timeSinceDown = DateTime.Now - downTime_Close;
                if (timeSinceDown.TotalMilliseconds < 500)
                {
                    Close();
                }
                SetCloseImage("Close_On_Mouse");
            }
        }

        private void Image1_MouseEnter(object sender, MouseEventArgs e)
        {
            isInsideimage = true;
            SetCloseImage("Close_On_Mouse");
        }

        private void Image1_MouseLeave(object sender, MouseEventArgs e)
        {
            isInsideimage = false;
            SetCloseImage("Close");
        }
    }
}
