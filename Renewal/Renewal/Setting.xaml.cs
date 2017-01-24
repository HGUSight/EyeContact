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
    /// Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            Width = Application.Current.MainWindow.Width;
            Height = Application.Current.MainWindow.Height;

            SetCoordinate.Width = Width;
            SetCoordinate.Height = Height / 6;

            Back.Width = Width;
            Back.Height = Height / 6;


            Left = Application.Current.MainWindow.Left; 
            Top = 0;
        }

        private void SetCoordinate_Click(object sender, RoutedEventArgs e)
        {
            SetCoordinate dlg = new Renewal.SetCoordinate();
            dlg.Show();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
