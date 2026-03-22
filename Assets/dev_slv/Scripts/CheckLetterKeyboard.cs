using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LetterData
{
    public char letter;
    public int value;
}

public class CheckLetterKeyboard : MonoBehaviour
{
    public static CheckLetterKeyboard Instance { get; private set; }

    [SerializeField] private List<LetterData> lettersList = new List<LetterData>();

    public int levelToLearnLetter =5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLetters(); // заполняем список букв
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLetters()
    {
        if (lettersList.Count > 0) return; // не перезаписываем, если уже есть данные

        lettersList = new List<LetterData>();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            lettersList.Add(new LetterData { letter = c, value = 0 });
        }
    }

    void Update()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                string keyString = kcode.ToString();

                if (keyString.Length == 1)
                {
                    char letterPressed = char.ToUpper(keyString[0]);

                    LetterData data = lettersList.Find(x => x.letter == letterPressed);
                    if (data == null || data.value < levelToLearnLetter)
                        continue;

                    CheckLetterInAllWords(letterPressed);
                }
            }
        }
    }

    public void CheckLetterInAllWords(char letter)
    {
        Word[] allWords = Object.FindObjectsByType<Word>(FindObjectsSortMode.None);
        foreach (Word w in allWords)
        {
            w.CheckLetter(letter);
        }
    }

    public void LearnLetter(char letter, int value)
    {
        char upperLetter = char.ToUpper(letter);
        LetterData data = lettersList.Find(x => x.letter == upperLetter);

        if (data == null)
        {
            lettersList.Add(new LetterData { letter = upperLetter, value = value });
        }
        else
        {
            data.value = value;
        }
    }

    public void ForgetLetter(char letter)
    {
        lettersList.RemoveAll(x => x.letter == char.ToUpper(letter));
    }

    public bool IsLetterAvailable(char letter)
    {
        LetterData data = lettersList.Find(x => x.letter == char.ToUpper(letter));
        return data != null && data.value > 5;
    }

    public void ChangeLetterLevel(char letter, int levelChange)
    {
        char upperLetter = char.ToUpper(letter);
        LetterData data = lettersList.Find(x => x.letter == upperLetter);

        if (data == null)
        {
            // Буквы ещё нет — добавляем с уровнем 1
            lettersList.Add(new LetterData { letter = upperLetter, value = 1 });
        }
        else
        {
            data.value += levelChange;
        }
    }
}