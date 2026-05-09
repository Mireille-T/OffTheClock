using UnityEngine;

public class InitFrameRate : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 90;
        Time.maximumDeltaTime = 0.1f;       // cap physics catch-up after frame spikes
        Time.fixedDeltaTime = 0.02f;        // 50Hz physics
    }
}
