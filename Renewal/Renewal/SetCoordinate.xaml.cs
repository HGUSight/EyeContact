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

namespace Renewal
{
    /// <summary>
    /// SetCoordinate.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetCoordinate : Window
    {
        public SetCoordinate()
        {
            InitializeComponent();
            SetHook();

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

                //when user coordinates gaze Point and mouse position. 

                    switch (vkCode)
                    {
                        //A key- 왼쪽으로 좌표점 이동시키기
                        case 65:
                            MainWindow.userCoordinateX -= 5;
                            MessageBox.Show("A ");
                            break;
                        //D key - 오른쪽으로 좌표점 이동시키기 
                        case 68:
                            MainWindow.userCoordinateX += 5;
                            break;
                        //W key - 위로 이동시키기
                        case 87:
                            MainWindow.userCoordinateY -= 5;
                            break;
                        //S key - 아래로 이동시키기
                        case 83:
                            MainWindow.userCoordinateY += 5;
                            break;
                        default:
                            break;
                    }
        
                return CallNextHookEx(hhook, code, (int)wParam, lParam); ;
            }
            else
                return CallNextHookEx(hhook, code, (int)wParam, lParam);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        { 
            UnHook();
        }
    }
    
}
