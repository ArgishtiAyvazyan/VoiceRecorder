using System;
using System.Collections.Generic;
using VoiceRecorder.Model;
using NAudio.CoreAudioApi;
using NAudio.Lame;
using NAudio.Wave;

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
        /// <summary>
        /// The class Microphone is an adapter on MMDevice.
        /// </summary>
        private class Microphone : IDevice
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

        /// <summary>
        /// The enum ERecordingStatus describes the recorder status.
        /// </summary>
        private enum ERecordingStatus
        {
            Recording = 0,
            Stopped,
            Paused,

        };

        /// <summary>
        /// The output file name.
        /// </summary>
        public string OutFileName { get; set; }

        /// <summary>
        /// The list of available devices (Microphones).
        /// </summary>
        public List<IDevice> Devices { get; private set; }

        /// <summary>
        /// The active device will be used for audio recording.
        /// </summary>
        public IDevice ActiveDevice { get; set; }

        private LameMP3FileWriter Writer { get; set; }
        private IWaveIn WaveIn { get; set; }
        private ERecordingStatus RecordingStatus { get; set; }

        public MP3Recorder()
        {
            RecordingStatus = ERecordingStatus.Stopped;

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
            switch (RecordingStatus)
            {
                case ERecordingStatus.Recording:
                    throw new InvalidOperationException("ERROR: The recording is already started.");
                case ERecordingStatus.Stopped:
                    InitRecorder();
                    break;
                case ERecordingStatus.Paused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RecordingStatus = ERecordingStatus.Recording;
            WaveIn.StartRecording();
        }

        /// <summary>
        /// Pauses Recording.
        ///
        /// Throw Exception if there is a problem during recording.
        /// </summary>
        public void PauseRecording()
        {
            RecordingStatus = ERecordingStatus.Paused;
            WaveIn.StopRecording();
        }

        /// <summary>
        /// Finishes recording.
        ///
        /// Throw InvalidOperationException if if Recording is already stopped.
        /// Throw Exception if there is a problem during recording.
        /// </summary>
        public void StopRecording()
        {
            if (RecordingStatus == ERecordingStatus.Stopped)
            {
                throw new InvalidOperationException("ERROR: The recording is already Stopped.");
            }

            RecordingStatus = ERecordingStatus.Stopped;
            WaveIn.StopRecording();
            Dispose();
        }

        /// <summary>
        /// Checks the recorder has active recording or not?
        /// </summary>
        /// <returns>True if the recorder is now in recording or paused states, otherwise False.</returns>
        public bool IsActive()
        {
            return RecordingStatus != ERecordingStatus.Stopped;
        }

        /// <summary>
        /// Initializes the recorder for starting record.
        /// </summary>
        private void InitRecorder()
        {
            WaveIn = new WasapiCapture(((Microphone)ActiveDevice).Device);

            Writer = new LameMP3FileWriter(OutFileName, WaveIn.WaveFormat, 128);

            WaveIn.DataAvailable += AvailableDataHandler;
            WaveIn.RecordingStopped += StopRecordingHandler;
        }

        /// <summary>
        /// Closes opened files and destroys the internal objects.
        /// </summary>
        private void Dispose()
        {
            RecordingStatus = ERecordingStatus.Stopped;
            WaveIn.Dispose();
            WaveIn = null;
            Writer.Close();
            Writer = null;
        }

        /// <summary>
        /// Catches the recorded data and writes in the file.
        /// </summary>
        /// <param name="sender">The Event owner.</param>
        /// <param name="e">Event args.</param>
        private void AvailableDataHandler(object sender, WaveInEventArgs e)
        {
            Writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        /// <summary>
        /// Handles the finish recording event.
        ///
        /// Throw Exception if there is a problem during recording.
        /// </summary>
        /// <param name="sender">The Event owner.</param>
        /// <param name="e">Event args.</param>
        private void StopRecordingHandler(object sender, StoppedEventArgs e)
        {
            if (RecordingStatus == ERecordingStatus.Recording)
            {
                RecordingStatus = ERecordingStatus.Stopped;
            }

            if (e.Exception != null)
            {
                throw new Exception($"ERROR: A problem was encountered during recording {e.Exception.Message}", e.Exception);
            }
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
