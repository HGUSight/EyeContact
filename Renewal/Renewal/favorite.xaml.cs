using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Renewal
{
    /// <summary>
    /// favorite.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class favorite : Window
    {
        public favorite()
        {
            InitializeComponent();

            Top = 0;
            Left = 0;

            Width = Application.Current.MainWindow.Width;

            Naver.Width = Width * 0.95;
            Naver.Height = Height / 6 * 0.95;

            Daum.Width = Width * 0.95;
            Daum.Height = Height / 6 * 0.95;

            Facebook.Width = Width * 0.95;
            Facebook.Height = Height / 6 * 0.95;

            Youtube.Width = Width * 0.95;
            Youtube.Height = Height / 6 * 0.95;

            Back.Width = Width * 0.95;
            Back.Height = Height / 6 * 0.95;
        }

        #region focus
        // 창에 focus 가지 않도록 no activate
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        #endregion

        #region initialized
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr ip = SetWindowLong(helper.Handle, GWL_EXSTYLE,
                GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }
        #endregion

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private void Naver_Click(object sender, RoutedEventArgs e)
        {
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();
            IntPtr handle = GetForegroundWindow();
            foreach (SHDocVw.WebBrowser IE in shellWindows)
            {
                if (IE.HWND.Equals(handle.ToInt32()))
                {
                    if (!IE.Busy)
                        IE.Navigate("www.naver.com");
                }

                
            }
        }

        private void Daum_Click(object sender, RoutedEventArgs e)
        {
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();
            IntPtr handle = GetForegroundWindow();
            foreach (SHDocVw.WebBrowser IE in shellWindows)
            {
                if (IE.HWND.Equals(handle.ToInt32()))
                {
                    if (!IE.Busy)
                        IE.Navigate("www.daum.com");
                }


            }
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();
            IntPtr handle = GetForegroundWindow();
            foreach (SHDocVw.WebBrowser IE in shellWindows)
            {
                if (IE.HWND.Equals(handle.ToInt32()))
                {
                    if (!IE.Busy)
                        IE.Navigate("www.facebook.com");
                }


            }
        }

        private void Youtube_Click(object sender, RoutedEventArgs e)
        {
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();
            IntPtr handle = GetForegroundWindow();
            foreach (SHDocVw.WebBrowser IE in shellWindows)
            {
                if (IE.HWND.Equals(handle.ToInt32()))
                {
                    if (!IE.Busy)
                        IE.Navigate("www.youtube.com");
                }


            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
