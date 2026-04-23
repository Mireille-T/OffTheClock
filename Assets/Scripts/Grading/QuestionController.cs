using UnityEngine;

public class QuestionController : MonoBehaviour
{
    // The hidden truth: Did the simulated student get this right?
    public bool isStudentAnswerCorrect; 

    // The GradingZone calls this when the player stamps
    public void ReceivePlayerGrade(bool playerStampedCorrectly)
    {
        if (playerStampedCorrectly == isStudentAnswerCorrect)
        {
            Debug.Log("Good grading! You caught it.");
            // Add to player score
        }
        else
        {
            Debug.Log("Bad grading! You marked a wrong answer as Correct (or vice versa).");
            // Penalize player score
        }

        // Tell the main paper that another question is finished
        GetComponentInParent<PaperController>().RegisterGrade();
    }
}