using UnityEngine;

public class QuestionController : MonoBehaviour
{
    // The hidden truth: Did the simulated student get this right?
    public bool isStudentAnswerCorrect; 

    public AudioSource deskSpeaker;

    private bool _isAlreadyGraded = false;

    // The GradingZone calls this when the player stamps
    public bool ReceivePlayerGrade(bool playerStampedCorrectly)
    {

        if (_isAlreadyGraded) return false;
        _isAlreadyGraded = true;

        if (deskSpeaker != null)
        {
            deskSpeaker.Play();
        }

        bool isAccuracyCorrect = (playerStampedCorrectly == isStudentAnswerCorrect);
        
        if (GradingManager.Instance != null)
        {
            GradingManager.Instance.ProcessGrade(isAccuracyCorrect);
        }

        if (isAccuracyCorrect)
        {
            Debug.Log("Good grading! You caught it.");
        }
        else
        {
            Debug.Log("Bad grading! Mistake recorded.");
        }

        GetComponentInParent<PaperController>().RegisterGrade();
        return true;
    }
}