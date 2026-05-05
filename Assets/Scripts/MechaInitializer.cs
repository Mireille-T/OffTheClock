using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MechaInitializer : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData;
    [SerializeField] private Transform classroomEnvironment = null;
    [SerializeField] private Vector3 teleportOffset = Vector3.zero;
    [SerializeField] private GameObject classroomAddons = null;
    [SerializeField] private GameObject mechaCockpit = null;
    [SerializeField] private AudioClip mechaAudio = null;

    private GameObject uiCanvas = null;
    private TeleportManager teleportManager = null;
    private GameObject cockpitInstance = null;

    private void Start()
    {
        if (mechaCockpit != null)
        {
            cockpitInstance = Instantiate(mechaCockpit, Camera.main.transform);
            teleportManager = cockpitInstance.GetComponentInChildren<TeleportManager>();
            if (teleportManager != null)
            {
                teleportManager.PositionToTeleport = classroomEnvironment.position + teleportOffset;
                teleportManager.Initializer = this.gameObject;
            }
        }

        Canvas canvas = this.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            uiCanvas = canvas.transform.parent.gameObject;
            uiCanvas.transform.SetParent(Camera.main.transform.parent);
        }
    }

    private void OnEnable()
    {
        if (rendererData != null && rendererData.rendererFeatures.Count > 0)
        {
            rendererData.rendererFeatures[0].SetActive(true);
        }

        if (cockpitInstance != null)
        {
            cockpitInstance.SetActive(true);
        }

        if (Camera.main.TryGetComponent(out AudioSource audioSource) && mechaAudio != null)
        {
            audioSource.PlayOneShot(mechaAudio);
        }

        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(true);
        }

        Animation revealAnimation = this.GetComponentInChildren<Animation>();
        if (revealAnimation != null)
        {
            revealAnimation.Play();
        }
    }

    private void OnDisable()
    {
        if (cockpitInstance != null)
        {
            cockpitInstance.SetActive(false);
        }

        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(false);
        }

        if (classroomAddons != null)
        {
            classroomAddons.SetActive(true);
        }
    }
}
