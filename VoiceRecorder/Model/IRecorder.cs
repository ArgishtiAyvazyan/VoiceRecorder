using System.Collections.Generic;

namespace VoiceRecorder.Model
{
    /// <summary>
    /// The abstraction level for working with audio devices.
    ///
    /// Implemented for eliminated dependency between Model and Audio engine.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// The audio device name.
        /// </summary>
        string Name { get; set; }
    }

    /// <summary>
    /// IRecorder provides an abstract interface for the audio recording which will
    /// be independent of the audio engine.
    /// 
    /// Implemented for eliminated dependency between Model and Audio engine.
    /// </summary>
    public interface IRecorder
    {
        /// <summary>
        /// The output file name.
        /// </summary>
        string OutFileName { set; get; }

        /// <summary>
        /// The list of available devices (Microphones).
        /// </summary>
        List<IDevice> Devices { get; }

        /// <summary>
        /// The active device will be used for audio recording.
        /// </summary>
        IDevice ActiveDevice { get; set; }

        /// <summary>
        /// Starts or resume the recording.
        ///
        /// Throws InvalidOperationException if Recording is already started.
        /// </summary>
        void StartRecording();

        /// <summary>
        /// Pauses Recording.
        ///
        /// Throw Exception if there is a problem during recording.
        /// </summary>
        void PauseRecording();

        /// <summary>
        /// Finishes recording.
        ///
        /// Throw InvalidOperationException if if Recording is already stopped.
        /// Throw Exception if there is a problem during recording.
        /// </summary>
        void StopRecording();


        /// <summary>
        /// Checks the recorder has active recording or not?
        /// </summary>
        /// <returns>True if the recorder is now in recording or paused states, otherwise False.</returns>
        bool IsActive();
    }
}
