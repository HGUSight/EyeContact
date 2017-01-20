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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace Renewal
{
    /// <summary>
    /// Stt.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Stt : Window
    {
        public Stt()
        {
            InitializeComponent();
        }


        public Stt()
        {
            InitializeComponent();
        }


        public void StoT()
        {
            string path = @"C:\WorkSpace\capstone\EyeContact\Renewal\test.flac";

            FileStream fileStream = File.OpenRead(path);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.SetLength(fileStream.Length);
            fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
            byte[] BA_AudioFile = memoryStream.GetBuffer();
            HttpWebRequest _HWR_SpeechToText = null;
            _HWR_SpeechToText =
                        (HttpWebRequest)HttpWebRequest.Create(
                            "https://www.google.com/speech-api/v2/recognize?output=json&lang=en-us&key=AIzaSyCzsbEmnTv36-aWE5ThgGTnNXuJF-AeLcs");
            _HWR_SpeechToText.Credentials = CredentialCache.DefaultCredentials;
            _HWR_SpeechToText.Method = "POST";
            _HWR_SpeechToText.ContentType = "audio/x-flac; rate=16000";
            _HWR_SpeechToText.ContentLength = BA_AudioFile.Length;
            Stream stream = _HWR_SpeechToText.GetRequestStream();
            stream.Write(BA_AudioFile, 0, BA_AudioFile.Length);
            stream.Close();

            HttpWebResponse HWR_Response = (HttpWebResponse)_HWR_SpeechToText.GetResponse();
            if (HWR_Response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader SR_Response = new StreamReader(HWR_Response.GetResponseStream());
                Console.WriteLine(SR_Response.ReadToEnd());
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StoT();
        }
           

    }
}
