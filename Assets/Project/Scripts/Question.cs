using System;

[Serializable]
public class Question
{
    public string prompt;          
    public string correctAnswer;   
    public string[] wrongAnswers; 

    public string[] GetShuffledOptions()
    {
        var arr = new string[3];
        arr[0] = correctAnswer;
        arr[1] = wrongAnswers[0];
        arr[2] = wrongAnswers[1];
        
        // simple Fisher-Yates shuffle
        System.Random rnd = new System.Random();
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            var tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }
        return arr;
    }
}
