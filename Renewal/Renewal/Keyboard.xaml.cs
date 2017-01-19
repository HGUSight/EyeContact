using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//using System.Windows.Forms;

namespace Renewal
{
    /// <summary>
    /// Keyboard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Keyboard : Window
    {


        double ButtonWidth = SystemParameters.MaximizedPrimaryScreenWidth / 10;
        double ButtonHeight = SystemParameters.MaximizedPrimaryScreenHeight / 6;

        private const int ButtonSize = 60;
        private const int WsExNoactivate = 0x08000000;
        private const int GwlExstyle = -20;

        // 키보드 이벤트 API
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_KEYDOWN = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int DOT = 0xBE;

        static string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string korean = "ㅁㅠㅊㅇㄷㄹㅎㅗㅑㅓㅏㅣㅡㅜㅐㅔㅂㄱㄴㅅㅕㅍㅈㅌㅛㅋ";
        static string special = "!@#$%^&*()~-=+[]<>?/:;'\"\\|,.";
        static string qwerty = "QWERTYUIOPASDFGHJKLZXCVBNM";

        string previousState = "Korean";

        public Keyboard()
        {
            InitializeComponent();

            // Panel 사이즈, 위치 조정
            topPanel.Height /= 2;
            koreanPanel.Height /= 2;
            koreanPanel.Margin = new Thickness(0, topPanel.Height, 0, 0);
            alphaPanel.Height /= 2;
            alphaPanel.Margin = new Thickness(0, topPanel.Height, 0, 0);
            specialPanel.Height /= 2;
            specialPanel.Margin = new Thickness(0, topPanel.Height, 0, 0);

            // 특수문자 입력시 특수문자 버튼 부분에 한/영 전환 버튼이 있도록(기본값: Hidden)
            alphaButton.Width = ButtonWidth;
            alphaButton.Height = ButtonHeight;
            alphaButton.Margin = new Thickness(ButtonWidth * 9, ButtonHeight, 0, 0);
            alphaButton.Children.Add(new Button { Content = "English", Tag = "Special", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
            koreanButton.Width = ButtonWidth;
            koreanButton.Height = ButtonHeight;
            koreanButton.Margin = new Thickness(ButtonWidth * 9, ButtonHeight, 0, 0);
            koreanButton.Children.Add(new Button { Content = "Korean", Tag = "Special", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });



            // TextBox 사이즈, 위치 조정
            textBox.Width = ButtonWidth * 7;
            textBox.Height = ButtonHeight * 2;
            textBox.Margin = new Thickness(ButtonWidth, ButtonHeight, 0, 0);

            // 숫자 버튼 생성(Top Pannel)
            for (var i = 0; i <= 9; i++)
            {
                topPanel.Children.Add(new Button { Content = i.ToString(), Tag = "Digit", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
            }

            // 시스템 버튼 생성(Top Pannel)
            for (int i = 0; i <= 19; i++)
            {
                if (i == 0)
                    topPanel.Children.Add(new Button { Content = "What", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                else if (i == 8)
                    topPanel.Children.Add(new Button { Content = System.Windows.Forms.Keys.Back, Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                else if (i == 9)
                    topPanel.Children.Add(new Button { Content = "Special", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                else if (i == 10)
                    topPanel.Children.Add(new Button { Content = "Shift", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                else if (i == 18)
                    topPanel.Children.Add(new Button { Content = System.Windows.Forms.Keys.Enter, Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                else if (i == 19)
                    topPanel.Children.Add(new Button { Content = "OK", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                else
                    topPanel.Children.Add(new Button { Content = "Empty", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
            }

            // 영어 버튼 생성(Alpha Pannel)
            for (var c = 'A'; c <= 'Z'; c++)
            {
                alphaPanel.Children.Add(new Button { Content = ConvertToQwerty(c), Tag = "Alpha", Width = ButtonWidth, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
                if (AlphaToKorean(ConvertToQwerty(c)) == 'ㅣ')
                    alphaPanel.Children.Add(new Button { Content = "Korean", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                if (AlphaToKorean(ConvertToQwerty(c)) == 'ㅍ')
                    alphaPanel.Children.Add(new Button { Content = ' ', Tag = "System", Width = ButtonWidth * 2, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
            }

            alphaPanel.Children.Add(new Button { Content = '.', Tag = "Special", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });

            // 한글 버튼 생성(Korean Pannel)
            for (var c = 'A'; c <= 'Z'; c++)
            {
                koreanPanel.Children.Add(new Button { Content = AlphaToKorean(ConvertToQwerty(c)), Tag = "Korean", Width = ButtonWidth, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
                if (AlphaToKorean(ConvertToQwerty(c)) == 'ㅣ')
                    koreanPanel.Children.Add(new Button { Content = "English", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                if (AlphaToKorean(ConvertToQwerty(c)) == 'ㅍ')
                    koreanPanel.Children.Add(new Button { Content = ' ', Tag = "System", Width = ButtonWidth * 2, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
            }
            koreanPanel.Children.Add(new Button { Content = '.', Tag = "Special", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });

            // 특수문자 버튼 생성(Speical Pannel)

            for (int i = 0; i < special.Length; i++)
            {
                specialPanel.Children.Add(new Button { Content = special[i], Tag = "Special", Width = ButtonWidth, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
                if (special[i] == '"')
                    specialPanel.Children.Add(new Button { Content = ' ', Tag = "Special", Width = ButtonWidth * 2, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
            }
            specialPanel.Children.Add(new Button { Content = '.', Tag = "Special", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });


            // 각 버튼과 Button Click method 연결
            foreach (Button button in topPanel.Children)
            {
                button.Click += Button_Click;
              
            }

            foreach (Button button in koreanPanel.Children)
            {
                button.Click += Button_Click;
            }

            foreach (Button button in alphaPanel.Children)
            {
                button.Click += Button_Click;
            }

            foreach (Button button in specialPanel.Children)
            {
                button.Click += Button_Click;
            }

            foreach (Button button in alphaButton.Children)
            {
                button.Click += Button_Click;
            }

            foreach (Button button in koreanButton.Children)
            {
                button.Click += Button_Click;
            }

            // TextBox에 포커스 맞춤
            textBox.Focus();
            // 특수문자 panel 숨기기(초기값: 한글)
            specialPanel.Visibility = Visibility.Hidden;
            alphaButton.Visibility = Visibility.Hidden;
            koreanButton.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button; // 각 버튼의 데이터를 button 변수로 가져옴
            var chars = button.Content.ToString().ToCharArray(); // 버튼 content의 내용을 char array로 받아옴

            // 한영 전환 버튼 클릭시 Korean Panel <-> English Panel 변경
            if (button.Content.ToString() == "English")
            {
                koreanPanel.Visibility = Visibility.Hidden;
                specialPanel.Visibility = Visibility.Hidden;
                alphaButton.Visibility = Visibility.Hidden;
                koreanButton.Visibility = Visibility.Hidden;
                previousState = "English";

            }
            else if (button.Content.ToString() == "Korean")
            {
                specialPanel.Visibility = Visibility.Hidden;
                koreanPanel.Visibility = Visibility.Visible;
                alphaButton.Visibility = Visibility.Hidden;
                koreanButton.Visibility = Visibility.Hidden;
                previousState = "Korean";
            }
            else if (button.Content.ToString() == "Special")
            {
                specialPanel.Visibility = Visibility.Visible;
                if (previousState == "Korean")
                {
                    koreanButton.Visibility = Visibility.Visible;
                }
                else
                {
                    alphaButton.Visibility = Visibility.Visible;
                }

            }
            else if (button.Content.ToString() == "OK")
            {
                Clipboard.SetText(textBox.Text);
                //var myWindowHandler = Process.GetCurrentProcess().MainWindowHandle;
                //ShowWindow(myWindowHandler, 5);
                //SetForegroundWindow(myWindowHandler);
                PlayAround();
                Clipboard.GetText();
                //this.Close();
            }
            // 클릭한 버튼이 한글일 경우
            else if (button.Tag.ToString() == "Korean")
            {
                InputMethod.Current.ImeConversionMode = ImeConversionModeValues.Native;
                keybd_event((byte)KoreanToAlpha(chars[0]), 0, KEYEVENTF_KEYDOWN, 0);
                keybd_event((byte)KoreanToAlpha(chars[0]), 0, KEYEVENTF_KEYUP, 0);
            }
            // 클릭한 버튼이 영어일 경우
            else if (button.Tag.ToString() == "Alpha")
            {
                InputMethod.Current.ImeConversionMode = ImeConversionModeValues.Alphanumeric;
                keybd_event((byte)chars[0], 0, KEYEVENTF_KEYDOWN, 0);
                keybd_event((byte)chars[0], 0, KEYEVENTF_KEYUP, 0);
            }
            // 클릭한 버튼이 숫자일 경우
            else if (button.Tag.ToString() == "Digit")
            {
                keybd_event((byte)chars[0], 0, KEYEVENTF_KEYDOWN, 0);
                keybd_event((byte)chars[0], 0, KEYEVENTF_KEYUP, 0);
            }
            // 시스템 키
            else if (button.Tag.ToString() == "System")
            {
                keybd_event(Convert.ToByte(button.Content), 0, KEYEVENTF_KEYDOWN, 0);
                keybd_event(Convert.ToByte(button.Content), 0, KEYEVENTF_KEYUP, 0);
            }
            // 그 외 특수문자
            else
            {
                textBox.Text += button.Content.ToString();
                textBox.CaretIndex = textBox.Text.Length;
            }

        }

        // 영문자 위치에 해당하는 한글로 변환 ex. 'A' -> 'ㅁ'
        public static char AlphaToKorean(char ch)
        {
            int index = alpha.IndexOf(ch);

            return korean[index];
        }

        // 한글 위치에 해당하는 한글로 변환 ex. 'ㅁ' -> 'A'
        public static char KoreanToAlpha(char ch)
        {
            int index = korean.IndexOf(ch);

            return alpha[index];
        }

        // 한글 위치에 해당하는 한글로 변환 ex. 'ㅁ' -> 'A'
        public static char ConvertToQwerty(char ch)
        {
            int index = alpha.IndexOf(ch);
            return qwerty[index];
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImportAttribute("User32.DLL")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        public void PlayAround()
        {
            Process[] processList = Process.GetProcesses();

            foreach (Process theProcess in processList)
            {
                string processName = theProcess.ProcessName;
                string mainWindowTitle = theProcess.MainWindowTitle;
                SetFocus(new HandleRef(null, theProcess.MainWindowHandle));
                Clipboard.GetText();
            }

        }



    }
}
