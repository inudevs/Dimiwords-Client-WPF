using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dimiwords_Client_WPF
{
    class TTS
    {
        private static bool isInit = false;
        private static SpeechSynthesizer Speech = new SpeechSynthesizer();

        public static bool Init(int volume = 100, int rate = 0)
        {
            Speech.SetOutputToDefaultAudioDevice();
            var isHelen = false;
            foreach (var voice in Speech.GetInstalledVoices())
            {
                if (voice.VoiceInfo.Name == "Microsoft Server Speech Text to Speech Voice (en-US, Helen)")
                {
                    isHelen = true;
                    break;
                }
            }
            if (!isHelen)
            {
                MessageBox.Show("Voice Helen이 존재하지 않습니다. 제대로 설치 후 실행해주세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Speech.SelectVoice("Microsoft Server Speech Text to Speech Voice (en-US, Helen)");
            Speech.TtsVolume = volume;
            Speech.Rate = rate;
            isInit = isHelen;
            return isHelen;
        }

        public static void Dispose()
        {
            Speech.Dispose();
        }

        public static void Speak(string text)
        {
            if (isInit)
                Speech.Speak(text);
        }
    }
}
