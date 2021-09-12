using System;
using NAudio.Wave;
using VoiceRecorder.Model;

namespace VoiceRecorder.AudioEngine.MP3PlayerImpl
{
    /// <summary>
    /// Internal data storage for MP3Recorder.
    /// </summary>
    internal class InternalData
    {
        /// <summary>
        /// The file name, which will be played.
        /// </summary>
        public string FileName { set; get; } = null;

        /// <summary>
        /// Event Playing Stopped, the listeners will be invoked when playing finished.
        /// </summary>
        public event EventHandler<EventData> PlayingStoppedEvent;

        /// <summary>
        /// Object for reading from MP3 files
        /// </summary>
        public Mp3FileReader Reader { get; set; } = null;

        /// <summary>
        /// The object essentially wraps the Windows waveOut APIs.
        /// </summary>
        public WaveOut WaveOut { get; set; } = null;

        public void InvokePlayingStoppedEventListeners(object owner, EventData args)
        {
            PlayingStoppedEvent?.Invoke(owner, args);
        }
    }
}
