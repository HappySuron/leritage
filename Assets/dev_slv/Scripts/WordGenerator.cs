using UnityEngine;
using TMPro;

public class WordGenerator : MonoBehaviour
{
    public static WordGenerator Instance; // Singleton

    [Header("Prefabs")]
    public GameObject letterPrefab;
    public GameObject wordPrefab;

    [Header("Parent")]
    public Transform wordsParent; // По умолчанию, куда будут создаваться слова

    private void Awake()
    {
        // Singleton паттерн
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public GameObject GenerateWord(string word, Transform parent = null)
    {
        if (string.IsNullOrEmpty(word))
            return null;

        if (parent == null)
            parent = wordsParent;

        // Создаем префаб слова
        GameObject newWord = Instantiate(wordPrefab, parent);

        // Находим контейнер для букв
        Transform lettersContainer = newWord.transform.Find("LettersContainer");
        if (lettersContainer == null)
        {
            Debug.LogError("Не найден LettersContainer в wordPrefab!");
            return newWord;
        }

        // Генерируем буквы
        foreach (char c in word)
        {
            GameObject letterObj = Instantiate(letterPrefab, lettersContainer);

            // Меняем текст в TextMeshPro
            TextMeshProUGUI tmp = letterObj.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null)
            {
                tmp.text = c.ToString();
            }
            else
            {
                Debug.LogError("TextMeshProUGUI не найден в letterPrefab!");
            }
        }

        return newWord; // Возвращаем созданный объект
    }


    void Start()
    {
        this.GenerateWord("ZXC", wordsParent);
        this.GenerateWord("ZASD", wordsParent);
    }
}