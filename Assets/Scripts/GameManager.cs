using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string GRADING_TEXT_PREFIX = "Correctly graded papers: ";
    private const string GRADING_TEXT_SEPARATOR = "/";

    [SerializeField] private TMP_Text classroomGradingText = null;

    private int numCorrectPapers = 0;
    public int NumCorrectPapers
    {
        set
        {
            numCorrectPapers = value;
            UpdateGradingUI();
        }
    }

    private int numTotalPapers = 0;
    public int NumTotalPapers
    {
        set
        {
            numTotalPapers = value;
            UpdateGradingUI();
        }
    }

    private void Start()
    {
        UpdateGradingUI();
    }

    private void UpdateGradingUI()
    {
        if (classroomGradingText != null)
        {
            classroomGradingText.text = GRADING_TEXT_PREFIX + numCorrectPapers.ToString()
                + GRADING_TEXT_SEPARATOR + numTotalPapers.ToString();
        }
    }
}
