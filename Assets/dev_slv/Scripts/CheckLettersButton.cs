using UnityEngine;

public class CheckLetterKeyboard : MonoBehaviour
{
    void Update()
    {
        // Перебираем все возможные нажатия букв
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                string keyString = kcode.ToString();

                // Берем только одиночные символы (A-Z)
                if (keyString.Length == 1)
                {
                    char letterPressed = keyString[0];

                    // Проверяем все слова на сцене
                    Word[] allWords = Object.FindObjectsByType<Word>(FindObjectsSortMode.None);
                    foreach (Word w in allWords)
                    {
                        w.CheckLetter(letterPressed);
                    }
                }
            }
        }
    }
}