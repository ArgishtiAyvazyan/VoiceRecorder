using System;
using NAudio.Wave;
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
        /// <summary>
        /// The enum EPlayStatus describes the player status.
        /// </summary>
        private enum EPlayStatus
        {
            Playing,
            Stopped,
            Paused,
        };

        /// <summary>
        /// The file name, which will be played.
        /// </summary>
        public string FileName { set; get; }

        /// <summary>
        /// Event Playing Stopped, the listeners will be invoked when playing finished.
        /// </summary>
        public event EventHandler<EventData> PlayingStoppedEvent;

        private Mp3FileReader Reader { get; set; }
        private WaveOut WaveOut { get; set; }
        private EPlayStatus PlayStatus { get; set; }

        /// <summary>
        /// Starts or resume playback.
        ///
        /// throw InvalidOperationException if Playing is already started.
        /// </summary>
        public void Play()
        {
            switch (PlayStatus)
            {
                case EPlayStatus.Playing:
                    throw new InvalidOperationException("ERROR: The Playing is already started.");
                case EPlayStatus.Stopped:
                    InitPlayer();
                    break;
                case EPlayStatus.Paused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PlayStatus = EPlayStatus.Playing;
            WaveOut.Play();
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void Pause()
        {
            PlayStatus = EPlayStatus.Paused;
            WaveOut.Pause();
        }

        /// <summary>
        /// Finishes playback.
        /// </summary>
        public void Stop()
        {
            PlayStatus = EPlayStatus.Stopped;
            WaveOut.Stop();
            WaveOut.Dispose();
            WaveOut = null;
            Reader.Close();
            Reader = null;
        }

        /// <summary>
        /// Checks the player has active playback or not?
        /// </summary>
        /// <returns>True if the player now in playing or paused states, otherwise False.</returns>
        public bool IsActive()
        {
            return PlayStatus != EPlayStatus.Stopped;
        }

        /// <summary>
        /// Initializes the player for starting playing.
        /// </summary>
        private void InitPlayer()
        {
            Reader = new Mp3FileReader(FileName);
            WaveOut = new WaveOut();
            WaveOut.Init(Reader);
            WaveOut.PlaybackStopped += PlaybackStoppedHandler;
        }

        /// <summary>
        /// Handles the finish playing event.
        /// </summary>
        /// <param name="sender">The Event owner.</param>
        /// <param name="e">Event args.</param>
        private void PlaybackStoppedHandler(object sender, StoppedEventArgs e)
        {
            Stop();
            PlayingStoppedEvent?.Invoke(this, new EventData());
        }
    }
}
