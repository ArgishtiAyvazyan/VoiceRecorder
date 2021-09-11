using System;

namespace VoiceRecorder.Model
{
    public class EventData : EventArgs
    { }

    /// <summary>
    /// IPlayer provides an abstract interface for Play the recording which will be independent of the audio engine.
    /// 
    /// Implemented for eliminated dependency between Model and Audio engine.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The file name, which will be played.
        /// </summary>
        string FileName { set; get; }

        /// <summary>
        /// Event Playing Stopped, the listeners will be invoked when playing finished.
        /// </summary>
        event EventHandler<EventData> PlayingStoppedEvent;

        /// <summary>
        /// Starts or resume playback.
        ///
        /// throw InvalidOperationException if Playing is already started.
        /// </summary>
        void Play();

        /// <summary>
        /// Pauses playback.
        /// </summary>
        void Pause();

        /// <summary>
        /// Finishes playback.
        /// </summary>
        void Stop();

        /// <summary>
        /// Checks the player has active playback or not?
        /// </summary>
        /// <returns>True if the player now in playing or paused states, otherwise False.</returns>
        bool IsActive();

    }
}
