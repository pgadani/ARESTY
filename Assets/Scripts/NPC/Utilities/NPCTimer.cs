using System.Diagnostics;

namespace NPC {
    /// <summary>
    /// Decided to implement the timer on the same thread to sync up with
    /// actual animations and IK frames. Otherwise it will not truly represent
    /// the delta time between frames.
    /// </summary>
    public class NPCTimer {

        // default one second
        public float Duration;

        private Stopwatch g_Stopwatch;
	
        public NPCTimer() {
            Finished = true;
            g_Stopwatch = new Stopwatch();
            g_Stopwatch.Reset();
            g_Stopwatch.Start();
        }

        public bool Finished;

        public void UpdateTimer() {
            if (!Finished) {
                if (g_Stopwatch.ElapsedMilliseconds >= Duration) {
                    g_Stopwatch.Stop();
                    g_Stopwatch.Reset();
                    Finished = true;
                }
            }
        }

        // Default to one second if no parameter specified.
        // Calling start while running restarts the timer
        public void StartTimer(float dur = 1000) {
            Duration = dur;
            Finished = false;
            g_Stopwatch.Reset();
            g_Stopwatch.Start();
        }
    }
}