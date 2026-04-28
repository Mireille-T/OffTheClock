using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    const string CONTROLLER_TAG = "VRController";

    [SerializeField] private GameObject otherCameraView = null;
    [SerializeField] private float secondsNeededToTeleport = 3.0f;
    [SerializeField] private float yPosToTeleport = 0.134f;
    [SerializeField] private float particleSpeedUpMultiplier = 5.0f;
    [SerializeField] private float particleSizeUpMultiplier = 5.0f;

    private AudioSource audioSource = null;
    private ParticleSystem particles = null;
    private ParticleSystem.MainModule particlesMain;
    private float secondsSinceTriggered = -1.0f;
    private float originalCameraViewYPos = 0.0f;
    private float initialParticleSpeed = 0.0f;
    private float initialParticleSize = 0.0f;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        particles = this.GetComponentInChildren<ParticleSystem>();
        if (particles != null)
        {
            particles.Stop();
            particlesMain = particles.main;
            initialParticleSpeed = particlesMain.startSpeedMultiplier;
            initialParticleSize = particlesMain.startSizeMultiplier;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag(CONTROLLER_TAG))
        {
            secondsSinceTriggered = 0.0f;

            if (otherCameraView != null)
            {
                originalCameraViewYPos = otherCameraView.transform.localPosition.y;
            }
            if (audioSource != null)
            {
                audioSource.Play();
            }
            if (particles != null)
            {
                particles.Play();
            }
        }   
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(CONTROLLER_TAG))
        {
            if (secondsSinceTriggered >= secondsNeededToTeleport)
            {
                // TODO: Teleport
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
                if (particles != null)
                {
                    particles.Stop();
                }
            }

            secondsSinceTriggered += Time.deltaTime;
            if (otherCameraView != null)
            {
                Vector3 pos = otherCameraView.transform.localPosition;
                otherCameraView.transform.localPosition = new Vector3(pos.x, Mathf.Lerp(originalCameraViewYPos, yPosToTeleport, secondsSinceTriggered / secondsNeededToTeleport), pos.z);
            }
            if (particles != null)
            {
                particlesMain.startSpeedMultiplier += Time.deltaTime * particleSpeedUpMultiplier;
                particlesMain.startSizeMultiplier += Time.deltaTime * particleSizeUpMultiplier;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(CONTROLLER_TAG))
        {
            secondsSinceTriggered = -1.0f;

            if (otherCameraView != null)
            {
                Vector3 pos = otherCameraView.transform.localPosition;
                otherCameraView.transform.localPosition = new Vector3(pos.x, originalCameraViewYPos, pos.z);
            }
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            if (particles != null)
            {
                particles.Stop();
                particlesMain.startSpeedMultiplier = initialParticleSpeed;
                particlesMain.startSizeMultiplier = initialParticleSize;
            }
        }
    }
}
