using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MechaInitializer : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData;
    [SerializeField] private Transform classroomEnvironment = null;
    [SerializeField] private GameObject classroomAddons = null;
    [SerializeField] private GameObject mechaCockpit = null;

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
                teleportManager.PositionToTeleport = classroomEnvironment.position;
                teleportManager.Initializer = this.gameObject;
            }
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
    }

    private void OnDisable()
    {
        if (cockpitInstance != null)
        {
            cockpitInstance.SetActive(false);
        }

        if (classroomAddons != null)
        {
            classroomAddons.SetActive(true);
        }
    }
}
