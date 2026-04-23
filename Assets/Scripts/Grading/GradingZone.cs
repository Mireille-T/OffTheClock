using UnityEngine;

public class GradingZone : MonoBehaviour
{
    public GameObject inkDecalPrefab; // Drag a Red/Green Checkmark prefab here
    public bool isCorrectBox;         // Check this in Inspector for the "Correct" box
    private bool hasBeenStamped = false;

    // This runs when the Stamp (tagged "Stamp") enters the BoxCollider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PencilTip") && !hasBeenStamped)
        {
            hasBeenStamped = true;

            // 1. Spawn the ink mark at the box's position
            // Move it slightly forward (z-axis) to prevent it clipping into the paper
            Instantiate(inkDecalPrefab, transform.position + (transform.forward * -0.01f), transform.rotation, transform);

            GetComponentInParent<QuestionController>().ReceivePlayerGrade(isCorrectBox);

            // 2. Play a sound if you have one attached to the box
            if(GetComponent<AudioSource>()) GetComponent<AudioSource>().Play();

            Debug.Log("Question graded as: " + (isCorrectBox ? "Correct" : "Incorrect"));
        }
    }
}