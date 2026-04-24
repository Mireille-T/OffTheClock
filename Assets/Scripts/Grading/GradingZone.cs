using UnityEngine;

public class GradingZone : MonoBehaviour
{
    public GameObject inkDecalPrefab;
    public bool isCorrectBox;         // Check this in Inspector for the "Correct" box
    private bool hasBeenStamped = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PencilTip") && !hasBeenStamped)
        {
            hasBeenStamped = true;
            Instantiate(inkDecalPrefab, transform.position + (transform.forward * -0.01f), transform.rotation, transform);

            GetComponentInParent<QuestionController>().ReceivePlayerGrade(isCorrectBox);

            if(GetComponent<AudioSource>()) GetComponent<AudioSource>().Play();

            Debug.Log("Question graded as: " + (isCorrectBox ? "Correct" : "Incorrect"));
        }
    }
}