using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dimiwords_Client_WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private DateTime downTime_Close, downTime_Min, downTime_Max;
        private object downSender_Close, downSender_Min, downSender_Max;
        private bool isInsideimage = false;
        private ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
        private ChromeDriver driver = null;
        private BackgroundWorker isDimiwords = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            driverService.HideCommandPromptWindow = true;
            isDimiwords.DoWork += IsDimiwords_DoWork;
            isDimiwords.RunWorkerAsync();
        }

        private string GetActiveWindowTitle()
        {
            var Buff = new StringBuilder(256);
            var handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, 256) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        private void IsDimiwords_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (GetActiveWindowTitle() == "DIMIWORDS - Chrome")
                {
                    Dispatcher.Invoke(() => Theme(driver.PageSource.Contains("background-image: linear-gradient(to right, rgb(236, 0, 140), rgb(252, 103, 103));")));
                }
                Thread.Sleep(10);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Discord.Start();
            Discord.UpdatePresence();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Discord.Stop();
            driver.Quit();
            Closedriver();
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
                var linear = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                linear.GradientStops.Add(new GradientStop(Color.FromRgb(236, 0, 140), 0));
                linear.GradientStops.Add(new GradientStop(Color.FromRgb(252, 103, 103), 1));
                dockPanel1.Background = linear;
            }
            else
            {
                var linear = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                linear.GradientStops.Add(new GradientStop(Color.FromRgb(85, 47, 201), 0));
                linear.GradientStops.Add(new GradientStop(Color.FromRgb(66, 150, 219), 1));
                dockPanel1.Background = linear;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Closedriver();
            driver = new ChromeDriver(driverService, new ChromeOptions())
            {
                Url = "https://dimiwords.tk/#/user/login"
            };
            var t1 = new Thread(() => driver.FindElementByXPath("//input[@placeholder='이메일']").SendKeys("shimjs8@naver.com"));
            var t2 = new Thread(() => driver.FindElementByXPath("//input[@placeholder='비밀번호']").SendKeys("+=mkonji1243"));
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            driver.FindElementByXPath("//button[.='로그인']").Click();
        }

        private void Closedriver()
        {
            var chromedriver = Process.GetProcessesByName("chromedriver");
            if (chromedriver.Length > 0)
            {
                foreach (var process in chromedriver)
                {
                    process.Kill();
                }
            }
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
                    try
                    {
                        Close();
                    }
                    catch (NullReferenceException)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
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
