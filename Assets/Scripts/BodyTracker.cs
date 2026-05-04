using UnityEngine;

public class BodyTracker : MonoBehaviour
{
    [SerializeField] private Vector3 offset = Vector3.zero;

    private void Update()
    {
        this.transform.position = Camera.main.transform.position + offset;
    }
}
