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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Renewal
{
    /// <summary>
    /// SpeechRecognition.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SpeechRecognition : Window
    {

        SpeechRecognitionEngine recognizer;
        public SpeechRecognition()
        {
            InitializeComponent();

            using (recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US")))
            {

                // Create and load a dictation grammar.
                recognizer.LoadGrammar(new DictationGrammar());

                // Configure input to the speech recognizer.
                recognizer.SetInputToDefaultAudioDevice();

                //
                RecognitionResult result = recognizer.Recognize();
                string ResultString = "";

                // add all recognized words to the result string
                foreach (RecognizedWordUnit w in result.Words)
                {
                    ResultString += w.Text; // Add words in result string
                    tb_speech.Text = ResultString; // show string in textbox

                }

                // Add a handler for the speech recognized event.
                recognizer.SpeechRecognized += recognizer_SpeechRecognized;


                // Start asynchronous, continuous speech recognition.
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                recognizer.Recognize();

                // Keep the console window open.
                //while (true)
                //{

                //}

            }
        }

        public void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            tb_speech.Text = e.Result.Text;
        }

        private void btn_listen_now_Click_1(object sender, RoutedEventArgs e)
        {
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
        }


    }

   
}
