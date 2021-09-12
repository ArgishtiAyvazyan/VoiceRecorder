using System;
using VoiceRecorder.AudioEngine.MP3PlayerImpl;
using VoiceRecorder.Model;

namespace VoiceRecorder.AudioEngine
{
    /// <summary>
    /// The class MP3Player provides the audio playback process.
    /// MP3Player can audio playback files in mp3 format.
    /// The class MP3Player implemented based on NAudio library.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class MP3Player : IPlayer
    {
        private readonly PlayingState _playingState;
        private readonly PausedState _pausedState;
        private readonly StoppedState _stoppedState;

        private AMP3PlayerState _activeState;

        private readonly InternalData _data;


        /// <summary>
        /// The file name, which will be played.
        /// </summary>
        public string FileName
        {
            get => _data.FileName;
            set => _data.FileName = value;
        }

        /// <summary>
        /// Event Playing Stopped, the listeners will be invoked when playing finished.
        /// </summary>
        public event EventHandler<EventData> PlayingStoppedEvent;


        public MP3Player()
        {
            _data = new InternalData();

            _playingState = new PlayingState(_data);
            _pausedState = new PausedState(_data);
            _stoppedState = new StoppedState(_data);

            _activeState = _stoppedState;

            _data.PlayingStoppedEvent += (owner, args)
                =>
            {
                PlayingStoppedEvent?.Invoke(owner, args);
            };
        }

        /// <summary>
        /// Starts or resume playback.
        ///
        /// throw InvalidOperationException if Playing is already started.
        /// </summary>
        public void Play()
        {
            _activeState.Play();
            _activeState = _playingState;
            _data.PlayingStoppedEvent += ((sender, data)
                =>
            {
                _activeState.Stop();
                _activeState = _stoppedState;
            });
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void Pause()
        {
            _activeState.Pause();
            _activeState = _pausedState;
        }

        /// <summary>
        /// Finishes playback.
        /// </summary>
        public void Stop()
        {
            _activeState.Stop();
            _activeState = _stoppedState;
        }

        /// <summary>
        /// Checks the player has active playback or not?
        /// </summary>
        /// <returns>True if the player now in playing or paused states, otherwise False.</returns>
        public bool IsActive()
        {
            return _activeState.IsActive();
        }
    }
}
