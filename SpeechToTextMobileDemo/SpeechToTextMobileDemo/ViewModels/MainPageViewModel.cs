using System;
using System.Windows.Input;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Plugin.AudioRecorder;
using Prism.Navigation;
using SpeechToTextMobileDemo.Bases;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SpeechToTextMobileDemo.ModelViews
{
    public class MainPageViewModel : ViewModelBase
    {
        const string speechKey = "2744529068354c3390a18c13d9922e00";
        const string speechRegion = "westeurope";

        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService();
        private readonly AudioPlayer audioPlayer = new AudioPlayer();
        public MainPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        private string _speechText;
        public string SpeechText
        {
            get => _speechText;
            set => SetProperty(ref _speechText, value);
        }

        public ICommand StartRecord
        {
            get
            {
                return new Command(async () =>
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        var permissons = await Permissions.CheckStatusAsync<Permissions.Microphone>();
                        var permissons1 = await Permissions.RequestAsync<Permissions.StorageWrite>();

                        if (permissons != PermissionStatus.Granted)
                        {
                            permissons = await Permissions.RequestAsync<Permissions.Microphone>();
                        }
                        if (permissons != PermissionStatus.Granted)
                        {
                            return;

                        }
                    }


                    if (!audioRecorderService.IsRecording)
                    {
                        await audioRecorderService.StartRecording();
                    }

                });
            }
        }

        public ICommand StopRecord
        {
            get
            {
                return new Command(async () =>
                {
                    if (audioRecorderService.IsRecording)
                    {
                        await audioRecorderService.StopRecording();
                    }
                });
            }
        }

        public ICommand GetRecord
        {
            get
            {
                return new Command(async () =>
                {

                    var deneme = audioRecorderService.GetAudioFilePath();
                    if (!audioRecorderService.IsRecording && deneme != null)
                    {
                        audioPlayer.Play(audioRecorderService.GetAudioFilePath());


                        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
                        speechConfig.SpeechRecognitionLanguage = "tr-TR";


                        using (var audioConfig = AudioConfig.FromWavFileInput(audioRecorderService.GetAudioFilePath()))
                        {
                            using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
                            {
                                Console.WriteLine("Speak into your microphone.");
                                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                                OutputSpeechRecognitionResult(speechRecognitionResult);

                            }

                        }
                    }




                });
            }
        }

        private void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
        {

            if (speechRecognitionResult.Reason == ResultReason.RecognizedSpeech)
            {
                SpeechText = speechRecognitionResult.Text;
            }
            else if (speechRecognitionResult.Reason == ResultReason.NoMatch)
            {
                SpeechText = "NOMATCH: Speech could not be recognized.";

            }
            else if (speechRecognitionResult.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);

                SpeechText = cancellation.Reason.ToString();

                if (cancellation.Reason == CancellationReason.Error)
                {
                    SpeechText = cancellation.ErrorCode + "   " + cancellation.ErrorDetails;
                }
            }

        }

    }
}

