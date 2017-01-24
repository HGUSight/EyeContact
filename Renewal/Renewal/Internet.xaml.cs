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

            Width = Application.Current.MainWindow.Width;
            Height = Application.Current.MainWindow.Height;

            Open.Width = Width;
            Open.Height = Height / 6;

            Back.Width = Width;
            Back.Height = Height / 6;

            Wallpaper.Width = Width;
            Wallpaper.Height = Height / 6;

            Search.Width = Width;
            Search.Height = Height / 6;

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


      
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Process process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                                                                            + "\\Internet Explorer\\iexplore.exe", "http://www.naver.com");
            process.Start();
        }

        private void Wallpaper_Click(object sender, RoutedEventArgs e)
        {
            const byte Window = 0x5B;// window key
            const byte D = 0x44;// D

            keybd_event(Window, 0, KEYDOWN, 0);
            keybd_event(D, 0, KEYDOWN, 0);
            keybd_event(Window, 0, KEYUP, 0);
            keybd_event(D, 0, KEYUP, 0);
        }


        //****************************************************************************

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hwnd, int msg, int wParam, StringBuilder sb);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string _ClassName, string _WindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern int FindWindowEx(int _Parent, int _ChildAfter, string _ClassName, string _WindowName);

        public const int WM_GETTEXTLENGTH = 0x000E;
        public const int WM_GETTEXT = 0x000D;

        Keyboard dlg = new Renewal.Keyboard();
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            int ie = FindWindow("IEFrame", null);
            int worker = FindWindowEx(ie, 0, "WorkerW", null);
            int toolbar = FindWindowEx(worker, 0, "rebarwindow32", null);
            int comboboxex = FindWindowEx(toolbar, 0, "Address Band Root", null);
            int edit = FindWindowEx(comboboxex, 0, "Edit", null);

            StringBuilder sb = new StringBuilder(512);
            SendMessage(edit, WM_GETTEXT, 255, sb);
            string url;

           
            url = sb.ToString();
            Uri myUri = new Uri(url);
            string host = myUri.Host;
            //MessageBox.Show("here 1 = " + host);

            if( host.IndexOf("naver") != -1 )
            {
                    
                dlg.Closed += new EventHandler(Keyboard_Closed);
                dlg.Show(); // 키보드 열기
                   


            }
            else if (host.IndexOf("daum") != -1)
            {

            }
            else if(host.IndexOf("google") != -1)
            {

            }
            else if (host.IndexOf("zum") != -1)
            {

            }
            else if(host.IndexOf("bing") != -1 || host.IndexOf("MSN") != -1)
            {

            }
            else if (host.IndexOf("youtube") != -1)
            {

            }
            else
            {
                MessageBox.Show("검색 기능이 지원되지 않는 사이트입니다. 검색 창에 직접 커서를 올려주세요.");
            }


               
        

          
        }

        void Keyboard_Closed(object sender, EventArgs e)
        {
            
            string word = dlg.textBox.Text;
            // MessageBox.Show(word);
            string url = "http://search.naver.com/search.naver?where=nexearch&query=" + word;
        

            Clipboard.SetText(url);
            const byte Alt = 0x12;// Alt key
            const byte D = 0x44;// D

            keybd_event(Alt, 0, KEYDOWN, 0);
            keybd_event(D, 0, KEYDOWN, 0);
            keybd_event(Alt, 0, KEYUP, 0);
            keybd_event(D, 0, KEYUP, 0);

            System.Threading.Thread.Sleep(100);

            keybd_event(0x08, 0, 0, 0); // backspace
            keybd_event(0x08, 0, 0x0002, 0);

            System.Threading.Thread.Sleep(100);

            keybd_event(0x11, 0, 0, 0); // ctrl+V
            keybd_event((byte)'V', 0, 0, 0);
            keybd_event(0x11, 0, 0x0002, 0);
            keybd_event((byte)'V', 0, 0x0002, 0);

           
            System.Threading.Thread.Sleep(100);

            keybd_event((byte)0x0D, 0, 0, 0); // enter
            keybd_event((byte)0x0D, 0, 0x0002, 0);


        }

        void Keyboard_Exit()
        {
            

            

        }
        /*// 앞으로
        private void Forward_Click(object sender, RoutedEventArgs e) 
        {
            const byte ALT = 0x12;// alt
            const byte Right = 0x27;// ->

            keybd_event(ALT, 0, KEYDOWN, 0);
            keybd_event(Right, 0, KEYDOWN, 0);
            keybd_event(ALT, 0, KEYUP, 0);
            keybd_event(Right, 0, KEYUP, 0);
        }
        */

        /*// 새로고침
      private void Re_Click(object sender, RoutedEventArgs e)
      {
          const byte F5 = 0x74;

          keybd_event(F5, 0, KEYDOWN, 0);//F5
          keybd_event(F5, 0, KEYUP, 0);
      }
      */

        /*******************************************************/
        // 윈도우 로드, 클로즈 시 Work area 변경
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppBarFunctions.SetAppBar(this, ABEdge.Left);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppBarFunctions.SetAppBar(this, ABEdge.None);
        }

    }
}

