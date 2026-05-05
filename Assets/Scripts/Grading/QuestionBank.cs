using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionData
{
    [TextArea(3, 5)]
    public string questionText;
    public Sprite questionImage; 
    
    public string correctAnswer;
    public List<string> wrongAnswers; 
}

[CreateAssetMenu(fileName = "NewQuestionBank", menuName = "GradingGame/Question Bank")]
public class QuestionBank : ScriptableObject
{
    public List<string> studentNames;
    public List<QuestionData> questions;
}