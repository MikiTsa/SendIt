using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;

namespace SendIt
{
    public class SpeechManager
    {
        private SpeechRecognitionEngine _recognizer;
        private SpeechSynthesizer _synthesizer;
        private bool _isListening;

        public event EventHandler OpenAddContact;
        public event EventHandler AddDefaultContact;
        public event EventHandler<string> SelectContact;
        public event EventHandler RemoveContact;
        public event EventHandler EditContact;
        public event EventHandler SendMessage;
        public event EventHandler ChangeStatus;

        private List<string> _availableCommands = new List<string>();
        public List<string> AvailableCommands => _availableCommands;

        public SpeechManager()
        {
            InitializeSpeechRecognition();
            InitializeSpeechSynthesis();
        }

        private void InitializeSpeechRecognition()
        {
            try
            {
                _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));

                var choices = new Choices();
                choices.Add("open add contact");
                choices.Add("add default contact");
                choices.Add("select contact one");
                choices.Add("select contact two");
                choices.Add("select contact three");
                choices.Add("select first contact");
                choices.Add("select second contact");
                choices.Add("select third contact");
                choices.Add("remove contact");
                choices.Add("delete contact");
                choices.Add("edit contact");
                choices.Add("modify contact");
                choices.Add("send message");
                choices.Add("send hello");
                choices.Add("change status");

                var gb = new GrammarBuilder();
                gb.Append(choices);
                var grammar = new Grammar(gb);

                _recognizer.LoadGrammar(grammar);
                _recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                _recognizer.SetInputToDefaultAudioDevice();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing speech recognition: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeSpeechSynthesis()
        {
            try
            {
                _synthesizer = new SpeechSynthesizer();
                _synthesizer.SetOutputToDefaultAudioDevice();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing speech synthesis: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.4)
                return;

            string command = e.Result.Text.ToLower();

            switch (command)
            {
                case "open add contact":
                    Speak("Opening add contact window");
                    OpenAddContact?.Invoke(this, EventArgs.Empty);
                    break;

                case "add default contact":
                    Speak("Adding a default contact");
                    AddDefaultContact?.Invoke(this, EventArgs.Empty);
                    break;

                case "select contact one":
                case "select first contact":
                    Speak("First contact selected");
                    SelectContact?.Invoke(this, "0");
                    break;

                case "select contact two":
                case "select second contact":
                    Speak("Second contact selected");
                    SelectContact?.Invoke(this, "1");
                    break;

                case "select contact three":
                case "select third contact":
                    Speak("Third contact selected");
                    SelectContact?.Invoke(this, "2");
                    break;

                case "remove contact":
                case "delete contact":
                    Speak("Contact has been removed");
                    RemoveContact?.Invoke(this, EventArgs.Empty);
                    break;

                case "edit contact":
                case "modify contact":
                    Speak("Opening edit contact window");
                    EditContact?.Invoke(this, EventArgs.Empty);
                    break;

                case "send message":
                    Speak("Message sent");
                    SendMessage?.Invoke(this, EventArgs.Empty);
                    break;

                case "send hello":
                    Speak("Hello message sent");
                    SendMessage?.Invoke(this, EventArgs.Empty);
                    break;

                case "change status":
                    Speak("Status has been changed");
                    ChangeStatus?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        public void Speak(string message)
        {
            try
            {
                _synthesizer.SpeakAsync(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in speech synthesis: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void StartListening()
        {
            if (!_isListening)
            {
                try
                {
                    Speak("Voice commands activated");
                    _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                    _isListening = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting speech recognition: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void StopListening()
        {
            if (_isListening)
            {
                try
                {
                    Speak("Voice commands deactivated");
                    _recognizer.RecognizeAsyncStop();
                    _isListening = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error stopping speech recognition: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void UpdateAvailableCommands(bool canEditOrRemove)
        {
            _availableCommands.Clear();

            _availableCommands.Add("open add contact");
            _availableCommands.Add("add default contact");
            _availableCommands.Add("select first contact");
            _availableCommands.Add("select second contact");
            _availableCommands.Add("select third contact");

            if (canEditOrRemove)
            {
                _availableCommands.Add("remove contact");
                _availableCommands.Add("edit contact");
                _availableCommands.Add("send message");
                _availableCommands.Add("change status");
            }
        }

        public void Dispose()
        {
            StopListening();
            _recognizer?.Dispose();
            _synthesizer?.Dispose();
        }
    }
}