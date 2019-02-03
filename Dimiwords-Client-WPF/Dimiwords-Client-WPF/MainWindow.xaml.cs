using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dimiwords_Client_WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime downTime;
        private object downSender;
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
            //var Bi = new BitmapImage(new Uri($@"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};component/{bmp}.png", UriKind.Absolute));
            Image1.Source = new BitmapImage(new Uri($"/Dimiwords-Client-WPF;component/Resources/{bmp}.png", UriKind.Relative));
        }

        private void Them(bool isWhite)
        {
            if (isWhite)
            {

            }
            else
            {

            }
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isInsideimage)
                DragMove();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                downSender = sender;
                downTime = DateTime.Now;
                SetCloseImage("Close_Mouse_Down");
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && sender ==  downSender)
            {
                var timeSinceDown = DateTime.Now - downTime;
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
