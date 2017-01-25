using mshtml;
using SHDocVw;
using System;
using System.Threading;
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
    /// Internet.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Internet : Window
    {

        public Internet()
        {
            InitializeComponent();

            Width = Application.Current.MainWindow.Width;
            Height = Application.Current.MainWindow.Height;

            Back.Width = Width;
            Back.Height = Height / 6;

            Wallpaper.Width = Width;
            Wallpaper.Height = Height / 6;


            Search.Width = Width;
            Search.Height = Height / 6;

            Favorite.Width = Width;
            Favorite.Height = Height / 6;

            Exit.Width = Width;
            Exit.Height = Height / 6;


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


        
        private void Back_Click(object sender, RoutedEventArgs e) // 뒤로
        {
            MainWindow.webBrowser.GoBack();
        }



        // 키보드 이벤트 API
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        const int KEYDOWN = 0x0000;
        const int KEYUP = 0x0002;

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

        Keyboard dlg;
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            /*
            
           
            url = sb.ToString();
            Uri myUri = new Uri(url);
            string host = myUri.Host;
            //MessageBox.Show("here 1 = " + host);

            */

            dlg = new Keyboard();
            dlg.Closed += new EventHandler(Keyboard_Closed);
            dlg.Show(); // 키보드 열기


        }


        IHTMLDocument2 doc2;
        IHTMLDocument3 doc3;
        void Keyboard_Closed(object sender, EventArgs e)
        {

            doc2 = (IHTMLDocument2)MainWindow.webBrowser.Document;
            

            
            // Cookie 읽기
            string cookie = doc2.cookie;
            Console.WriteLine("Cookie: {0}", cookie);

            // Document 속성 읽기
            string title = doc2.title;
            string url = doc2.url;
            Console.WriteLine("{0} - {1}", url, title);
            
            //string title = doc2.title;
            if(title.IndexOf("Google") != -1)
            {
                // 특정 Element 컨트롤
                doc3 = (IHTMLDocument3)MainWindow.webBrowser.Document;
                IHTMLElement q = doc3.getElementsByName("q").item("q", 0);
                q.setAttribute("value", Clipboard.GetText());

                IHTMLFormElement form_google = doc2.forms.item(Type.Missing, 0);
                form_google.submit();

            }

            else if(title.IndexOf("NAVER") != -1)
            {
                doc3 = (IHTMLDocument3)MainWindow.webBrowser.Document;
                IHTMLElement query = doc3.getElementsByName("query").item("query", 0);
                //검색어 셋팅
                query.setAttribute("value", Clipboard.GetText());

                //네이버검색버튼 : search_btn
               doc3.getElementById("search_btn").click();
               // IHTMLFormElement form_naver = doc2.forms.item(Type.Missing, 0);
               // form_naver.submit();
            }

            else if(title.IndexOf("Daum") != -1)
            {
                // 특정 Element 컨트롤
                doc3 = (IHTMLDocument3)MainWindow.webBrowser.Document;
                IHTMLElement q_daum = doc3.getElementsByName("q").item("q", 0);
                q_daum.setAttribute("value", Clipboard.GetText());

                IHTMLFormElement form_daum = doc2.forms.item(Type.Missing, 0);
                form_daum.submit();
                //doc3.getElementById("searchSubmit").click();
            }
                

        }
        
        /*// 앞으로
        private void Forward_Click(object sender, RoutedEventArgs e) 
        {
            MainWindow.webBrowser.GoForward();
        }
        */

        /*// 새로고침
      private void Re_Click(object sender, RoutedEventArgs e)
      {
          MainWindow.webBrowser.Refresh();
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

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.webBrowser.Quit();
            Close();
        }
    }
}

