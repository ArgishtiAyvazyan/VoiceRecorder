using System;
using NAudio.CoreAudioApi;
using NAudio.Lame;
using NAudio.Wave;

namespace VoiceRecorder.AudioEngine.MP3RecorderImpl
{
    /// <summary>
    /// The State interface declares the state-specific methods.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal abstract class AMP3RecorderState
    {
        protected InternalData Data; 

        protected AMP3RecorderState(InternalData data)
        {
            Data = data;
        }

        public abstract void StartRecording();

        public abstract void PauseRecording();

        public abstract void StopRecording();

        public abstract bool IsActive();

        /// <summary>
        /// Closes opened files and destroys the internal objects.
        /// </summary>
        protected void Dispose()
        {
            Data.WaveIn.Dispose();
            Data.WaveIn = null;
            Data.Writer.Close();
            Data.Writer = null;
        }
    }

    /// <summary>
    /// Class RecordingState provide their own implementations for the state-specific methods. 
    /// </summary>
    internal class RecordingState : AMP3RecorderState
    {
        public RecordingState(InternalData data)
            : base(data)
        { }

        public override void StartRecording()
        {
            throw new InvalidOperationException("ERROR: The recording is already started.");
        }

        public override void PauseRecording()
        {
            Data.WaveIn.StopRecording();
        }

        public override void StopRecording()
        {
            Data.WaveIn.StopRecording();
            Dispose();
        }

        public override bool IsActive()
        {
            return true;
        }
    }

    /// <summary>
    /// Class PausedState provide their own implementations for the state-specific methods. 
    /// </summary>
    internal class PausedState : AMP3RecorderState
    {
        public PausedState(InternalData data)
            : base(data)
        { }

        public override void StartRecording()
        {
            Data.WaveIn.StartRecording();
        }

        public override void PauseRecording()
        {
        }

        public override void StopRecording()
        {
            Data.WaveIn.StopRecording();
            Dispose();
        }

        public override bool IsActive()
        {
            return true;
        }
    }

    /// <summary>
    /// Class StoppedState provide their own implementations for the state-specific methods. 
    /// </summary>
    internal class StoppedState : AMP3RecorderState
    {
        public StoppedState(InternalData data)
            : base(data)
        { }

        public override void StartRecording()
        {
            InitRecorder();
            Data.WaveIn.StartRecording();
        }

        public override void PauseRecording()
        {
        }

        public override void StopRecording()
        {
            throw new InvalidOperationException("ERROR: The recording is already Stopped.");
        }

        public override bool IsActive()
        {
            return false;
        }

        /// <summary>
        /// Initializes the recorder for starting record.
        /// </summary>
        private void InitRecorder()
        {
            if (Data.WaveIn != null) { throw new InvalidOperationException("ERROR: Data.WaveIn should be null."); }
            Data.WaveIn = new WasapiCapture(((Microphone)Data.ActiveDevice).Device);

            if (Data.Writer != null) { throw new InvalidOperationException("ERROR: Data.Writer should be null."); }
            Data.Writer = new LameMP3FileWriter(Data.OutFileName, Data.WaveIn.WaveFormat, 128);

            Data.WaveIn.DataAvailable += AvailableDataHandler;
            Data.WaveIn.RecordingStopped += StopRecordingHandler;
        }

        /// <summary>
        /// Catches the recorded data and writes in the file.
        /// </summary>
        /// <param name="sender">The Event owner.</param>
        /// <param name="e">Event args.</param>
        private void AvailableDataHandler(object sender, WaveInEventArgs e)
        {
            Data.Writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        /// <summary>
        /// Handles the finish recording event.
        ///
        /// Throw Exception if there is a problem during recording.
        /// </summary>
        /// <param name="sender">The Event owner.</param>
        /// <param name="e">Event args.</param>
        private static void StopRecordingHandler(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                throw new Exception($"ERROR: A problem was encountered during recording {e.Exception.Message}", e.Exception);
            }
        }
    }
}
