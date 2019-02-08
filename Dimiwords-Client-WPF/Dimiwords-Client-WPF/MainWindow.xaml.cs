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
using static Dimiwords_Client_WPF.Properties.Settings;

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
        private bool isInsideimage = false, isStateChange = false;
        private ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
        private ChromeDriver dimiwords = null, benedu = null, civar09 = null;
        private ChromeOptions chromeOptions = new ChromeOptions();
        private BackgroundWorker isDimiwords = new BackgroundWorker();
        private string id = "바보 주노", pw = "멍청 주노";

        public MainWindow()
        {
            InitializeComponent();
            Left = Default.form_Size.Left;
            Top = Default.form_Size.Top;
            Width = Default.form_Size.Width;
            Height = Default.form_Size.Height;
            if (Default.form_is_Max)
            {
                WindowState = WindowState.Maximized;
                SetMaxImage("");
            }
            Theme(Default.theme_Set);
            driverService.HideCommandPromptWindow = true;
            isDimiwords.DoWork += IsDimiwords_DoWork;
            isDimiwords.RunWorkerAsync();
            chromeOptions.AddArguments($"--window-position={Left},{Top}", $"--window-size={Width},{Height}");
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
                try
                {
                    var activeWindow = GetActiveWindowTitle();
                    if (activeWindow == "DIMIWORDS - Chrome")
                    {
                        Dispatcher.Invoke(() => Theme(dimiwords.PageSource.Contains("background-image: linear-gradient(to right, rgb(236, 0, 140), rgb(252, 103, 103));") ? Themes.Magenta : Themes.Blue));
                    }
                    else if (activeWindow == "좋은공부 많이하자!::베네듀(BenEdu) - Chrome")
                    {
                        Dispatcher.Invoke(() => Theme(Themes.Yellow_BND));
                    }
                    else if (activeWindow == "CIVAR09 - Chrome")
                    {
                        Dispatcher.Invoke(() => Theme(Themes.Yellow_09));
                    }
                }
                catch (NullReferenceException)
                {

                }
                catch (InvalidOperationException)
                {

                }
                Thread.Sleep(10);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Discord.Start();
            Discord.UpdatePresence();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Discord.Stop();
            dimiwords.Quit();
            Closedriver();
        }

        private void SetCloseImage(string bmp)
        {
            Image1.Source = new BitmapImage(new Uri($"/Dimiwords-Client-WPF;component/Resources/{bmp}.png", UriKind.Relative));
        }

        private void SetMaxImage(string bmp)
        {
            var MON = (WindowState == WindowState.Maximized ? "Normal" : "Max") + bmp;
            Image2.Source = new BitmapImage(new Uri($"/Dimiwords-Client-WPF;component/Resources/{MON}.png", UriKind.Relative));
        }

        private void SetMinImage(string bmp)
        {
            Image3.Source = new BitmapImage(new Uri($"/Dimiwords-Client-WPF;component/Resources/{bmp}.png", UriKind.Relative));
        }

        private void Theme(int ThemeSet)
        {
            if (ThemeSet == Themes.Magenta)
            {
                var linear = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                linear.GradientStops.Add(new GradientStop(Color.FromRgb(236, 0, 140), 0));
                linear.GradientStops.Add(new GradientStop(Color.FromRgb(252, 103, 103), 1));
                dockPanel1.Background = linear;
                Default.dimiwords_Theme = false;
            }
            else if (ThemeSet == Themes.Blue)
            {
                var linear = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                linear.GradientStops.Add(new GradientStop(Color.FromRgb(85, 47, 201), 0));
                linear.GradientStops.Add(new GradientStop(Color.FromRgb(66, 150, 219), 1));
                dockPanel1.Background = linear;
                Default.dimiwords_Theme = true;
            }
            else if (ThemeSet == Themes.Yellow_BND)
            {
                var linear = new SolidColorBrush(Color.FromRgb(252, 208, 54));
                dockPanel1.Background = linear;
            }
            else if (ThemeSet == Themes.Yellow_09)
            {
                var linear = new SolidColorBrush(Color.FromRgb(255, 201, 56));
                dockPanel1.Background = linear;
            }
            Default.Save();
            Default.theme_Set = ThemeSet;
            Default.Save();
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
                SetMaxImage("_Mouse_Down");
            }
        }

        private void Image2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && sender == downSender_Max)
            {
                var timeSinceDown = DateTime.Now - downTime_Max;
                if (timeSinceDown.TotalMilliseconds < 500)
                {
                    WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                }
                SetMaxImage("_On_Mouse");
            }
        }

        private void Image2_MouseEnter(object sender, MouseEventArgs e)
        {
            isInsideimage = true;
            SetMaxImage("_On_Mouse");
        }

        private void Image2_MouseLeave(object sender, MouseEventArgs e)
        {
            isInsideimage = false;
            SetMaxImage("");
        }

        private void civar09_Open(object sender, RoutedEventArgs e)
        {
            try
            {
                civar09?.Quit();
                civar09 = new ChromeDriver(driverService, chromeOptions)
                {
                    Url = "http://dimiwords.tk:39991/#/"
                };
            }
            catch (InvalidOperationException)
            {

            }
            catch (NoSuchWindowException)
            {

            }
            catch (WebDriverException)
            {

            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!isStateChange)
            {
                Default.form_Size = new System.Drawing.Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
                Default.Save();
            }
            isStateChange = false;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            isStateChange = true;
            Default.form_is_Max = WindowState == WindowState.Maximized;
            Default.Save();
        }

        private void dimiwords_Open(object sender, RoutedEventArgs e)
        {
            try
            {
                dimiwords?.Quit();
                dimiwords = new ChromeDriver(driverService, chromeOptions)
                {
                    Url = "https://dimiwords.tk/#/user/login"
                };
                dimiwords.FindElementByXPath("//input[@placeholder='이메일']").SendKeys(id);
                dimiwords.FindElementByXPath("//input[@placeholder='비밀번호']").SendKeys(pw);
                Thread.Sleep(200);
                dimiwords.FindElementByXPath("//button[.='로그인']").Click();
                if (Default.dimiwords_Theme)
                {
                    var retry = 0;
                    Retry:;
                    if (retry > 300)
                        goto RetryEnd;
                    try
                    {
                        Thread.Sleep(10);
                        dimiwords.FindElementByXPath("//div[@class='v-switch-button']").Click();
                        goto RetryEnd;
                    }
                    catch (NoSuchElementException)
                    {
                        retry++;
                        goto Retry;
                    }
                    catch (StaleElementReferenceException)
                    {
                        retry++;
                        goto Retry;
                    }
                    RetryEnd:;
                }
            }
            catch (InvalidOperationException)
            {

            }
            catch (NoSuchWindowException)
            {

            }
            catch (WebDriverException)
            {

            }
        }

        private void benedu_Open(object sender, RoutedEventArgs e)
        {
            try
            {
                benedu?.Quit();
                benedu = new ChromeDriver(driverService, chromeOptions)
                {
                    Url = "https://www.benedu.co.kr/Index.aspx"
                };
                benedu.FindElementById("liLogin").Click();
                var retry = 0;
                Retry:;
                if (retry > 300)
                    goto RetryEnd;
                try
                {
                    Thread.Sleep(100);
                    benedu.FindElementById("inputEmail").SendKeys(id);
                    Thread.Sleep(100);
                    benedu.FindElementById("inputPassword").SendKeys(pw);
                    Thread.Sleep(200);
                    benedu.FindElementById("btnLogin").Click();
                }
                catch (ElementNotVisibleException)
                {
                    retry++;
                    goto Retry;
                }
                RetryEnd:;
            }
            catch (InvalidOperationException)
            {

            }
            catch (NoSuchWindowException)
            {

            }
            catch (WebDriverException)
            {

            }
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
