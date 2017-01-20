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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Diagnostics;
using System.Windows.Interop;

namespace Renewal
{
    /// <summary>
    /// Internet.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Internet : Window
    {
        

        public Internet()
        {
            InitializeComponent();
        }

        // 창에 focus 가지 않도록 no activate
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

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


        // 키보드 이벤트 API
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        const int KEYDOWN = 0x0000;
        const int KEYUP = 0x0002;

        private void Back_Click(object sender, RoutedEventArgs e) // 뒤로
        {
            //for keyboard event trigger
            const byte ALT = 0x12;// alt
            const byte Left = 0x25;// <-
            
            keybd_event(ALT, 0, KEYDOWN, 0); 
            keybd_event(Left, 0, KEYDOWN, 0); 
            keybd_event(ALT, 0, KEYUP, 0);
            keybd_event(Left, 0, KEYUP, 0);
        }

        private void Forward_Click(object sender, RoutedEventArgs e) // 앞으로
        {
            const byte ALT = 0x12;// alt
            const byte Right = 0x27;// ->

            keybd_event(ALT, 0, KEYDOWN, 0); 
            keybd_event(Right, 0, KEYDOWN, 0); 
            keybd_event(ALT, 0, KEYUP, 0);
            keybd_event(Right, 0, KEYUP, 0);
        }

        private void Re_Click(object sender, RoutedEventArgs e)
        {
            const byte F5 = 0x74;// F5

            keybd_event(F5, 0, KEYDOWN, 0);
            keybd_event(F5, 0, KEYUP, 0);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Process process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                                                                            + "\\Internet Explorer\\iexplore.exe", "http://www.naver.com");
            process.Start();
        }

     
    }
}
