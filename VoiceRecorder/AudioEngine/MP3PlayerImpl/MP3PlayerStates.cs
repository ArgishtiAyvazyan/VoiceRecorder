using System;
using NAudio.Wave;
using VoiceRecorder.Model;

namespace VoiceRecorder.AudioEngine.MP3PlayerImpl
{
    /// <summary>
    /// The State interface declares the state-specific methods.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal abstract class AMP3PlayerState
    {
        protected InternalData Data;

        protected AMP3PlayerState(InternalData data)
        {
            Data = data;
        }

        public abstract void Play();

        public abstract void Pause();

        public abstract void Stop();

        public abstract bool IsActive();

    }

    /// <summary>
    /// Class PlayingState provide their own implementations for the state-specific methods. 
    /// </summary>
    internal class PlayingState : AMP3PlayerState
    {
        public PlayingState(InternalData data) : base(data)
        {
        }

        public override void Play()
        {
            throw new InvalidOperationException("ERROR: The Playing is already started.");
        }

        public override void Pause()
        {
            Data.WaveOut.Pause();
        }

        public override void Stop()
        {
            Data.WaveOut.Stop();
            Data.WaveOut.Dispose();
            Data.WaveOut = null;
            Data.Reader.Close();
            Data.Reader = null;
        }

        public override bool IsActive()
        {
            return true;
        }
    }

    /// <summary>
    /// Class PausedState provide their own implementations for the state-specific methods. 
    /// </summary>

    internal class PausedState : AMP3PlayerState
    {
        public PausedState(InternalData data) : base(data)
        {
        }

        public override void Play()
        {
            Data.WaveOut.Play();
        }

        public override void Pause()
        {
        }

        public override void Stop()
        {
            Data.WaveOut.Stop();
            Data.WaveOut.Dispose();
            Data.WaveOut = null;
            Data.Reader.Close();
            Data.Reader = null;
        }

        public override bool IsActive()
        {
            return true;
        }
    }

    /// <summary>
    /// Class StoppedState provide their own implementations for the state-specific methods. 
    /// </summary>
    internal class StoppedState : AMP3PlayerState
    {
        public StoppedState(InternalData data) : base(data)
        {
        }

        public override void Play()
        {
            InitPlayer();
            Data.WaveOut.Play();
        }

        public override void Pause()
        {
        }

        public override void Stop()
        {
        }

        public override bool IsActive()
        {
            return false;
        }

        /// <summary>
        /// Initializes the player for starting playing.
        /// </summary>
        private void InitPlayer()
        {
            if (Data.Reader != null) { throw new InvalidOperationException("ERROR: Data.Reader should be null."); }
            Data.Reader = new Mp3FileReader(Data.FileName);

            if (Data.WaveOut != null) { throw new InvalidOperationException("ERROR: Data.WaveOut should be null."); }
            Data.WaveOut = new WaveOut();

            Data.WaveOut.Init(Data.Reader);
            Data.WaveOut.PlaybackStopped += PlaybackStoppedHandler;
        }

        /// <summary>
        /// Handles the finish playing event.
        /// </summary>
        /// <param name="sender">The Event owner.</param>
        /// <param name="e">Event args.</param>
        private void PlaybackStoppedHandler(object sender, StoppedEventArgs e)
        {
            Stop();
            Data.InvokePlayingStoppedEventListeners(this, new EventData());
        }
    }

}
