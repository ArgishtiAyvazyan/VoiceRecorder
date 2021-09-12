using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        /// <summary>
        /// The enum ERecordingStatus describes the recorder status.
        /// </summary>
        public enum ERecordingStatus
        {
            Recording = 0,
            Paused,
            Stopped,
        };

        /// <summary>
        /// The enum EPlayStatus describes the player status.
        /// </summary>
        public enum EPlayStatus
        {
            Playing = 0,
            Paused,
            Stopped,
        };

        private readonly ApplicationModel _applicationModel;
        private readonly string _fileName = "tmp_audio.mp3";


        private EPlayStatus _ePlayStatus;
        private ERecordingStatus _eRecordingStatus;
        private bool _recordingNotExists;
        private bool _declineRecordCommand;
        private bool _declinePlayCommand;

        /// <summary>
        /// Determines the Playing status.
        /// </summary>
        public EPlayStatus PlayingStatus
        {
            get => _ePlayStatus;
            set
            {
                _ePlayStatus = value;
                OnPropertyChanged(nameof(PlayingStatus));
            }
        }

        /// <summary>
        /// Determines the Recording status.
        /// </summary>
        public ERecordingStatus RecordingStatus
        {
            get => _eRecordingStatus;
            set
            {
                _eRecordingStatus = value;
                OnPropertyChanged(nameof(RecordingStatus));
            }
        }

        /// <summary>
        /// Determines Play command needs to reject or not?
        /// </summary>
        public bool DeclinePlayCommand
        {
            get => _declinePlayCommand;
            set
            {
                _declinePlayCommand = value;
                OnPropertyChanged(nameof(DeclinePlayCommand));
            }
        }

        /// <summary>
        /// Determines Record command needs to reject or not?
        /// </summary>
        public bool DeclineRecordCommand
        {
            get => _declineRecordCommand;
            set
            {
                _declineRecordCommand = value;
                OnPropertyChanged(nameof(DeclineRecordCommand));
            }
        }

        /// <summary>
        /// Determines record is exists or not?
        /// </summary>
        public bool RecordingNotExists
        {
            get => _recordingNotExists;
            set
            {
                _recordingNotExists = value;
                OnPropertyChanged(nameof(RecordingNotExists));
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

            PlayingStatus = EPlayStatus.Stopped;
            RecordingStatus = ERecordingStatus.Stopped;
            _recordingNotExists = false;
            InitCommands();
        }

        /// <summary>
        /// Initialize the PlayPauseCommand, StartPauseRecordingCommand and StopCommand commands
        /// </summary>
        private void InitCommands()
        {
            PlayPauseCommand = new RelayCommand((x) =>
            {
                DeclinePlayCommand = RecordingStatus != ERecordingStatus.Stopped;
                if (DeclinePlayCommand)
                {
                    return;
                }

                switch (PlayingStatus)
                {
                    case EPlayStatus.Playing:
                        _applicationModel.PausePlaying();
                        PlayingStatus = EPlayStatus.Paused;
                        return;
                    case EPlayStatus.Paused:
                        break;
                    case EPlayStatus.Stopped:
                        if (!File.Exists(_fileName))
                        {
                            RecordingNotExists = true;
                            return;
                        }
                        _applicationModel.Player.FileName = _fileName;
                        _applicationModel.Player.PlayingStoppedEvent += (owner, args) =>
                        {
                            PlayingStatus = EPlayStatus.Stopped;
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _applicationModel.PlayRecord();
                PlayingStatus = EPlayStatus.Playing;
            });

            StartPauseRecordingCommand = new RelayCommand((x) =>
            {
                DeclineRecordCommand = PlayingStatus != EPlayStatus.Stopped;
                if (DeclineRecordCommand)
                {
                    return;
                }

                switch (RecordingStatus)
                {
                    case ERecordingStatus.Recording:
                        _applicationModel.PauseRecording();
                        RecordingStatus = ERecordingStatus.Paused;
                        return;
                    case ERecordingStatus.Paused:
                        break;
                    case ERecordingStatus.Stopped:
                        _applicationModel.Recorder.OutFileName = _fileName;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _applicationModel.StartRecording();
                RecordingStatus = ERecordingStatus.Recording;
            });

            StopCommand = new RelayCommand((x) =>
            {
                if (_applicationModel.Player.IsActive())
                {
                    _applicationModel.StopPlaying();
                    PlayingStatus = EPlayStatus.Stopped;
                    return;
                }

                if (_applicationModel.Recorder.IsActive())
                {
                    _applicationModel.StopRecording();

                    RecordingStatus = ERecordingStatus.Stopped;
                }
            });
        }

    }
}
