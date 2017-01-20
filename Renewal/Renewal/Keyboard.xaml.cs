using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Renewal
{
    public partial class Keyboard : Window
    {
        // 버튼 크기 결정
        double ButtonWidth = SystemParameters.WorkArea.Right / 10; // 버튼 너비는 해상도 너비의 1/10
        double ButtonHeight = SystemParameters.WorkArea.Bottom / 6; // 버튼 높이는 해상도 너비의 1/6

        // 키보드 이벤트 API
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        // Virtual-Key Code
        public const int KEYEVENTF_KEYDOWN = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int DOT = 0xBE; // '.' flag

        // for문으로 qwerty 순서로 버튼을 생성하기 위한 문자별 array
        static string korean = "ㅂㅈㄷㄱㅅㅛㅕㅑㅐㅔㅁㄴㅇㄹㅎㅗㅓㅏㅣㅋㅌㅊㅍㅠㅜㅡ";
        static string qwerty = "QWERTYUIOPASDFGHJKLZXCVBNM";
        static string special = "!@#$%^&*()~-=+[]<>?/:;'\"\\|,.";
        
        public Keyboard()
        {
            InitializeComponent();

            // Panel 사이즈, 위치 조정
            topPanel.Height = ButtonHeight;
            leftPanel.Width = ButtonWidth;
            leftPanel.Height = ButtonHeight * 2;
            leftPanel.Margin = new Thickness(0, ButtonHeight, 0, 0);
            rightPanel.Width = ButtonWidth * 2;
            rightPanel.Height = ButtonHeight * 2;
            rightPanel.Margin = new Thickness(ButtonWidth * 8, ButtonHeight, 0, 0);
            koreanPanel.Height /= 2;
            koreanPanel.Margin = new Thickness(0, ButtonHeight * 3, 0, 0);
            englishPanel.Height /= 2;
            englishPanel.Margin = new Thickness(0, ButtonHeight * 3, 0, 0);
            specialPanel.Height /= 2;
            specialPanel.Margin = new Thickness(0, ButtonHeight * 3, 0, 0);

            // TextBox 사이즈, 위치 조정
            textBox.Width = ButtonWidth * 7;
            textBox.Height = ButtonHeight * 2;
            textBox.Margin = new Thickness(ButtonWidth, ButtonHeight, 0, 0);

            // 숫자 버튼 생성(Top Pannel)
            for (var i = 0; i <= 9; i++)
            {
                topPanel.Children.Add(new Button { Content = i.ToString(), Tag = "Digit", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
            }

            // 좌측 시스템 버튼 생성(Left Panel)
            leftPanel.Children.Add(new Button { Content = "What", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
            leftPanel.Children.Add(new Button { Content = "Shift", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });

            // 우측 시스템 버튼 생성(Right Panel)
            Button specialButton = new Button { Content = "★", Tag = "SpecialButton", Width = ButtonWidth, Height = ButtonHeight, Focusable = false };
            rightPanel.Children.Add(new Button { Content = System.Windows.Forms.Keys.Back, Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
            rightPanel.Children.Add(specialButton);
            rightPanel.Children.Add(new Button { Content = System.Windows.Forms.Keys.Enter, Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
            rightPanel.Children.Add(new Button { Content = "OK", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });

            // 영어 버튼 생성(English Pannel)
            for (int i = 0; i < qwerty.Length; i++)
            {
                englishPanel.Children.Add(new Button { Content = qwerty[i], Tag = "Alpha", Width = ButtonWidth, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
                if (qwerty[i] == 'L')
                    englishPanel.Children.Add(new Button { Content = "Korean", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                if (qwerty[i] == 'V')
                    englishPanel.Children.Add(new Button { Content = ' ', Tag = "System", Width = ButtonWidth * 2, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
            }
            englishPanel.Children.Add(new Button { Content = '.', Tag = "Special", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });

            // 한글 버튼 생성(Korean Pannel)
            for (int i = 0; i < korean.Length; i++)
            {
                koreanPanel.Children.Add(new Button { Content = korean[i], Tag = "Korean", Width = ButtonWidth, Height = ButtonHeight, Focusable = false, Background = System.Windows.Media.Brushes.White });
                if (korean[i] == 'ㅣ')
                    koreanPanel.Children.Add(new Button { Content = "English", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
                if (korean[i] == 'ㅍ')
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

            // 한영 전환 버튼
            changeButton.Width = ButtonWidth;
            changeButton.Height = ButtonHeight;
            changeButton.Margin = new Thickness(ButtonWidth * 9, ButtonHeight * 4, 0, 0);

            // 각 버튼과 Button Click method 연결
            foreach (Button button in topPanel.Children)
                button.Click += Button_Click;
            foreach (Button button in leftPanel.Children)
                button.Click += Button_Click;
            foreach (Button button in rightPanel.Children)
                button.Click += Button_Click;
            foreach (Button button in koreanPanel.Children)
                button.Click += Button_Click;
            foreach (Button button in englishPanel.Children)
                button.Click += Button_Click;
            foreach (Button button in specialPanel.Children)
                button.Click += Button_Click;

            textBox.Focus(); // TextBox에 포커스 맞춤
            specialPanel.Visibility = Visibility.Hidden; // 특수문자 panel 숨기기(초기값: 한글)
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button; // 각 버튼의 데이터를 button 변수로 가져옴
            string content = button.Content.ToString();

            // OK 버튼 클릭시 textBox의 내용을 Clipboard에 복사하고 키보드 종료
            if (content == "OK")
            {
                Clipboard.SetText(textBox.Text);
                this.Close();
            }
            else if (button.Tag.ToString() == "SpecialButton")
            {
                specialPanel.Visibility = Visibility.Visible;
                if (changeButton.Content.ToString() == "한")
                    changeButton.Content = "영";
                else
                    changeButton.Content = "한";
                changeButton.Margin = new Thickness(ButtonWidth * 9, ButtonHeight, 0, 0);
            }
            // 시스템 키
            else if (button.Tag.ToString() == "System")
            {
                keybd_event(Convert.ToByte(button.Content), 0, KEYEVENTF_KEYDOWN, 0);
                keybd_event(Convert.ToByte(button.Content), 0, KEYEVENTF_KEYUP, 0);
            }
            // 클릭한 버튼이 한글일 경우
            else if (button.Tag.ToString() == "Korean")
            {
                InputMethod.Current.ImeConversionMode = ImeConversionModeValues.Native;
                keybd_event((byte)KoreanToAlpha(content[0]), 0, KEYEVENTF_KEYDOWN, 0);
                keybd_event((byte)KoreanToAlpha(content[0]), 0, KEYEVENTF_KEYUP, 0);
            }
            // 클릭한 버튼이 영어일 경우
            else if (button.Tag.ToString() == "Alpha")
            {
                InputMethod.Current.ImeConversionMode = ImeConversionModeValues.Alphanumeric;
                keybd_event((byte)content[0], 0, KEYEVENTF_KEYDOWN, 0);
                keybd_event((byte)content[0], 0, KEYEVENTF_KEYUP, 0);
            }
            // 그 외 숫자, 특수문자
            else
            {
                textBox.Text += button.Content.ToString();
                textBox.CaretIndex = textBox.Text.Length;
            }

        }
        // 한글 위치에 해당하는 한글로 변환 ex. 'ㅁ' -> 'A'
        public static char KoreanToAlpha(char ch)
        {
            int index = korean.IndexOf(ch);

            return qwerty[index];
        }

        // 한영 전환 버튼 클릭시
        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            string content = changeButton.Content.ToString();

            if (content == "영")
            {
                koreanPanel.Visibility = Visibility.Hidden;
                specialPanel.Visibility = Visibility.Hidden;
                changeButton.Content = "한";
                changeButton.Margin = new Thickness(ButtonWidth * 9, ButtonHeight * 4, 0, 0);
            }
            else
            {
                specialPanel.Visibility = Visibility.Hidden;
                koreanPanel.Visibility = Visibility.Visible;
                changeButton.Content = "영";
                changeButton.Margin = new Thickness(ButtonWidth * 9, ButtonHeight * 4, 0, 0);
            }
        }
    }
}
