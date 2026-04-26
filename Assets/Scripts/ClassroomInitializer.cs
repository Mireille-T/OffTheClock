using UnityEngine;

public class ClassroomInitializer : MonoBehaviour
{
    [SerializeField] private GameObject glasses = null;

    private void Start()
    {
        if (glasses != null)
        {
            Instantiate(glasses, Camera.main.transform);
        }
    }
}
