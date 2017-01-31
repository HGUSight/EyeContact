using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Mouse.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Mouse : Window
    {
        public Mouse()
        {
            InitializeComponent();

            Width = Application.Current.MainWindow.Width;
            Height = Application.Current.MainWindow.Height;

            RClick.Width = Width;
            RClick.Height = Height / 6;

            DClick.Width = Width;
            DClick.Height = Height / 6;

            Drag.Width = Width;
            Drag.Height = Height / 6;

            Back.Width = Width;
            Back.Height = Height / 6;


            Left = Application.Current.MainWindow.Left;
            Top = 0;
        }

        private void RClick_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mouseEvent_var = (int)MainWindow.mouseEvent.RCLICKED;
        }

        private void DClick_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mouseEvent_var = (int)MainWindow.mouseEvent.DOUBLECLICKED;
        }

        private void Drag_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mouseEvent_var = (int)MainWindow.mouseEvent.DRAGCLICKED;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
