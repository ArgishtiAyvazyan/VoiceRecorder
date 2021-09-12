using System.Collections.Generic;
using NAudio.Lame;
using NAudio.Wave;
using VoiceRecorder.Model;

namespace VoiceRecorder.AudioEngine.MP3RecorderImpl
{
    /// <summary>
    /// Internal data storage for MP3Recorder.
    /// </summary>
    internal class InternalData
    {
        /// <summary>
        /// The output file name.
        /// </summary>
        public string OutFileName { get; set; } = null;

        /// <summary>
        /// The list of available devices (Microphones).
        /// </summary>
        public List<IDevice> Devices { get; set; } = null;

        /// <summary>
        /// The active device will be used for audio recording.
        /// </summary>
        public IDevice ActiveDevice { get; set; } = null;

        /// <summary>
        /// MP3 encoding object.
        /// </summary>
        public LameMP3FileWriter Writer { get; set; } = null;

        /// <summary>
        /// Allows recording using the Windows waveIn APIs Events are raised as recorded buffers are made available.
        /// </summary>
        public IWaveIn WaveIn { get; set; } = null;
    }
}
