using System;
using System.Collections.Generic;
using VoiceRecorder.Model;
using NAudio.CoreAudioApi;
using NAudio.Lame;
using NAudio.Wave;
using VoiceRecorder.AudioEngine.MP3RecorderImpl;

namespace VoiceRecorder.AudioEngine
{
    /// <summary>
    /// The class MP3Recorder provides the audio recording process.
    /// The result will be saved in mp3 format.
    /// The class MP3Recorder implemented based on NAudio library.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class MP3Recorder : IRecorder
    {
        private readonly RecordingState _recordingState;
        private readonly PausedState _pausedState;
        private readonly StoppedState _stoppedState;

        private AMP3RecorderState _activeState;

        private readonly InternalData _data;

        /// <summary>
        /// The output file name.
        /// </summary>
        public string OutFileName
        {
            get => _data.OutFileName;
            set => _data.OutFileName = value;
        }

        /// <summary>
        /// The list of available devices (Microphones).
        /// </summary>
        public List<IDevice> Devices
        {
            get => _data.Devices;
            set => _data.Devices = value;
        }

        /// <summary>
        /// The active device will be used for audio recording.
        /// </summary>
        public IDevice ActiveDevice
        {
            get => _data.ActiveDevice;
            set => _data.ActiveDevice = value;
        }


        public MP3Recorder()
        {
            _data = new InternalData();

            _recordingState = new RecordingState(_data);
            _pausedState    = new PausedState(_data);
            _stoppedState   = new StoppedState(_data);

            _activeState = _stoppedState;

            CollectMicrophones();
            SelectDefaultDevice();
        }

        /// <summary>
        /// Starts or resume the recording.
        ///
        /// Throws InvalidOperationException if Recording is already started.
        /// </summary>
        public void StartRecording()
        {
            _activeState.StartRecording();
            _activeState = _recordingState;
        }

        /// <summary>
        /// Pauses Recording.
        ///
        /// Throw Exception if there is a problem during recording.
        /// </summary>
        public void PauseRecording()
        {
            _activeState.PauseRecording();
            _activeState = _pausedState;
        }

        /// <summary>
        /// Finishes recording.
        ///
        /// Throw InvalidOperationException if if Recording is already stopped.
        /// Throw Exception if there is a problem during recording.
        /// </summary>
        public void StopRecording()
        {
            _activeState.StopRecording();
            _activeState = _stoppedState;
        }

        /// <summary>
        /// Checks the recorder has active recording or not?
        /// </summary>
        /// <returns>True if the recorder is now in recording or paused states, otherwise False.</returns>
        public bool IsActive()
        {
            return _activeState.IsActive();
        }

        /// <summary>
        /// Collects the available devices (Microphones).
        /// </summary>
        private void CollectMicrophones()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            Devices = new List<IDevice>();

            foreach (var device in devices)
            {
                Devices.Add(new Microphone(device));
            }
        }

        /// <summary>
        /// Gets and sets windows default device as an active device.
        /// </summary>
        private void SelectDefaultDevice()
        {
            var enumerator = new MMDeviceEnumerator();
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
            ActiveDevice = new Microphone(defaultDevice);
        }
    }
}
