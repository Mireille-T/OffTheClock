using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PaperSpawner : MonoBehaviour
{
    [Header("Prefabs & Data")]
    public GameObject paperPrefab;
    public GameObject questionBlockPrefab;
    public QuestionBank questionBank;

    [Header("Movement Transforms")]
    public Transform pStart;
    public Transform pInter;
    public Transform pGrading;
    public Transform pDone;

    [Header("Settings")]
    public int questionsPerPaper = 3;

    void Start()
    {
        // Start the game by spawning the first paper
        SpawnPaper();
    }

    public void SpawnPaper()
    {
        // 1. Instantiate at Start Position
        GameObject paper = Instantiate(paperPrefab, pStart.position, pStart.rotation, pStart.parent);

        // 2. Setup UI (Names and Questions)
        SetupPaperUI(paper);

        // 3. Initialize the Movement/State Logic
        PaperController pc = paper.GetComponent<PaperController>();
        
        // We pass the transforms and the question count to the controller
        pc.Init(pStart, pInter, pGrading, pDone, questionsPerPaper);
    }

    private void SetupPaperUI(GameObject paper)
    {
        // Find the Name Text
        // Note: Make sure your Text object is named "StudentNameText" in the Paper prefab
        Transform nameTransform = paper.transform.Find("Object_0_0_0/Canvas/StudentNameText");
    
        if (nameTransform != null)
        {
            TextMeshProUGUI nameText = nameTransform.GetComponent<TextMeshProUGUI>();
            nameText.text = "Student: " + questionBank.studentNames[Random.Range(0, questionBank.studentNames.Count)];
        }
        else
        {
            Debug.LogError("Could not find StudentNameText! Check your object path.");
        }

        // Find the Questions Container
        Transform container = paper.transform.Find("Object_0_0_0/Canvas/QuestionsContainer");

        // Spawn the randomized questions
        for (int i = 0; i < questionsPerPaper; i++)
        {
            // Pick a random question from the bank
            QuestionData qData = questionBank.questions[Random.Range(0, questionBank.questions.Count)];

            // Instantiate the UI block
            GameObject block = Instantiate(questionBlockPrefab, container, false);

            block.transform.localScale = Vector3.one; 
            block.transform.localRotation = Quaternion.identity;

            // Inject the text
            block.transform.Find("QuestionText").GetComponent<TextMeshProUGUI>().text = qData.questionText;

            // Inject the image if it exists
            Image qImage = block.transform.Find("QuestionImage").GetComponent<Image>();
            if (qData.questionImage != null)
            {
                qImage.sprite = qData.questionImage;
                qImage.gameObject.SetActive(true);
            }
            else
            {
                qImage.gameObject.SetActive(false);
            }

            TextMeshProUGUI answerText = block.transform.Find("StudentAnswerText").GetComponent<TextMeshProUGUI>();
            QuestionController qController = block.GetComponent<QuestionController>();

            if (Random.value > 0.5f) 
            {
                answerText.text = "Answer: " + qData.correctAnswer;
                qController.isStudentAnswerCorrect = true;
            }
            else 
            {
                // Pick a random wrong answer from the list
                string randomWrong = qData.wrongAnswers[Random.Range(0, qData.wrongAnswers.Count)];
                answerText.text = "Answer: " + randomWrong;
                qController.isStudentAnswerCorrect = false;
            }
        }
    }
}