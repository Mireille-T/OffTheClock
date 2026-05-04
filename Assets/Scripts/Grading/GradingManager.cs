using UnityEngine;

public class GradingManager : MonoBehaviour
{
    public static GradingManager Instance;

    [Header("Scoring & Streaks")]
    public int currentScore = 0;
    public int currentStreak = 0;
    public int scorePerCorrect = 100;

    [Header("Timer Settings")]
    public float timeRemaining = 60f; // 1 minute shift
    private bool isShiftActive = true;

    [Header("Statistics")]
    public int totalGraded = 0;
    public int correctCount = 0;

    private GameManager gameManager = null;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (isShiftActive)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndShift();
            }
        }
    }

    public void ProcessGrade(bool isCorrect)
    {
        if (!isShiftActive) return;

        totalGraded++;
        if (gameManager != null)
        {
            gameManager.NumTotalPapers = totalGraded;
        }

        if (isCorrect)
        {
            correctCount++;
            if (gameManager != null)
            {
                gameManager.NumCorrectPapers = correctCount;
            }

            currentStreak++;
            
            int pointsGained = scorePerCorrect + (currentStreak * 10);
            currentScore += pointsGained;
            
            Debug.Log($"Correct! Streak: {currentStreak} | Points: +{pointsGained}");
        }
        else
        {
            currentStreak = 0;
            Debug.Log("Mistake! Streak Reset.");
        }
    }

    public void EndShift()
    {
        isShiftActive = false;
        string finalRank = CalculateRank();
        Debug.Log($"Shift Over! Final Score: {currentScore} | Rank: {finalRank}");
        
        // Trigger level done
    }

    private string CalculateRank()
    {
        if (totalGraded == 0) return "F";

        float accuracy = (float)correctCount / totalGraded;
        
        // Ranking logic based on accuracy and speed (score)
        if (accuracy >= 0.95f && currentScore > 1000) return "S";
        if (accuracy >= 0.80f) return "A";
        if (accuracy >= 0.60f) return "B";
        if (accuracy >= 0.40f) return "C";
        return "D";
    }
}