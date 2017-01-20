using System;
using System.Text;
using System.Windows;
using System.Windows.Forms;
//move mouse
using System.Runtime.InteropServices;
//Tobii
using Tobii.EyeX.Framework;
using System.Diagnostics;
using System.Windows.Interop;

namespace Renewal
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {


        public MainWindow()
        {

            InitializeComponent();

            // calculate screen size
            Width = SystemParameters.MaximizedPrimaryScreenWidth / 8;
            Height = SystemParameters.MaximizedPrimaryScreenHeight;

            double ButtonWidth = SystemParameters.MaximizedPrimaryScreenWidth / 8;
            double ButtonHeight = SystemParameters.MaximizedPrimaryScreenHeight / 6;

            Left = ButtonWidth * 7;
            Top = 0;

            Move_Mouse();

            Size.Equals(Width, Height);

            //키보드 후킹 --> up key를 누르면 마우스 왼쪽 버튼 클릭이 작동
            SetHook();


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


        // move mouse
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        // coordinate Gaze Point
        public static int userCoordinateX = 0;
        public static int userCoordinateY = 0;

        // make sure SetCoordinate is opened.
        public static bool isCoordinate = false;

        // move_mouse event?
        private void Move_Mouse()
        {
            var lightlyFilteredGazeDataStream = ((App)System.Windows.Application.Current)._eyeXHost.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);
            lightlyFilteredGazeDataStream.Next += (s, e) => SetCursorPos((int)e.X + userCoordinateX, (int)e.Y + userCoordinateY);

            var eyePositionDataStream = ((App)System.Windows.Application.Current)._eyeXHost.CreateEyePositionDataStream();
        }
        //**********************************************


        // button click event 마우스 버튼, 키보드 버튼이 나타남
        private void Mouse_Click(object sender, RoutedEventArgs e)
        {
            Mouse dlg = new Renewal.Mouse();
            dlg.Show();
        }

        private void Keyboard_Click(object sender, RoutedEventArgs e)
        {
            Keyboard dlg = new Renewal.Keyboard();
            dlg.Closed += new EventHandler(Keyboard_Closed);
            dlg.Show();
        }
        //**********************************************


        // 키보드 이벤트 API
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        //  키보드 종료 되면서 clipboard에 복사된 텍스트가 ctrl + v 됨
        void Keyboard_Closed(object sender, EventArgs e)
        {
            keybd_event((byte)0x11, 0, 0, 0);
            keybd_event((byte)'V', 0, 0, 0);
            keybd_event((byte)0x11, 0, 0x0002, 0);
            keybd_event((byte)'V', 0, 0x0002, 0);
        }

        //키보드 후킹
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;

        private LowLevelKeyboardProc _proc = hookProc;

        private static IntPtr hhook = IntPtr.Zero;
        // 전역후킹 변수들

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int LEFTDOWN = 0x0002;
        public const int LEFTUP = 0x0004;
        public const int RIGHTDOWN = 0x0008;
        public const int RIGHTUP = 0x0010;
        public const int MOUSEWHEEL = 0x0800;
        //마우스 후킹 변수들 

        public static int mouseEvent_var;
        public enum mouseEvent
        {
            LCLICKED = 0,
            RCLICKED = 1,
            DOUBLECLICKED = 2,
            DRAGCLICKED = 3
        }
        // 마우스 이벤트 변수들

        public void SetHook() // 후킹을 시작
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0); // _porc 함수로 넘어감 = hookPorc로 넘어감
        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN) // keydown이 인지
            {
                int vkCode = Marshal.ReadInt32(lParam);


                if (vkCode.ToString() == "124") // 38: up key, 81: q key, 124: F13 key(shift+F1)
                                               // http://cherrytree.at/misc/vk.htm 참조
                {
                    switch (mouseEvent_var)
                    {
                        case (int)mouseEvent.LCLICKED:
                            mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 왼쪽 클릭 
                            mouse_event(LEFTUP, 0, 0, 0, 0);
                            return (IntPtr)1; // return 1: vkCode(up 키) 메세지를 메세지 큐로 전달하지 않음 (=up 키가 작동되지 않음)
                                              // return CallNextHookEx(hhook, code, (int)wParam, lParam); : 해당 메세지를 큐로 전달함
                        case (int)mouseEvent.RCLICKED:
                            mouse_event(RIGHTDOWN, 0, 0, 0, 0); // 마우스 오른쪽 클릭 
                            mouse_event(RIGHTUP, 0, 0, 0, 0);
                            mouseEvent_var = (int)mouseEvent.LCLICKED;
                            return (IntPtr)1;

                        case (int)mouseEvent.DOUBLECLICKED:
                            mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 더블 클릭 
                            mouse_event(LEFTUP, 0, 0, 0, 0);
                            mouse_event(LEFTDOWN, 0, 0, 0, 0); 
                            mouse_event(LEFTUP, 0, 0, 0, 0);
                            mouseEvent_var = (int)mouseEvent.LCLICKED;
                            return (IntPtr)1;

                        case (int)mouseEvent.DRAGCLICKED:
                            mouse_event(LEFTDOWN, 0, 0, 0, 0); // 마우스 드래그 
                            mouseEvent_var = (int)mouseEvent.LCLICKED;
                            return (IntPtr)1;
                    }
                    
                }

               

                //when user coordinates gaze Point and mouse position. 
                if (isCoordinate)
                {
                    switch (vkCode)
                    {
                        //A key- 왼쪽으로 좌표점 이동시키기
                        case 65:
                            userCoordinateX -= 5;
                            break;
                        //D key - 오른쪽으로 좌표점 이동시키기 
                        case 68:
                            userCoordinateX += 5;
                            break;
                        //W key - 위로 이동시키기
                        case 87:
                            userCoordinateY -= 5;
                            break;
                        //S key - 아래로 이동시키기
                        case 83:
                            userCoordinateY += 5;
                            break;
                        default:
                            break;
                    }
                }

                else
                    return CallNextHookEx(hhook, code, (int)wParam, lParam);

                
            }
            return CallNextHookEx(hhook, code, (int)wParam, lParam);
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            UnHook();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            SetHook();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Setting dlg = new Renewal.Setting();
            dlg.Show();
        }
        //**********************************************




    }
}
