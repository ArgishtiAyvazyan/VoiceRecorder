using System;
using NAudio.CoreAudioApi;
using VoiceRecorder.Model;

namespace VoiceRecorder.AudioEngine.MP3RecorderImpl
{
    /// <summary>
    /// The class Microphone is an adapter on MMDevice.
    /// </summary>
    internal class Microphone : IDevice
    {
        public Microphone(MMDevice device)
        {
            Device = device;
        }

        /// <summary>
        /// The audio device name.
        /// </summary>
        public string Name
        {
            get => Device.FriendlyName;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The nested device.
        /// </summary>
        public MMDevice Device { get; private set; }
    }
}
