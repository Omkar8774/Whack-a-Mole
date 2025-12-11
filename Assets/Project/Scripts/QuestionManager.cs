using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QuestionManager : MonoBehaviour
{
    public List<Question> questions = new List<Question>();

    // Index order for shuffled questions
    private List<int> order = new List<int>();
    private int currentIdx = 0;

    void Awake()
    {
        // Could load a sample JSON from Resources/questions.json
        // or populate questions via a form UI
    }

    public void InitializeShuffle()
    {
        order.Clear();
        for (int i = 0; i < questions.Count; i++) order.Add(i);

        // Fisher–Yates shuffle
        for (int i = order.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = order[i];
            order[i] = order[j];
            order[j] = tmp;
        }

        currentIdx = 0; // Reset index for new game
    }

    public Question GetNextQuestion()
    {
        if (questions.Count == 0) return null;
        if (currentIdx >= order.Count) return null;

        var q = questions[order[currentIdx]];
        currentIdx++;
        return q;
    }

    public void AddQuestion(Question q)
    {
        questions.Add(q);
        // Could persist this question to local JSON if needed
    }

    // Save current questions to a persistent JSON file
    public void SaveQuestionsToFile(string fileName = "questions_saved.json")
    {
        string json = JsonUtility.ToJson(new QuestionListWrapper { items = questions }, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);
        Debug.Log("Saved questions to: " + path);
    }

    [System.Serializable]
    private class QuestionListWrapper { public List<Question> items; }
}
