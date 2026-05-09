using System.Text;
using UnityEngine;
using TMPro;
using Unity.Profiling;

namespace OffTheClock.DebugTools
{
    // Attach to a GameObject. Assign a world-space TMP_Text that's parented to the
    // player camera so it's always visible in VR. Shows real-time perf stats.
    public class PerfOverlay : MonoBehaviour
    {
        [Tooltip("World-space TMP text parented to camera. If null, logs to console only.")]
        public TMP_Text outputText;

        [Tooltip("How often (seconds) to update the displayed stats.")]
        public float refreshInterval = 0.5f;

        [Tooltip("Also log to Unity console.")]
        public bool logToConsole = false;

        private float _accumTime;
        private int _frameCount;
        private float _maxFrameMs;
        private float _minFrameMs = float.MaxValue;
        private readonly StringBuilder _sb = new();

        private ProfilerRecorder _gcAllocRecorder;
        private ProfilerRecorder _drawCallsRecorder;
        private ProfilerRecorder _setPassRecorder;
        private ProfilerRecorder _trianglesRecorder;
        private ProfilerRecorder _physicsTimeRecorder;
        private ProfilerRecorder _scriptUpdateRecorder;

        void OnEnable()
        {
            _gcAllocRecorder      = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC.Alloc.Count");
            _drawCallsRecorder    = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            _setPassRecorder      = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            _trianglesRecorder    = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
            _physicsTimeRecorder  = ProfilerRecorder.StartNew(ProfilerCategory.Scripts, "PhysicsManager.FixedUpdate");
            _scriptUpdateRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Scripts, "BehaviourUpdate");
        }

        void OnDisable()
        {
            _gcAllocRecorder.Dispose();
            _drawCallsRecorder.Dispose();
            _setPassRecorder.Dispose();
            _trianglesRecorder.Dispose();
            _physicsTimeRecorder.Dispose();
            _scriptUpdateRecorder.Dispose();
        }

        void Update()
        {
            float ms = Time.unscaledDeltaTime * 1000f;
            _accumTime += Time.unscaledDeltaTime;
            _frameCount++;
            if (ms > _maxFrameMs) _maxFrameMs = ms;
            if (ms < _minFrameMs) _minFrameMs = ms;

            if (_accumTime >= refreshInterval)
            {
                float avgMs = (_accumTime / _frameCount) * 1000f;
                float fps = 1000f / avgMs;

                int activeProjectiles = GameObject.FindObjectsByType<OffTheClock.Combat.EnemyProjectile>(FindObjectsSortMode.None).Length;
                int activeEnemies     = GameObject.FindObjectsByType<OffTheClock.Combat.EnemyAI>(FindObjectsSortMode.None).Length;

                _sb.Clear();
                _sb.AppendFormat("FPS  {0:F0}  ({1:F1}ms avg)\n", fps, avgMs);
                _sb.AppendFormat("Spike  min {0:F1} / max {1:F1}ms\n", _minFrameMs, _maxFrameMs);
                _sb.AppendFormat("Draws  {0}  SetPass  {1}\n",
                    _drawCallsRecorder.LastValue, _setPassRecorder.LastValue);
                _sb.AppendFormat("Tris  {0:N0}\n", _trianglesRecorder.LastValue);
                _sb.AppendFormat("GCAllocs/frame  {0}\n", _gcAllocRecorder.LastValue);
                _sb.AppendFormat("Enemies  {0}  Projectiles  {1}\n", activeEnemies, activeProjectiles);

                string text = _sb.ToString();
                if (outputText != null) outputText.text = text;
                if (logToConsole) Debug.Log(text);

                _accumTime = 0f;
                _frameCount = 0;
                _maxFrameMs = 0f;
                _minFrameMs = float.MaxValue;
            }
        }
    }
}
