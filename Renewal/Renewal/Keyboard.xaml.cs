using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// stt
using System.IO;
using System.Net;
using CUETools.Codecs;
using CUETools.Codecs.FLAKE;
using NAudio.Wave;
// json parsing
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Renewal
{
    public partial class Keyboard : Window
    {
        #region variable
        // 버튼 크기 결정
        double ButtonWidth = SystemParameters.PrimaryScreenWidth / 10;
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

        // voice click
        private string path = @"C:\Audio\";
        private string rawFile = @"raw.wav";
        private string wavFile = @"audio.wav";
        private string flacFile = @"audio.flac";
        private string flac_path = @"C:\Audio\audio.flac";
        private bool isStart = true;

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);


        #endregion

        #region make-keyboard
        public Keyboard()
        {
            InitializeComponent();

            // Panel 사이즈, 위치 조정
            topPanel.Height = ButtonHeight;
            topPanel.Width = ButtonWidth * 10;
            leftPanel.Width = ButtonWidth;
            leftPanel.Height = ButtonHeight * 2;
            leftPanel.Margin = new Thickness(0, ButtonHeight, 0, 0);
            rightPanel.Width = ButtonWidth * 2;
            rightPanel.Height = ButtonHeight * 2;
            rightPanel.Margin = new Thickness(ButtonWidth * 8, ButtonHeight, 0, 0);
            koreanPanel.Height /= 2;
            koreanPanel.Width = ButtonWidth * 10;
            koreanPanel.Margin = new Thickness(0, ButtonHeight * 3, 0, 0);
            englishPanel.Width = ButtonWidth * 10;
            englishPanel.Height /= 2;
            englishPanel.Margin = new Thickness(0, ButtonHeight * 3, 0, 0);
            specialPanel.Height /= 2;
            specialPanel.Width = ButtonWidth * 10;
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
            leftPanel.Children.Add(new Button { Content = "Speech", Tag = "System", Width = ButtonWidth, Height = ButtonHeight, Focusable = false });
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
        #endregion

        #region button-click
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
            else if (content == "Speech")
            {
                Stt();
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
        #endregion

        #region korean to english
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
        #endregion

        #region Speech to text
        private void Stt()
        {
            System.IO.Directory.CreateDirectory(path);

            if(isStart)
            {
                isStart = false;
                start_record();
            }
            else
            {
                isStart = true;
                end_record();
                convert();
                send();
            }
        }

        private void start_record()
        {
            mciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
            mciSendString("record recsound", "", 0, 0);
        }

        private void end_record()
        {
            mciSendString(@"save recsound " + path + rawFile, "", 0, 0);
            mciSendString("close recsound ", "", 0, 0);
        }

        private void convert()
        {
            using (var reader = new WaveFileReader(path + rawFile))
            {
                var newFormat = new NAudio.Wave.WaveFormat(16000, 16, 1);
                using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                {
                    WaveFileWriter.CreateWaveFile(path + wavFile, conversionStream);
                }
            }

            if (!File.Exists(path + wavFile))
            {
                Console.WriteLine("wav file no!");
            }
            else
            {
                using (FileStream sourceStream = new FileStream(path + wavFile, FileMode.Open))
                {
                    //FileStream sourceStream = new FileStream(path + wavFile, FileMode.Open);
                    WAVReader audioSource = new WAVReader(path + wavFile, sourceStream);

                    //FlakeWriter flac = new FlakeWriter(File.Create(path), audioSource.PCM);

                    AudioBuffer buff = new AudioBuffer(audioSource, 0x10000);
                    FlakeWriter flakeWriter = new FlakeWriter(path + flacFile, audioSource.PCM);

                    flakeWriter.CompressionLevel = 8;
                    while (audioSource.Read(buff, -1) != 0)
                    {
                        flakeWriter.Write(buff);
                    }

                    flakeWriter.Close();
                    audioSource.Close();
                }
            }
        }

        private void send()
        {
            // request
            using (FileStream fileStream = File.OpenRead(flac_path))
            {
                //FileStream fileStream = File.OpenRead(flac_path);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    //MemoryStream memoryStream = new MemoryStream();
                    memoryStream.SetLength(fileStream.Length);
                    fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
                    byte[] BA_AudioFile = memoryStream.GetBuffer();
                    HttpWebRequest _HWR_SpeechToText = null;
                    _HWR_SpeechToText =
                                (HttpWebRequest)HttpWebRequest.Create(
                                    "https://www.google.com/speech-api/v2/recognize?output=json&lang=ko-KR&key=AIzaSyCzsbEmnTv36-aWE5ThgGTnNXuJF-AeLcs");
                    _HWR_SpeechToText.Credentials = CredentialCache.DefaultCredentials;
                    _HWR_SpeechToText.Method = "POST";
                    _HWR_SpeechToText.ContentType = "audio/x-flac; rate=16000";
                    _HWR_SpeechToText.ContentLength = BA_AudioFile.Length;
                    using (Stream stream = _HWR_SpeechToText.GetRequestStream())
                    {
                        //Stream stream = _HWR_SpeechToText.GetRequestStream();
                        stream.Write(BA_AudioFile, 0, BA_AudioFile.Length);
                        stream.Close();

                        HttpWebResponse HWR_Response = (HttpWebResponse)_HWR_SpeechToText.GetResponse();

                        // response 
                        if (HWR_Response.StatusCode == HttpStatusCode.OK)
                        {
                            StreamReader SR_Response = new StreamReader(HWR_Response.GetResponseStream());
                            
                            var result = SR_Response.ReadToEnd();
                            Console.WriteLine("This is result : " + result);

                            var jsons = result.Split('\n');

                            json_parsing(jsons);
                        }
                    }
                }
            }
        }

        private void json_parsing(string[] jsons)
        {
            foreach (var root in jsons)
            {
                dynamic jsonObject = JsonConvert.DeserializeObject(root);
                if (jsonObject == null || jsonObject.result.Count <= 0)
                    continue;

                string json = jsonObject.result[0].alternative.ToString();
                var json_array = JArray.Parse(json);

                int i = 0;
                int max = 0;
                var max_confidence = 0;
                foreach (var a in json_array)
                {
                    if (i == 0)
                    {
                        max = i;
                        max_confidence = jsonObject.result[0].alternative[0].confidence;
                    }
                    else if (jsonObject.result[0].alternative[i].confidence >= max_confidence)
                    {
                        max = i;
                        max_confidence = jsonObject.result[0].alternative[i].confidence;
                    }
                    Console.WriteLine("1, 2, 3 : " + a);
                    i++;
                }
                var text = jsonObject.result[0].alternative[max].transcript;
                Console.WriteLine("test : " + text);
            }
        }
        #endregion
    }
}
