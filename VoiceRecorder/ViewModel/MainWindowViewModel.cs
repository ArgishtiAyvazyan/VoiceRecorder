using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VoiceRecorder.Annotations;
using VoiceRecorder.AudioEngine;
using VoiceRecorder.Model;

namespace VoiceRecorder.ViewModel
{
    /// <summary>
    /// Implementation of ICommand interface based on Relay Command pattern.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    /// <summary>
    /// The main View Model class.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly ApplicationModel _applicationModel;
        private readonly string _fileName = "tmp_audio.mp3";

        private bool _isPlaying;
        private bool _disablePlaying;
        private bool _isRecording;
        private bool _disableRecording;

        /// <summary>
        /// The IsPlaying property show is now in the playing process.
        /// </summary>
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        /// <summary>
        /// The IsRecording property show is now in the recording process.
        /// </summary>
        public bool IsRecording
        {
            get => _isRecording;
            set
            {
                _isRecording = value;
                OnPropertyChanged(nameof(IsRecording));
            }
        }

        /// <summary>
        /// The PlayPauseCommand Commands.
        /// if the current state is Stopped or Paused switches to Playing audio state.
        /// if the current state is Playing switches to Paused audio state.
        /// </summary>
        public ICommand PlayPauseCommand { get; private set; }

        /// <summary>
        /// The StartPauseRecordingCommand Commands.
        /// if the current state is Stopped or Paused switches to Recording state.
        /// if the current state is Recording switches to Paused state.
        /// </summary>
        public ICommand StartPauseRecordingCommand { get; private set; }

        /// <summary>
        /// The StopCommand Commands.
        /// if the current state is Playing or Recording switches to Stopped state.
        /// </summary>
        public ICommand StopCommand { get; private set; }


        /// <summary>
        /// The list of available devices (Microphones).
        /// </summary>
        public ObservableCollection<IDevice> Devices
        {
            get => new ObservableCollection<IDevice>(_applicationModel.Recorder.Devices);
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The selected device, which will be used for audio recording.
        /// </summary>
        public IDevice SelectedDevice
        {
            get => _applicationModel.Recorder.ActiveDevice;
            set
            {
                _applicationModel.Recorder.ActiveDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }


        public MainWindowViewModel()
        {
            _applicationModel = new ApplicationModel(new MP3Recorder(), new MP3Player());

            IsPlaying = false;
            IsRecording = false;
            InitCommands();
        }

        /// <summary>
        /// Initialize the PlayPauseCommand, StartPauseRecordingCommand and StopCommand commands
        /// </summary>
        private void InitCommands()
        {
            PlayPauseCommand = new RelayCommand((x) =>
            {
                if (_disablePlaying)
                {
                    return;
                }

                if (IsPlaying)
                {
                    _applicationModel.PausePlaying();
                }
                else
                {
                    if (!_applicationModel.Player.IsActive())
                    {
                        _applicationModel.Player.FileName = _fileName;
                        _applicationModel.Player.PlayingStoppedEvent += (owner, args) =>
                        {
                            IsPlaying = false;
                            _disableRecording = false;
                        };
                        _disableRecording = true;
                    }
                    _applicationModel.PlayRecord();
                }

                IsPlaying = !IsPlaying;
            });

            StartPauseRecordingCommand = new RelayCommand((x) =>
            {
                if (_disableRecording)
                {
                    return;
                }

                if (IsRecording)
                {
                    _applicationModel.PauseRecording();
                }
                else
                {
                    if (!_applicationModel.Recorder.IsActive())
                    {
                        _applicationModel.Recorder.OutFileName = _fileName;
                    }
                    _applicationModel.StartRecording();
                    _disablePlaying = true;
                }

                IsRecording = !IsRecording;
            });

            StopCommand = new RelayCommand((x) =>
            {
                if (_applicationModel.Player.IsActive())
                {
                    _applicationModel.StopPlaying();
                    IsPlaying = false;
                    _disableRecording = false;
                    return;
                }

                if (_applicationModel.Recorder.IsActive())
                {
                    _applicationModel.StopRecording();

                    IsRecording = false;
                    _disablePlaying = false;
                }
            });
        }

    }
}
