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

    public int levelToLearnLetter = 5;

    public int learendLettersCount = 0;

    public Canvas winCanvas;

    // 🔥 быстрый доступ к визуалам
    private Dictionary<char, LetterKeyVisual> letterVisuals = new Dictionary<char, LetterKeyVisual>();



    [Header("Era's count")]
    public int letterToWin = 27;
    private int currentStageIndex = 0; 
    

    [SerializeField] private int[] stageThresholds  = { 5, 10, 15, 20, 27 };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLetters();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLetters()
    {
        if (lettersList.Count > 0) return;

        for (char c = 'A'; c <= 'Z'; c++)
        {
            lettersList.Add(new LetterData { letter = c, value = 0 });
        }
    }

    // 🔥 регистрация кнопок
    public void RegisterKey(LetterKeyVisual key)
    {
        char upper = char.ToUpper(key.Letter);

        //Debug.Log("Register key: " + upper); // 👈

        if (!letterVisuals.ContainsKey(upper))
        {
            letterVisuals.Add(upper, key);
        }

        // сразу обновляем визуал
        int level = GetLetterLevel(upper);
        key.UpdateVisual(level, levelToLearnLetter);
    }

    public int GetLetterLevel(char letter)
    {
        LetterData data = lettersList.Find(x => x.letter == char.ToUpper(letter));
        return data != null ? data.value : 0;
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

                    CheckLetterInFirstEnemies(letterPressed);
                }
            }
        }
    }

    public bool CheckLetterInAllWords(char letter)
    {
        bool found = false;

        Word[] allWords = Object.FindObjectsByType<Word>(FindObjectsSortMode.None);
        foreach (Word w in allWords)
        {
            if (w.CheckLetter(letter))  // предполагаем, что CheckLetter теперь возвращает bool
            {
                found = true;
            }
        }

        return found;
    }

    public bool CheckLetterInFirstEnemies(char letter)
    {
        bool found = false;

        // Перебираем все линии
        foreach (var linePair in WaveSpawner.Instance.enemiesByLine)
        {
            var lineEnemies = linePair.Value;

            if (lineEnemies.Count == 0) continue;

            // Берем только первого врага в линии
            Enemy firstEnemy = lineEnemies[0];
            if (firstEnemy.wordObject == null) continue;

            Word word = firstEnemy.wordObject.GetComponent<Word>();
            if (word != null && word.CheckLetter(letter))
            {
                found = true;
            }
        }

        return found;
    }

    public void LearnLetter(char letter, int value)
    {
        char upperLetter = char.ToUpper(letter);
        LetterData data = lettersList.Find(x => x.letter == upperLetter);

        if (data == null)
        {
            data = new LetterData { letter = upperLetter, value = value };
            lettersList.Add(data);
        }
        else
        {
            data.value = value;
        }

        UpdateLetterVisual(upperLetter, data.value);
    }

    public void ForgetLetter(char letter)
    {
        char upper = char.ToUpper(letter);
        lettersList.RemoveAll(x => x.letter == upper);

        UpdateLetterVisual(upper, 0);
    }

    public bool IsLetterAvailable(char letter)
    {
        LetterData data = lettersList.Find(x => x.letter == char.ToUpper(letter));
        return data != null && data.value >= levelToLearnLetter;
    }

    public void ChangeLetterLevel(char letter, int levelChange)
    {
        char upperLetter = char.ToUpper(letter);
        LetterData data = lettersList.Find(x => x.letter == upperLetter);

        if (data == null)
        {
            data = new LetterData { letter = upperLetter, value = 1 };
            lettersList.Add(data);
        }
        else
        {
            data.value += levelChange;
            if (data.value == levelToLearnLetter)
            {
                learendLettersCount++;
                Debug.Log("Letter " + upperLetter + " learned");

                CheckStageProgress(); // 🔥 вот тут вызываем проверку slv


                if (learendLettersCount >= letterToWin)
                {
                    Debug.Log("All letters learned");
                    winCanvas.gameObject.SetActive(true);
                }
            }
        }

        // 🔥 защита от отрицательных значений
        data.value = Mathf.Max(0, data.value);

        UpdateLetterVisual(upperLetter, data.value);
    }

    // 🔥 обновление визуала без FindObjects
    private void UpdateLetterVisual(char letter, int level)
    {
        if (letterVisuals.TryGetValue(letter, out var key))
        {
            key.UpdateVisual(level, levelToLearnLetter);
        }
    }

    private void CheckStageProgress()
    {
        // пока не вышли за границы массива и достигнут порог
        while (currentStageIndex < stageThresholds.Length && learendLettersCount >= stageThresholds[currentStageIndex])
        {
            // вызываем метод менеджера
            EraManager.Instance.MoveToNextEra();
            currentStageIndex++;
        }
    }
}