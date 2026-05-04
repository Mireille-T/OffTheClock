using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ClassroomInitializer : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData;
    [SerializeField] private Transform mechaEnvironment = null;
    [SerializeField] private GameObject mechaAddons = null;
    [SerializeField] private GameObject glasses = null;
    [SerializeField] private TeleportManager classroomTeleportManager = null;

    private GameObject glassesInstance = null;

    private void Start()
    {
        if (glasses != null)
        {
            glassesInstance = Instantiate(glasses, Camera.main.transform);
        }

        if (classroomTeleportManager != null)
        {
            classroomTeleportManager.PositionToTeleport = mechaEnvironment.position;
            classroomTeleportManager.Initializer = this.gameObject;
        }
    }

    private void OnEnable()
    {
        if (rendererData != null && rendererData.rendererFeatures.Count > 0)
        {
            rendererData.rendererFeatures[0].SetActive(false);
        }

        if (glassesInstance != null)
        {
            glassesInstance.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (glassesInstance != null)
        {
            glassesInstance.SetActive(false);
        }

        if (mechaAddons != null)
        {
            mechaAddons.SetActive(true);
        }
    }
}
