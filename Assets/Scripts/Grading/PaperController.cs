using UnityEngine;
using System.Collections;

public class PaperController : MonoBehaviour
{
    public int totalQuestions;
    private int questionsGraded = 0;
    private bool isDone = false;

    // These will be passed in by the Spawner
    [HideInInspector] public Transform startPos, intermediatePos, gradingPos, donePos;

    public void Init(Transform s, Transform i, Transform g, Transform d, int qCount)
    {
        startPos = s; intermediatePos = i; gradingPos = g; donePos = d;
        totalQuestions = qCount;
        StartCoroutine(FlowRoutine());
    }

    public void RegisterGrade()
    {
        questionsGraded++;
        if (questionsGraded >= totalQuestions && !isDone)
        {
            isDone = true;
            StartCoroutine(ExitRoutine());
        }
    }

    IEnumerator FlowRoutine()
    {
        // 1. Move to Intermediate
        yield return StartCoroutine(MoveTo(intermediatePos.position, intermediatePos.rotation, 1.0f));
        // 2. Move to Main Grading Desk
        yield return StartCoroutine(MoveTo(gradingPos.position, gradingPos.rotation, 1.0f));
    }

    IEnumerator ExitRoutine()
    {
        // 1. Move back to Intermediate
        yield return StartCoroutine(MoveTo(intermediatePos.position, intermediatePos.rotation, 1.0f));
        // 2. Move to Done pile
        yield return StartCoroutine(MoveTo(donePos.position, donePos.rotation, 1.0f));

        FindObjectOfType<PaperSpawner>().SpawnPaper();
        
        // Optional: Destroy or deactivate after reaching done
        Destroy(gameObject, 1f); 
    }

    IEnumerator MoveTo(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        float time = 0;
        Vector3 startP = transform.position;
        Quaternion startR = transform.rotation;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startP, targetPos, time / duration);
            transform.rotation = Quaternion.Lerp(startR, targetRot, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}