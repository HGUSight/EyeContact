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

        [DllImport("user32.dll", SetLastError = true)]
        static extern void Keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        const int KEYDOWN = 0;
        const int KEYUP = 0x0002;

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //for keyboard event trigger

           const byte ALT = 12;
           const byte Left = 25;

            Keybd_event((byte)0x12, 0, 0, 0);
            Keybd_event((byte)0x25, 0, 0, 0);
            Keybd_event((byte)0x12, 0, 0x0002, 0);
            Keybd_event((byte)0x25, 0, 0x0002, 0);
          
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
