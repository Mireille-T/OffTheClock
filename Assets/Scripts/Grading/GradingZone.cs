using UnityEngine;

public class GradingZone : MonoBehaviour
{
    public GameObject inkDecalPrefab;
    public bool isCorrectBox;         // Check this in Inspector for the "Correct" box

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PencilTip"))
        {

            if (GetComponentInParent<QuestionController>().ReceivePlayerGrade(isCorrectBox)){
                GameObject decal = Instantiate(inkDecalPrefab, transform.parent);

                RectTransform rt = decal.GetComponent<RectTransform>();

                rt.position = transform.position;

                Vector3 localPos = rt.localPosition;
                localPos.z = -0.5f; 
                rt.localPosition = localPos;

                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;


                if(GetComponent<AudioSource>()) GetComponent<AudioSource>().Play();

                Debug.Log("Question graded as: " + (isCorrectBox ? "Correct" : "Incorrect"));

            }
        }
    }
}