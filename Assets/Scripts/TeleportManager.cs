using Unity.XR.CoreUtils;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    const string CONTROLLER_TAG = "VRController";

    [SerializeField] private GameObject otherCameraView = null;
    [SerializeField] private float secondsNeededToTeleport = 3.0f;
    [SerializeField] private float yPosToTeleport = 0.134f;
    [SerializeField] private float particleRateMultiplier = 5.0f;
    [SerializeField] private float particleSizeUpMultiplier = 5.0f;

    private GameObject initializer = null;
    public GameObject Initializer
    {
        set
        {
            initializer = value;
        }
    }

    private Vector3 positionToTeleport;
    public Vector3 PositionToTeleport
    {
        set
        {
            positionToTeleport = value;
        }
    }

    private AudioSource audioSource = null;
    private ParticleSystem particles = null;
    private ParticleSystem.MainModule particlesMain;
    private ParticleSystem.EmissionModule particlesEmission;
    private float secondsSinceTriggered = -1.0f;
    private float originalCameraViewYPos = 0.0f;
    private float initialParticleRate = 0.0f;
    private float initialParticleSize = 0.0f;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        particles = this.GetComponentInChildren<ParticleSystem>();
        if (particles != null)
        {
            particles.Stop();
            particlesMain = particles.main;
            particlesEmission = particles.emission;
            initialParticleRate = particlesEmission.rateOverTimeMultiplier;
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
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
                if (particles != null)
                {
                    particles.Stop();
                }

                FindAnyObjectByType<XROrigin>().transform.position = positionToTeleport;
                if (initializer != null)
                {
                    initializer.SetActive(false);
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
                particlesEmission.rateOverTimeMultiplier += Time.deltaTime * particleRateMultiplier;
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
                particlesEmission.rateOverTimeMultiplier = initialParticleRate;
                particlesMain.startSizeMultiplier = initialParticleSize;
            }
        }
    }
}
