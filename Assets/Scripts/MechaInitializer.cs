using UnityEngine;

public class MechaInitializer : MonoBehaviour
{
    [SerializeField] private GameObject mechaCockpit = null;

    private void Start()
    {
        if (mechaCockpit != null)
        {
            Instantiate(mechaCockpit, Camera.main.transform);
        }
    }
}
