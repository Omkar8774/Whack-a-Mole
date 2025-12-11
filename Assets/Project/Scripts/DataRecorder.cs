using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ResponseRecord
{
    public string question;
    public string chosenAnswer;
    public bool correct;
    public string timestamp;
}

[Serializable]
public class ResponseList
{
    public List<ResponseRecord> responses = new List<ResponseRecord>();
}

public class DataRecorder : MonoBehaviour
{
    private ResponseList list = new ResponseList();

    public void Clear() { list.responses.Clear(); }

    public void RecordResponse(string question, string chosen, bool correct, DateTime time)
    {
        list.responses.Add(new ResponseRecord
        {
            question = question,
            chosenAnswer = chosen,
            correct = correct,
            timestamp = time.ToString("o")
        });
    }

    public void SaveToFile(string filename = "responses.json")
    {
        string json = JsonUtility.ToJson(list, true);
        string path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(path, json);
        Debug.Log("Responses saved to: " + path);
    }
}
