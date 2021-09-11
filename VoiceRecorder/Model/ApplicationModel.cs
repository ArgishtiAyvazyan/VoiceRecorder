namespace VoiceRecorder.Model
{
    /// <summary>
    /// ApplicationModel the class encapsulating application logic.
    /// </summary>
    public class ApplicationModel
    {
        /// <summary>
        /// The abstract Recorder. 
        /// </summary>
        public IRecorder Recorder { get; set; }

        /// <summary>
        /// The abstract Player. 
        /// </summary>
        public IPlayer Player { get; set; }

        /// <summary>
        /// Creates a new instance of ApplicationModel.
        /// </summary>
        /// <param name="recorder">The recorder.</param>
        /// <param name="player">The player.</param>
        public ApplicationModel(IRecorder recorder, IPlayer player)
        {
            Recorder = recorder;
            Player = player;
        }

        /// <summary>
        /// Starts or resume playback.
        /// </summary>
        public void PlayRecord()
        {
            Player.Play();
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void PausePlaying()
        {
            Player.Pause();
        }

        /// <summary>
        /// Finishes playback.
        /// </summary>
        public void StopPlaying()
        {
            Player.Stop();
        }

        /// <summary>
        /// Starts or resume the recording.
        /// </summary>
        public void StartRecording()
        {
            Recorder.StartRecording();
        }

        /// <summary>
        /// Pauses Recording.
        /// </summary>
        public void PauseRecording()
        {
            Recorder.PauseRecording();
        }

        /// <summary>
        /// Finishes recording.
        /// </summary>
        public void StopRecording()
        {
            Recorder.StopRecording();
        }
    }
}
