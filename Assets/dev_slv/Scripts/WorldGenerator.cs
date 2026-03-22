using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator Instance; // Singleton

    [Header("Prefabs")]
    public GameObject letterPrefab;
    public GameObject wordPrefab;

    public GameObject enemyPrefab;

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

    public GameObject GenerateEnemyWithWord(GameObject enemyPrefab, Vector3 position, string word)
    {
        // создаем врага
        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);

        // ищем точку для слова
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("На enemyPrefab нет скрипта Enemy!");
            return enemyObj;
        }

        // генерим слово как дочерний объект
        GameObject wordObj = GenerateWord(word, enemy.wordSpawnPoint);

        // можно сохранить ссылку (если нужно)
        enemy.SetWordObject(wordObj);
        Word wordComponent = wordObj.GetComponent<Word>();
        if (wordComponent != null)
        {
            wordComponent.SetOwner(enemy);
            Debug.Log(enemy);
        }
        return enemyObj;
    }


    void Start()
    {
        //this.GenerateWord("ZXC", wordsParent);
        //this.GenerateWord("ZASD", wordsParent);

        //this.GenerateEnemyWithWord(enemyPrefab, wordsParent.position, "ZXC");
        
        // GameObject enemyObj = this.GenerateEnemyWithWord(enemyPrefab, new Vector3(0, 0, 50), "IMTREX");
        // Enemy enemy = enemyObj.GetComponent<Enemy>();
        // enemy.MoveTo(new Vector3(0, 0, -100));
    }
}