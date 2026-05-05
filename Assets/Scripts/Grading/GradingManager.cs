using UnityEngine;

public class GradingManager : MonoBehaviour
{
    public static GradingManager Instance;

    [Header("Global Scoring & Streaks")]
    public int currentScore = 0;
    public int currentStreak = 0;
    public int scorePerCorrect = 100;

    [Header("Session Statistics")]
    public int sessionTotalGraded = 0;
    public int sessionCorrectCount = 0;
    public bool isInGradingWorld = false;

    [Header("Buffer System Settings")]
    public float currentBufferTime = 60f;     
    public float maxBufferTime = 90f;         
    public float timeAddedPerPaper = 30f;     
    
    private bool isGameActive = true;

    [Header("Global Statistics")]
    public int totalGraded = 0;
    public int correctCount = 0;

    private GameManager gameManager = null;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (isGameActive)
        {
            currentBufferTime -= Time.deltaTime;
            
            if (currentBufferTime <= 0)
            {
                currentBufferTime = 0;
                OnBufferEmpty();
            }
        }
    }

    // call on tp to
    public void StartSession()
    {
        if (!isGameActive) return;

        isInGradingWorld = true;
        
        sessionTotalGraded = 0;
        sessionCorrectCount = 0;
        
        Debug.Log("Grading Session Started! Session stats reset.");
    }

    //call on tp away
    public void EndSession()
    {
        if (!isInGradingWorld) return;

        isInGradingWorld = false;
        
        // Calculate the rank based ONLY on the papers graded during this specific visit
        string sessionRank = CalculateSessionRank();
        
        Debug.Log($"Teleported out! Session Graded: {sessionTotalGraded} | Session Rank: {sessionRank}");
        
        // Optional: You could add logic here to reward the player based on their sessionRank!
    }

    public void ProcessGrade(bool isCorrect)
    {
        // Prevent grading if the game is over OR if the player is in the gamer world
        if (!isGameActive || !isInGradingWorld) return;

        // 1. Update Global Stats
        totalGraded++;
        if (gameManager != null) gameManager.NumTotalPapers = totalGraded;

        // 2. Update Session Stats
        sessionTotalGraded++;

        // 3. Add time back to the buffer
        AddTimeToBuffer(timeAddedPerPaper);

        if (isCorrect)
        {
            correctCount++;
            sessionCorrectCount++; // Track correct answers for this specific session
            
            if (gameManager != null) gameManager.NumCorrectPapers = correctCount;

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

    private void AddTimeToBuffer(float amountToAdd)
    {
        currentBufferTime += amountToAdd;
        if (currentBufferTime > maxBufferTime) currentBufferTime = maxBufferTime;
    }

    public void OnBufferEmpty()
    {
        isGameActive = false;
        isInGradingWorld = false;
        Debug.Log("Buffer Depleted! Calling external lose function...");
        
        SceneManager.LoadScene(1);
    }

    private string CalculateSessionRank()
    {
        // If they teleported in and out without doing anything
        if (sessionTotalGraded == 0) return "F";

        float accuracy = (float)sessionCorrectCount / sessionTotalGraded;
        
        if (accuracy >= 0.95f && sessionTotalGraded >= 5) return "S";
        if (accuracy >= 0.80f) return "A";
        if (accuracy >= 0.60f) return "B";
        if (accuracy >= 0.40f) return "C";
        return "D";
    }
}