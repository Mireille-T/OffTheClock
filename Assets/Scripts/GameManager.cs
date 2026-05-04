using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string GRADING_TEXT_PREFIX = "Correctly graded papers: ";
    private const string GRADING_TEXT_SEPARATOR = "/";
    private const string ENEMY_TEXT_PREFIX = "Enemies defeated: ";

    [SerializeField] private TMP_Text classroomGradingText = null;
    [SerializeField] private TMP_Text mechaGradingText = null;
    [SerializeField] private TMP_Text classroomEnemyText = null;
    [SerializeField] private TMP_Text mechaEnemyText = null;

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

    private int numEnemiesDefeated = 0;

    private void Start()
    {
        UpdateGradingUI();
    }

    private void UpdateGradingUI()
    {
        string gradingText = GRADING_TEXT_PREFIX + numCorrectPapers.ToString()
                + GRADING_TEXT_SEPARATOR + numTotalPapers.ToString();

        if (classroomGradingText != null)
        {
            classroomGradingText.text = gradingText;
        }

        if (mechaGradingText != null)
        {
            mechaGradingText.text = gradingText;
        }
    }

    private void UpdateEnemyUI()
    {
        string enemyText = ENEMY_TEXT_PREFIX + numEnemiesDefeated;

        if (classroomEnemyText != null)
        {
            classroomEnemyText.text = enemyText;
        }

        if (mechaEnemyText != null)
        {
            mechaEnemyText.text = enemyText;
        }
    }

    public void AddToNumEnemiesDefeated(int addValue = 1)
    {
        numEnemiesDefeated += addValue;
        UpdateEnemyUI();
    }
}
