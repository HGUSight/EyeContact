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


            // calculate screen size
            Width = Screen.PrimaryScreen.Bounds.Width / 8;
            Height = Screen.PrimaryScreen.Bounds.Height;

            int ButtonWidth = Screen.PrimaryScreen.Bounds.Width / 8;
            int ButtonHeight = Screen.PrimaryScreen.Bounds.Height / 6;

            Size.Equals(Width, Height);

        }

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
