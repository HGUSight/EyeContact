using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//move mouse
using System.Runtime.InteropServices;
//Tobii
using Tobii.EyeX.Framework;

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

            // move mouse
            Move_Mouse();

            // calculate screen size
            Width = Screen.PrimaryScreen.Bounds.Width / 8;
            Height = Screen.PrimaryScreen.Bounds.Height;

            int ButtonWidth = Screen.PrimaryScreen.Bounds.Width / 8;
            int ButtonHeight = Screen.PrimaryScreen.Bounds.Height / 6;

            Size.Equals(Width, Height);

            //키보드 후킹 --> up key를 누르면 마우스 왼쪽 버튼 클릭이 작동
            SetHook();

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
            dlg.Show();
        }
    }
}
